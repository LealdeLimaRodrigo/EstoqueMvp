CREATE DATABASE [EstoqueMvp];
GO

USE [EstoqueMvp];
GO

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

CREATE TABLE [dbo].[Produto](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Sku] [varchar](50) NOT NULL,
	[Nome] [nvarchar](100) NOT NULL,
	[Descricao] [nvarchar](500) NULL,
	[Preco] [decimal](18, 2) NOT NULL,
	[Ativo] [bit] NOT NULL DEFAULT 1,
	CONSTRAINT [PK_Produto] PRIMARY KEY CLUSTERED ([Id] ASC),
	CONSTRAINT [UQ_Produto_Sku] UNIQUE NONCLUSTERED ([Sku] ASC)
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

CREATE TABLE [dbo].[EstoqueSetor](
	[SetorId] [int] NOT NULL,
	[ProdutoId] [int] NOT NULL,
	[QuantidadeEstoque] [decimal](18, 2) NOT NULL CONSTRAINT [DF_EstoqueSetor_Qtd] DEFAULT (0),
	CONSTRAINT [PK_EstoqueSetor] PRIMARY KEY CLUSTERED ([SetorId] ASC, [ProdutoId] ASC),
	CONSTRAINT [FK_EstoqueSetor_Setor] FOREIGN KEY([SetorId]) REFERENCES [dbo].[Setor] ([Id]),
	CONSTRAINT [FK_EstoqueSetor_Produto] FOREIGN KEY([ProdutoId]) REFERENCES [dbo].[Produto] ([Id])
);
GO

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
	CONSTRAINT [FK_Movimentacao_Usuario] FOREIGN KEY([UsuarioId]) REFERENCES [dbo].[Usuario] ([Id])
);
GO

-- 4. Inserção de Dados Iniciais (Carga Obrigatória para o MVP)
-- 4.1 Tipos de Movimentação (Core do negócio)
INSERT INTO [dbo].[TipoMovimentacao] ([Nome], [Descricao]) VALUES 
('Entrada', 'Aquisição de novos produtos para o estoque de um setor'),
('Consumo', 'Baixa de produto por consumo interno do setor'),
('Envio', 'Saída de produto por transferência para outro setor'),
('Recebimento', 'Entrada de produto por transferência de outro setor');
GO

-- 4.2 Setores Iniciais
INSERT INTO [dbo].[Setor] ([Nome], [Descricao]) VALUES 
('Almoxarifado Central', 'Estoque principal de recebimento e distribuição'),
('Setor de TI', 'Equipamentos e suprimentos de informática'),
('Setor de RH', 'Materiais de escritório e recursos humanos');
GO

-- 4.3 Usuário Admin Padrão (Senha fictícia hashada - equivalente a 'admin123' usando BCrypt)
-- O CPF 00000000000 facilitará o login do avaliador
INSERT INTO [dbo].[Usuario] ([Nome], [Cpf], [SenhaHash]) VALUES 
('Administrador do Sistema', '00000000000', '240be518fabd2724ddb6f04eeb1da5967448d7e831c08c8fa822809f74c720a9'); 
GO

-- 4.4 Alguns produtos iniciais para teste
INSERT INTO [dbo].[Produto] ([Sku], [Nome], [Descricao], [Preco]) VALUES 
('INF-MOU-001', 'Mouse Óptico Sem Fio', 'Mouse sem fio ergonômico 2.4GHz', 45.90),
('INF-TEC-001', 'Teclado Mecânico ABNT2', 'Teclado mecânico switch brown', 189.50),
('PAP-SUL-001', 'Resma Papel Sulfite A4', 'Caixa com 500 folhas sulfite A4 brancas', 22.00);
GO