# 📦 Sistema de Gestão de Estoque (MVP)

Este projeto é uma solução Full Stack desenvolvida em **ASP.NET 4.8** para o gerenciamento de inventário, movimentações entre setores e controle de acesso de usuários. A arquitetura foi desenhada seguindo os princípios de separação de responsabilidades e alta performance.

## 🏗️ Arquitetura e Tecnologias

A solução está dividida em camadas para garantir a manutenibilidade e escalabilidade do código:

* **Backend (Web API):** Construído em ASP.NET 4.8, utilizando **Dapper** para persistência de dados de alta performance e **Unity** para injeção de dependência.
* **Segurança:** Implementação de **JWT (JSON Web Token)** via OWIN para proteção de rotas e autenticação baseada em CPF e Senha (BCrypt com salt automático).
* **Frontend (Web):** Desenvolvido com **Razor Views (.cshtml)** e **Bootstrap**, integrando-se à API via chamadas assíncronas para uma experiência dinâmica.
* **Regras de Negócio:** Validação de CPF customizada, geração automática de **SKU** e controle rigoroso de saldo por setor.
* **Qualidade de Código:** Tratamento global de exceções, validações com **FluentValidation** e documentação interativa com **Swagger**.

---

## 🚀 Instruções de Configuração e Execução

### 1. Preparação do Banco de Dados
O sistema utiliza SQL Server para persistência de dados.
1. Localize o arquivo `Criar_EstoqueMvp.sql` na raiz do repositório.
2. Execute o script no seu servidor SQL Server. Ele criará o banco `EstoqueMvp`, as tabelas e realizará a carga inicial de:
   * Tipos de Movimentação (Entrada, Consumo, Envio, Recebimento).
   * Setores e Produtos para teste.
   * Usuário Administrador padrão.

### 2. Configuração da Conexão (Web.config)
No projeto **`EstoqueMvp.Api`**, abra o arquivo `Web.config` e ajuste a string de conexão para apontar para o seu servidor local. Procure pela tag de connectionStrings e deixe assim:

<connectionStrings>
  <add name="EstoqueConexao" connectionString="Data Source=SEU_SERVIDOR;Initial Catalog=EstoqueMvp;Integrated Security=True" providerName="System.Data.SqlClient" />
</connectionStrings>

> ⚠️ **Nota sobre Segurança (JWT):** A chave secreta (`JwtSecretKey`) está atualmente definida no `Web.config` para facilitar a execução local do MVP. Em um ambiente de produção real, essa chave deve ser movida para um cofre de chaves (como *Azure Key Vault*) ou injetada via Variáveis de Ambiente do servidor. Além disso, recomenda-se a implementação de um *Rate Limiting* no endpoint de login para proteção contra ataques de força bruta.

### 3. Execução da Solução
1. Abra o arquivo `EstoqueMvp.sln` no Visual Studio.
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

## 🏛️ Decisões Arquiteturais (ADRs)

Ao revisar o código deste MVP, você notará padrões modernos implementados por design:

1. **Dapper vs Entity Framework:** Escolhemos o Dapper (Micro-ORM) para garantir a máxima performance nas operações de estoque. Uma `Transferencia` manipula várias tabelas e exige bloqueios mínimos de transação. O Dapper nos dá controle total sobre as queries (otimizadas e com SARGability completa).
2. **Estoque Dividido por Setor:** Em vez de ter um campo global `Produto.QuantidadeEstoque`, modelamos a tabela `EstoqueSetor` como o "caixa" (saldo) de cada departamento. Isso permite que um produto seja transferido de TI para Almoxarifado sem perda de rastreabilidade (gerando uma movimentação espelhada com o mesmo `TransacaoId`).
3. **Injeção de Dependência (SOLID):** Utilizamos o contêiner `Unity` para injetar instâncias de Repositórios e Serviços. Os controllers dependem apenas das *Interfaces* (`IProdutoServico`), facilitando mock em testes unitários. Outra aplicação é o uso exaustivo do `FluentValidation` com validators injetados na pilha.
4. **Resiliência (Graceful Degradation):** Filtros globais capturam Exceções de Domínio (ex: *Transação Abortada* lançando HTTP 409 em vez de 500) e o fluxo do `Identity` utiliza parsing seguro (`TryParse`) para ignorar tokens maliciosos, evitando que o servidor sofra crash.

---

## 📊 Exemplos de Requisições da API

Se você for testar o Backend diretamente (via CLI/Postman) em vez de usar o Frontend MVC, aqui estão os *Curl* de referência:

**1. Login (Autenticação)**
```bash
curl -X POST http://localhost:PORTA/api/usuario/login \
     -H "Content-Type: application/json" \
     -d '{"Cpf": "00000000000", "Senha": "admin123"}'
```
*-> Copie o `Token` retornado para usar no Header das próximas requests.*

**2. Listar Produtos (Com Saldo Global Calculado)**
```bash
curl -X GET http://localhost:PORTA/api/produto \
     -H "Authorization: Bearer <SEU_TOKEN_AQUI>"
```

**3. Registrar Entrada de Estoque (Soma ao setor especificado)**
```bash
curl -X POST http://localhost:PORTA/api/movimentacao/entrada \
     -H "Authorization: Bearer <SEU_TOKEN_AQUI>" \
     -H "Content-Type: application/json" \
     -d '{
           "ProdutoId": 1,
           "SetorId": 2,
           "Quantidade": 50,
           "UsuarioId": 1
         }'
```

---

**Desenvolvido por Rodrigo Leal de Lima**