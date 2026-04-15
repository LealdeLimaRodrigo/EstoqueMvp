# 📦 Sistema de Gestão de Estoque (MVP)

Este projeto é uma solução Full Stack desenvolvida em **ASP.NET 4.8** para o gerenciamento de inventário, movimentações entre setores e controle de acesso de usuários. A arquitetura foi desenhada seguindo os princípios de separação de responsabilidades e alta performance.

## 🏗️ Arquitetura e Tecnologias

A solução está dividida em camadas para garantir a manutenibilidade e escalabilidade do código:

* **Backend (Web API):** Construído em ASP.NET 4.8, utilizando **Dapper** para persistência de dados de alta performance e **Unity** para injeção de dependência.
* **Segurança:** Implementação de **JWT (JSON Web Token)** via OWIN para proteção de rotas e autenticação baseada em CPF e Senha (SHA-256).
* **Frontend (Web):** Desenvolvido com **Razor Views (.cshtml)** e **Bootstrap**, integrando-se à API via chamadas assíncronas para uma experiência dinâmica.
* **Regras de Negócio:** Validação de CPF customizada, geração automática de **SKU** e controle rigoroso de saldo por setor.
* **Qualidade de Código:** Tratamento global de exceções, validações com **FluentValidation** e documentação interativa com **Swagger**.

---

## 🚀 Instruções de Configuração e Execução

### 1. Preparação do Banco de Dados
O sistema utiliza SQL Server para persistência de dados.
1. Localize o arquivo `Criar_EstoqueMvp.sql` na pasta `Scripts` (ou na raiz).
2. Execute o script no seu servidor SQL Server. Ele criará o banco `EstoqueMvp`, as tabelas e realizará a carga inicial de:
   * Tipos de Movimentação (Entrada, Consumo, Envio, Recebimento).
   * Setores e Produtos para teste.
   * Usuário Administrador padrão.

### 2. Configuração da Conexão (Web.config)
No projeto **`EstoqueMvp.Api`**, abra o arquivo `Web.config` e ajuste a string de conexão para apontar para o seu servidor local. Procure pela tag de connectionStrings e deixe assim:

<connectionStrings>
  <add name="EstoqueMvpConnection" connectionString="Data Source=SEU_SERVIDOR;Initial Catalog=EstoqueMvp;Integrated Security=True" providerName="System.Data.SqlClient" />
</connectionStrings>

### 3. Execução da Solução
1. Abra o arquivo `EstoqueMvp.sln` no Visual Studio 2022.
2. Clique com o botão direito na **Solution** (no topo do Gerenciador de Soluções) e selecione "Set Startup Projects".
3. Escolha "Multiple startup projects" e configure tanto a **`EstoqueMvp.Api`** quanto a **`EstoqueMvp.Web`** para a ação **Start**.
4. Pressione `F5`.

---

## 🔑 Credenciais para Teste

Para realizar o login e testar as funcionalidades protegidas:
* **CPF:** 00000000000
* **Senha:** admin123

O sistema gerará um token JWT que será armazenado na sessão do navegador para autorizar as operações de CRUD e Movimentação.

---

## 🛠️ Funcionalidades Implementadas
* **CRUD de Produtos:** Cadastro com geração automática de SKU e controle de status (Ativo/Inativo).
* **Controle de Estoque por Setor:** Visão detalhada de quantidades disponíveis em cada departamento.
* **Movimentações:**
  * **Entrada:** Incremento de estoque com registro de transação.
  * **Consumo:** Baixa de itens com validação de saldo insuficiente.
  * **Transferência entre Setores:** Débito e crédito simultâneos em transação atômica.
* **Gestão de Usuários:** Cadastro completo com validação de CPF e criptografia de senha.

---

**Desenvolvido por Rodrigo Leal de Lima**