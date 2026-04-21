# 📦 Sistema de Gestão de Estoque (MVP)

> **⚠️ Nota ao avaliador:** a aplicação estava funcionando corretamente na data de entrega do desafio **(17/04/2026)**. No entanto, por não ter sido especificada a versão do axios na URL do CDN, a biblioteca foi atualizada automaticamente pelo jsDelivr em **19/04/2026**, invalidando o hash de integridade (SRI) configurado e derrubando o login da aplicação. O problema foi identificado e corrigido em **21/04/2026** com a simples adição de `@1.15.0` à URL do CDN, versão que estava vigente na data da entrega. A versão foi fixada para que o MVP possa ser avaliado corretamente.

Solução Full Stack desenvolvida em **ASP.NET 4.8**

A arquitetura foi desenhada com foco em **separação de responsabilidades, integridade de dados e alta performance**, utilizando Dapper, JWT e cobertura de testes unitários.

## 🌐 Deploy em Produção (Live Demo)
Para facilitar a avaliação, o sistema foi publicado na nuvem (Azure). Sinta-se à vontade para testar a aplicação em tempo real:

| Recurso | Acesso |
|---|---|
| **Frontend (Web)** | [https://estoquemvp-web.azurewebsites.net](https://estoquemvp-web.azurewebsites.net) |
| **API (Backend)** | [https://estoquemvp-api.azurewebsites.net](https://estoquemvp-api.azurewebsites.net) |
| **Documentação API (Swagger)** | [https://estoquemvp-api.azurewebsites.net/swagger](https://estoquemvp-api.azurewebsites.net/swagger) |
| **Banco de Dados** | Azure SQL Database |

### 🔑 Credenciais para Avaliação
- **CPF:** `000.000.000-00`
- **Senha:** `admin123`

---

## 🚀 Como Executar Localmente (Passo a Passo)

Caso deseje rodar a aplicação em seu ambiente local, siga as instruções abaixo:

### 1. Pré-requisitos
- Visual Studio 2022 (ou superior)
- SQL Server (LocalDB, Express ou instância completa)
- .NET Framework 4.8 SDK

### 2. Configuração do Banco de Dados
1. Na **raiz deste repositório**, localize o script `Criar_EstoqueMvp.sql`.
2. Execute-o em seu SQL Server (via SSMS ou Azure Data Studio).
3. O script criará o banco `EstoqueMvp`, todas as tabelas com seus respectivos relacionamentos (FKs) e inserirá a carga inicial de dados (Setores, Produtos, Tipos de Movimentação e o Usuário Admin padrão).

### 3. Configuração da Connection String
No projeto **`EstoqueMvp.Api`**, abra o arquivo `Web.config` e ajuste a string de conexão para apontar para o seu servidor SQL local:

```xml
<connectionStrings>
  <add name="EstoqueConexao"
       connectionString="Data Source=SEU_SERVIDOR;Initial Catalog=EstoqueMvp;Integrated Security=True"
       providerName="System.Data.SqlClient" />
</connectionStrings>
```

### 4. Executando a Aplicação
1. Abra a solution `EstoqueMvp.slnx` no Visual Studio.
2. Configure **Múltiplos Projetos de Inicialização**: defina tanto `EstoqueMvp.Api` quanto `EstoqueMvp.Web` com a ação **Start**.
3. Pressione **F5**. O navegador abrirá automaticamente na tela de login.

*(Nota sobre Segurança: A chave secreta do JWT (`JwtSecretKey`) está configurada no `Web.config` apenas para facilitar a execução local do avaliador. Em um cenário real de produção, essa chave residiria em um cofre de chaves ou variáveis de ambiente).*

---

## 🏗️ Arquitetura e Decisões Técnicas (ADRs)

A solução utiliza uma arquitetura em 5 camadas (N-Tier), respeitando o **Dependency Inversion Principle** (a API e os Serviços dependem de abstrações do Domínio, não de implementações concretas).

* **Dominio:** Entidades, Interfaces e Enums (zero dependências).
* **Dados:** Repositórios implementados com **Dapper** (Micro-ORM) para máxima performance nas queries e controle transacional preciso.
* **Servicos:** Regras de negócio, mapeamentos e validações com **FluentValidation**.
* **API:** Controllers enxutos, orquestração HTTP, Swagger e Middleware JWT via OWIN.
* **Web (Front):** Razor Views com Bootstrap 5.3 e chamadas assíncronas via Axios/JS modular.

### 🛡️ Critérios de Avaliação Atendidos

| Critério / Requisito | Como foi resolvido |
|---|---|
| **Controle de Estoque & Regras** | Validação server-side estrita. É impossível realizar Consumo ou Transferência com saldo insuficiente (Gera HTTP 400 Bad Request). |
| **Rastreabilidade** | Toda ação gera um registro imutável (append-only) na tabela de `MovimentacaoEstoque`, vinculando data, tipo, setor de origem/destino e o usuário responsável. |
A transferência debita de um setor e credita em outro dentro de um `TransactionScope`.
| **Segurança e SQL Injection** | 100% das queries utilizam parâmetros seguros via Dapper. Senhas protegidas com hash **BCrypt** e endpoints protegidos por token JWT. |
| **Padrões SOLID** | Injeção de dependência via **Unity Container**. Cada serviço possui uma única responsabilidade (SRP). OCP garantido no design dos enumeradores de movimentação. |
| **Geração de SKU** | Automatizada na camada de serviço, seguindo o padrão `PRD-{GUID-HEX}`. |
| **Tratamento de Exceções** | Implementação de um `GlobalExceptionFilter` na API que traduz exceções de domínio em respostas HTTP amigáveis, evitando exposição de stack trace. |

---

## 🧪 Qualidade de Software

O projeto conta com uma suíte de **98 Testes Unitários** (MSTest + Moq), cobrindo:
* Validação complexa do algoritmo de CPF (Server-side).
* Regras de negócio de movimentação (tentativas de saída com estoque zerado/negativo).
* Validações de DTOs e Comportamento dos Controllers.

---

## 📚 Stack Tecnológica Resumida
* **Backend:** C# / ASP.NET 4.8 Web API 2
* **Banco de Dados:** SQL Server / Azure SQL
* **ORM:** Dapper
* **Frontend:** ASP.NET MVC (Razor), Vanilla JS, Bootstrap 5.3, Chart.js
* **Bibliotecas:** FluentValidation, BCrypt.Net, Swashbuckle (Swagger), Unity (IoC), System.IdentityModel.Tokens.Jwt