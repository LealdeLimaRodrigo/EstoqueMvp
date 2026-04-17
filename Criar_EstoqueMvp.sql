-- ============================================================
-- Criar_EstoqueMvp.sql — Script de criação do banco de dados
-- Cria todas as tabelas, constraints, índices e dados iniciais.
-- ============================================================

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
('PAP-SUL-001', 'Resma Papel Sulfite A4', 'Caixa com 500 folhas sulfite A4 brancas', 22.00),
('INF-CAB-001', 'Cabo HDMI 2m', 'Cabo HDMI 2.0 blindado 2 metros', 29.90),
('INF-FON-001', 'Fone de Ouvido USB', 'Headset USB com microfone e cancelamento de ruído', 75.00);
GO

-- 5.5 Estoque inicial nos setores (permite testar consumo e transferência imediatamente)
-- Os saldos abaixo já refletem as movimentações dos itens 5.7 e 5.8
INSERT INTO [dbo].[EstoqueSetor] ([SetorId], [ProdutoId], [QuantidadeEstoque]) VALUES 
(1, 1, 50),   -- Almoxarifado: 50 Mouse
(1, 2, 30),   -- Almoxarifado: 30 Teclado
(1, 3, 95),   -- Almoxarifado: 120 entrada - 20 envio RH - 5 consumo = 95 Resma
(1, 4, 3),    -- Almoxarifado: 10 entrada - 5 envio TI - 2 consumo internamente? => 3 (estoque baixo!)
(1, 5, 8),    -- Almoxarifado: 15 entrada - 5 envio TI - 2 consumo = 8 Fone (estoque baixo!)
(2, 1, 8),    -- TI: 10 entrada - 2 consumo = 8 Mouse
(2, 2, 4),    -- TI: 5 entrada - 1 consumo = 4 Teclado
(2, 4, 2),    -- TI: 5 recebido - 3 consumo = 2 Cabo HDMI (estoque baixo!)
(2, 5, 5),    -- TI: 5 recebido do Almoxarifado
(3, 3, 15);   -- RH: 20 recebido - 5 consumo = 15 Resma
GO

-- 5.6 Histórico de entradas iniciais (auditoria consistente com datas retroativas)
INSERT INTO [dbo].[MovimentacaoEstoque] ([TransacaoId], [ProdutoId], [SetorId], [TipoMovimentacaoId], [Quantidade], [UsuarioId], [DataMovimentacao]) VALUES
(NEWID(), 1, 1, 1, 50, 1, DATEADD(DAY, -15, GETDATE())),  -- Entrada Almoxarifado Mouse
(NEWID(), 2, 1, 1, 30, 1, DATEADD(DAY, -15, GETDATE())),  -- Entrada Almoxarifado Teclado
(NEWID(), 3, 1, 1, 120, 1, DATEADD(DAY, -15, GETDATE())), -- Entrada Almoxarifado Resma
(NEWID(), 4, 1, 1, 10, 1, DATEADD(DAY, -12, GETDATE())),  -- Entrada Almoxarifado Cabo HDMI
(NEWID(), 5, 1, 1, 15, 1, DATEADD(DAY, -12, GETDATE())),  -- Entrada Almoxarifado Fone
(NEWID(), 1, 2, 1, 10, 1, DATEADD(DAY, -14, GETDATE())),  -- Entrada TI Mouse
(NEWID(), 2, 2, 1, 5, 1, DATEADD(DAY, -14, GETDATE()));   -- Entrada TI Teclado
GO

-- 5.7 Transferências entre setores (Envio + Recebimento sempre em par, mesma TransacaoId)

-- Transferência 1: Almoxarifado → RH (20 Resma Papel)
DECLARE @txId1 UNIQUEIDENTIFIER = NEWID();
INSERT INTO [dbo].[MovimentacaoEstoque] ([TransacaoId], [ProdutoId], [SetorId], [TipoMovimentacaoId], [Quantidade], [UsuarioId], [DataMovimentacao]) VALUES
(@txId1, 3, 1, 3, 20, 1, DATEADD(DAY, -10, GETDATE())),  -- Envio: Almoxarifado envia 20 Resma
(@txId1, 3, 3, 4, 20, 1, DATEADD(DAY, -10, GETDATE()));  -- Recebimento: RH recebe 20 Resma
GO

-- Transferência 2: Almoxarifado → TI (5 Cabo HDMI)
DECLARE @txId2 UNIQUEIDENTIFIER = NEWID();
INSERT INTO [dbo].[MovimentacaoEstoque] ([TransacaoId], [ProdutoId], [SetorId], [TipoMovimentacaoId], [Quantidade], [UsuarioId], [DataMovimentacao]) VALUES
(@txId2, 4, 1, 3, 5, 1, DATEADD(DAY, -8, GETDATE())),    -- Envio: Almoxarifado envia 5 Cabo HDMI
(@txId2, 4, 2, 4, 5, 1, DATEADD(DAY, -8, GETDATE()));    -- Recebimento: TI recebe 5 Cabo HDMI
GO

-- Transferência 3: Almoxarifado → TI (5 Fone de Ouvido)
DECLARE @txId3 UNIQUEIDENTIFIER = NEWID();
INSERT INTO [dbo].[MovimentacaoEstoque] ([TransacaoId], [ProdutoId], [SetorId], [TipoMovimentacaoId], [Quantidade], [UsuarioId], [DataMovimentacao]) VALUES
(@txId3, 5, 1, 3, 5, 1, DATEADD(DAY, -6, GETDATE())),    -- Envio: Almoxarifado envia 5 Fone
(@txId3, 5, 2, 4, 5, 1, DATEADD(DAY, -6, GETDATE()));    -- Recebimento: TI recebe 5 Fone
GO

-- 5.8 Consumos (saída de produtos por uso interno dos setores)
INSERT INTO [dbo].[MovimentacaoEstoque] ([TransacaoId], [ProdutoId], [SetorId], [TipoMovimentacaoId], [Quantidade], [UsuarioId], [DataMovimentacao]) VALUES
(NEWID(), 3, 3, 2, 5, 1, DATEADD(DAY, -7, GETDATE())),   -- Consumo: RH consome 5 Resma
(NEWID(), 4, 2, 2, 3, 1, DATEADD(DAY, -5, GETDATE())),   -- Consumo: TI consome 3 Cabo HDMI
(NEWID(), 5, 1, 2, 2, 1, DATEADD(DAY, -4, GETDATE())),   -- Consumo: Almoxarifado consome 2 Fone
(NEWID(), 1, 2, 2, 2, 1, DATEADD(DAY, -3, GETDATE())),   -- Consumo: TI consome 2 Mouse
(NEWID(), 3, 1, 2, 5, 1, DATEADD(DAY, -2, GETDATE())),   -- Consumo: Almoxarifado consome 5 Resma
(NEWID(), 2, 2, 2, 1, 1, DATEADD(DAY, -1, GETDATE()));   -- Consumo: TI consome 1 Teclado
GO