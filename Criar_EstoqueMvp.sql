CREATE DATABASE [EstoqueMvp];
GO

USE [EstoqueMvp];
GO

-- ============================================================
-- 1. Tabelas de domínio (Setor, TipoMovimentacao)
-- ============================================================

CREATE TABLE [dbo].[Setor](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Nome] [nvarchar](100) NOT NULL,
	[Descricao] [nvarchar](500) NULL,
	[Ativo] [bit] NOT NULL DEFAULT 1,
	CONSTRAINT [PK_Setor] PRIMARY KEY CLUSTERED ([Id] ASC)
);
GO

CREATE TABLE [dbo].[TipoMovimentacao](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Nome] [nvarchar](100) NOT NULL,
	[Descricao] [nvarchar](500) NULL,
	CONSTRAINT [PK_TipoMovimentacao] PRIMARY KEY CLUSTERED ([Id] ASC)
);
GO

-- ============================================================
-- 2. Tabelas principais (Produto, Usuario)
-- ============================================================

CREATE TABLE [dbo].[Produto](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Sku] [varchar](50) NOT NULL,
	[Nome] [nvarchar](100) NOT NULL,
	[Descricao] [nvarchar](500) NULL,
	[Preco] [decimal](18, 2) NOT NULL,
	[Ativo] [bit] NOT NULL DEFAULT 1,
	CONSTRAINT [PK_Produto] PRIMARY KEY CLUSTERED ([Id] ASC),
	CONSTRAINT [UQ_Produto_Sku] UNIQUE NONCLUSTERED ([Sku] ASC),
	CONSTRAINT [CK_Produto_Preco] CHECK ([Preco] > 0)
);
GO

CREATE TABLE [dbo].[Usuario](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Nome] [nvarchar](100) NOT NULL,
	[Cpf] [varchar](11) NOT NULL,
	[SenhaHash] [varchar](255) NOT NULL,
	[Ativo] [bit] NOT NULL DEFAULT 1,
	CONSTRAINT [PK_Usuario] PRIMARY KEY CLUSTERED ([Id] ASC),
	CONSTRAINT [UQ_Usuario_Cpf] UNIQUE NONCLUSTERED ([Cpf] ASC)
);
GO

-- ============================================================
-- 3. Tabela de saldo de estoque por setor (chave composta)
-- ============================================================

CREATE TABLE [dbo].[EstoqueSetor](
	[SetorId] [int] NOT NULL,
	[ProdutoId] [int] NOT NULL,
	[QuantidadeEstoque] [decimal](18, 2) NOT NULL CONSTRAINT [DF_EstoqueSetor_Qtd] DEFAULT (0),
	CONSTRAINT [PK_EstoqueSetor] PRIMARY KEY CLUSTERED ([SetorId] ASC, [ProdutoId] ASC),
	CONSTRAINT [FK_EstoqueSetor_Setor] FOREIGN KEY([SetorId]) REFERENCES [dbo].[Setor] ([Id]),
	CONSTRAINT [FK_EstoqueSetor_Produto] FOREIGN KEY([ProdutoId]) REFERENCES [dbo].[Produto] ([Id]),
	CONSTRAINT [CK_EstoqueSetor_Quantidade] CHECK ([QuantidadeEstoque] >= 0)
);
GO

-- Índice para consultas que filtram apenas por ProdutoId
-- (a PK clusterizada começa por SetorId, logo queries por ProdutoId não a utilizam)
CREATE NONCLUSTERED INDEX [IX_EstoqueSetor_ProdutoId] ON [dbo].[EstoqueSetor] ([ProdutoId]);
GO

-- ============================================================
-- 4. Tabela de histórico de movimentações (append-only)
-- ============================================================

CREATE TABLE [dbo].[MovimentacaoEstoque](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[TransacaoId] [uniqueidentifier] NOT NULL,
	[ProdutoId] [int] NOT NULL,
	[SetorId] [int] NOT NULL,
	[TipoMovimentacaoId] [int] NOT NULL,
	[Quantidade] [decimal](18, 2) NOT NULL,
	[DataMovimentacao] [datetime2](7) NOT NULL CONSTRAINT [DF_Movimentacao_Data] DEFAULT (GETDATE()),
	[UsuarioId] [int] NOT NULL,
	CONSTRAINT [PK_MovimentacaoEstoque] PRIMARY KEY CLUSTERED ([Id] ASC),
	CONSTRAINT [FK_Movimentacao_Produto] FOREIGN KEY([ProdutoId]) REFERENCES [dbo].[Produto] ([Id]),
	CONSTRAINT [FK_Movimentacao_Setor] FOREIGN KEY([SetorId]) REFERENCES [dbo].[Setor] ([Id]),
	CONSTRAINT [FK_Movimentacao_Tipo] FOREIGN KEY([TipoMovimentacaoId]) REFERENCES [dbo].[TipoMovimentacao] ([Id]),
	CONSTRAINT [FK_Movimentacao_Usuario] FOREIGN KEY([UsuarioId]) REFERENCES [dbo].[Usuario] ([Id]),
	CONSTRAINT [CK_Movimentacao_Quantidade] CHECK ([Quantidade] > 0)
);
GO

-- Índices para consultas frequentes por filtros individuais
CREATE NONCLUSTERED INDEX [IX_MovimentacaoEstoque_ProdutoId] ON [dbo].[MovimentacaoEstoque] ([ProdutoId]);
CREATE NONCLUSTERED INDEX [IX_MovimentacaoEstoque_SetorId] ON [dbo].[MovimentacaoEstoque] ([SetorId]);
CREATE NONCLUSTERED INDEX [IX_MovimentacaoEstoque_UsuarioId] ON [dbo].[MovimentacaoEstoque] ([UsuarioId]);
CREATE NONCLUSTERED INDEX [IX_MovimentacaoEstoque_DataMovimentacao] ON [dbo].[MovimentacaoEstoque] ([DataMovimentacao]);
CREATE NONCLUSTERED INDEX [IX_MovimentacaoEstoque_TransacaoId] ON [dbo].[MovimentacaoEstoque] ([TransacaoId]);
GO

-- ============================================================
-- 5. Carga inicial de dados (Seed obrigatório para o MVP)
-- ============================================================

-- 5.1 Tipos de Movimentação (valores fixos, mapeados pelo enum TipoMovimentacaoEnum)
INSERT INTO [dbo].[TipoMovimentacao] ([Nome], [Descricao]) VALUES 
('Entrada', 'Aquisição de novos produtos para o estoque de um setor'),
('Consumo', 'Baixa de produto por consumo interno do setor'),
('Envio', 'Saída de produto por transferência para outro setor'),
('Recebimento', 'Entrada de produto por transferência de outro setor');
GO

-- 5.2 Setores iniciais
INSERT INTO [dbo].[Setor] ([Nome], [Descricao]) VALUES 
('Almoxarifado Central', 'Estoque principal de recebimento e distribuição'),
('Setor de TI', 'Equipamentos e suprimentos de informática'),
('Setor de RH', 'Materiais de escritório e recursos humanos');
GO

-- 5.3 Usuário Admin Padrão (Senha: 'admin123' hashada com BCrypt)
-- O CPF 00000000000 facilita o login do avaliador durante os testes
INSERT INTO [dbo].[Usuario] ([Nome], [Cpf], [SenhaHash]) VALUES 
('Administrador do Sistema', '00000000000', '$2a$11$4s.kriSt.AcKeH2aSZpRWeE4.04CHDBLFaCIN6zCOQMqZ7a2k5DWK');
GO

-- 5.4 Produtos iniciais para teste
INSERT INTO [dbo].[Produto] ([Sku], [Nome], [Descricao], [Preco]) VALUES 
('INF-MOU-001', 'Mouse Óptico Sem Fio', 'Mouse sem fio ergonômico 2.4GHz', 45.90),
('INF-TEC-001', 'Teclado Mecânico ABNT2', 'Teclado mecânico switch brown', 189.50),
('PAP-SUL-001', 'Resma Papel Sulfite A4', 'Caixa com 500 folhas sulfite A4 brancas', 22.00);
GO

-- 5.5 Estoque inicial nos setores (permite testar consumo e transferência imediatamente)
INSERT INTO [dbo].[EstoqueSetor] ([SetorId], [ProdutoId], [QuantidadeEstoque]) VALUES 
(1, 1, 50),   -- Almoxarifado: 50 Mouse
(1, 2, 30),   -- Almoxarifado: 30 Teclado
(1, 3, 100),  -- Almoxarifado: 100 Resma
(2, 1, 10),   -- TI: 10 Mouse
(2, 2, 5);    -- TI: 5 Teclado
GO

-- 5.6 Histórico correspondente ao estoque inicial para consistência de auditoria
INSERT INTO [dbo].[MovimentacaoEstoque] ([TransacaoId], [ProdutoId], [SetorId], [TipoMovimentacaoId], [Quantidade], [UsuarioId], [DataMovimentacao]) VALUES
(NEWID(), 1, 1, 1, 50, 1, GETDATE()), -- Entrada inicial Almoxarifado
(NEWID(), 2, 1, 1, 30, 1, GETDATE()), -- Entrada inicial Almoxarifado
(NEWID(), 3, 1, 1, 100, 1, GETDATE()),-- Entrada inicial Almoxarifado
(NEWID(), 1, 2, 1, 10, 1, GETDATE()), -- Entrada inicial TI
(NEWID(), 2, 2, 1, 5, 1, GETDATE());  -- Entrada inicial TI
GO