# 📦 Sistema de Gestão de Estoque (MVP)

Solução Full Stack desenvolvida em **ASP.NET 4.8** para gerenciamento de inventário, movimentações entre setores e controle de acesso. Arquitetura em camadas com separação de responsabilidades, Dapper, JWT e 98 testes unitários.

## 🌐 Deploy em Produção (Azure)

| Recurso | URL |
|---|---|
| **Frontend (Web)** | [https://estoquemvp-web.azurewebsites.net](https://estoquemvp-web.azurewebsites.net) |
| **API (Backend)** | [https://estoquemvp-api.azurewebsites.net](https://estoquemvp-api.azurewebsites.net) |
| **Swagger (Documentação API)** | [https://estoquemvp-api.azurewebsites.net/swagger](https://estoquemvp-api.azurewebsites.net/swagger) |
| **Banco de Dados** | Azure SQL Server (`sqlsrv-estoquemvp2.database.windows.net`) |

> Para testar diretamente, acesse o **Frontend** e faça login com as credenciais abaixo.

---

## 🔑 Credenciais de Teste

| Campo | Valor |
|---|---|
| CPF | `000.000.000-00` |
| Senha | `admin123` |

---

## 🏗️ Arquitetura

```
EstoqueMvp.slnx
├── Dominio/          → Entidades, Interfaces, Enums (zero dependências)
├── Dados/            → Repositórios Dapper (SQL Server)
├── Servicos/         → Regras de negócio, DTOs, Validators, Mapeamentos, Exceptions
├── EstoqueMvp.Api/   → Web API 2 (Controllers, JWT, Swagger, Middleware, Filtros)
├── EstoqueMvp.Web/   → Frontend MVC (Razor Views, Bootstrap 5.3, JS modular)
└── EstoqueMvp.Testes/→ Testes unitários (MSTest + Moq — 98 testes)
```

**Fluxo de dependência:** `Api → Servicos → Dominio ← Dados` (Dependency Inversion)

### Tecnologias

| Camada | Tecnologia |
|---|---|
| Backend | ASP.NET Web API 2 (.NET Framework 4.8) |
| Persistência | **Dapper** (Micro-ORM) + SQL Server |
| Autenticação | **JWT** via OWIN + cookie httpOnly + **BCrypt** |
| Validação | **FluentValidation** (10 validators) |
| IoC | **Unity Container** |
| Documentação | **Swagger** (Swashbuckle) |
| Frontend | **Razor Views** + Bootstrap 5.3 + Vanilla JS modular |
| Testes | **MSTest** + **Moq** (98 testes — serviços, validações e controllers) |
| Hospedagem | **Azure App Service** (Web + API) + **Azure SQL Database** |

---

## 🚀 Configuração e Execução Local

### 1. Pré-requisitos
- **Visual Studio 2022+** (ou 2026)
- **SQL Server** (LocalDB, Express ou instância completa)
- **.NET Framework 4.8** SDK

### 2. Banco de Dados
1. Localize o arquivo **`Criar_EstoqueMvp.sql`** na raiz do repositório.
2. Execute-o no SQL Server Management Studio (SSMS) ou Azure Data Studio.
3. O script cria o banco `EstoqueMvp`, todas as tabelas (com PKs, FKs, CHECKs, índices) e insere dados iniciais:
   - 4 Tipos de Movimentação (Entrada, Consumo, Envio, Recebimento)
   - 3 Setores (Almoxarifado Central, Setor de TI, Setor de RH)
   - 5 Produtos com SKU (Mouse, Teclado, Resma, Cabo HDMI, Fone)
   - 1 Usuário Admin (CPF: 00000000000 / Senha: admin123)
   - Estoque inicial distribuído nos setores
   - Histórico de 19 movimentações (entradas, transferências, consumos)

### 3. Connection String
No projeto **`EstoqueMvp.Api`**, edite o `Web.config`:

```xml
<connectionStrings>
  <add name="EstoqueConexao"
       connectionString="Data Source=SEU_SERVIDOR;Initial Catalog=EstoqueMvp;Integrated Security=True"
       providerName="System.Data.SqlClient" />
</connectionStrings>
```

### 4. URL da API no Frontend
No projeto **`EstoqueMvp.Web`**, edite o `Web.config`:

```xml
<appSettings>
  <add key="ApiBaseUrl" value="https://localhost:44333/api" />
</appSettings>
```

> Ajuste a porta conforme a configuração do IIS Express (veja `Properties/launchSettings.json` ou as propriedades do projeto).

### 5. Execução
1. Abra `EstoqueMvp.slnx` no Visual Studio.
2. Configure **múltiplos projetos de inicialização**: `EstoqueMvp.Api` + `EstoqueMvp.Web` → **Start**.
3. Pressione **F5**.
4. O navegador abrirá a tela de login.

> ⚠️ **JWT Secret Key:** A chave `JwtSecretKey` está no `Web.config` para facilitar execução local. Em produção, utilizar variáveis de ambiente ou cofre de segredos.

---

## ✅ Funcionalidades Implementadas

### Requisitos do Desafio

| Requisito | Status | Implementação |
|---|---|---|
| CRUD de Produtos (nome, descrição, preço, estoque) | ✅ | Cadastro, edição, soft delete, restauração |
| Geração de SKU único | ✅ | Formato `PRD-{12 hex}` via GUID, imutável |
| CRUD de Usuários | ✅ | Cadastro, edição, soft delete, restauração |
| Login com CPF | ✅ | JWT em cookie httpOnly + BCrypt |
| Validação de CPF (server-side) | ✅ | Algoritmo completo (11 dígitos verificadores) |
| Entrada de Produtos | ✅ | Credita no setor + registra movimentação |
| Consumo de Produtos | ✅ | Debita com validação de saldo ≥ quantidade |
| Transferência entre Setores | ✅ | TransactionScope atômico, TransacaoId compartilhado |
| Impedir estoque negativo | ✅ | Validação server-side antes de debitar |
| Registro de movimentações | ✅ | Tabela append-only com tipo, data, usuário, transação |
| Script SQL de criação | ✅ | `Criar_EstoqueMvp.sql` na raiz |
| Separação de responsabilidades | ✅ | 5 camadas (Domínio, Dados, Serviços, API, Web) |
| SQL com FKs e tipos adequados | ✅ | PKs, FKs, CHECKs, índices, IDENTITY |
| Proteção contra SQL Injection | ✅ | 100% queries parametrizadas (Dapper) |
| Tratamento de exceções | ✅ | GlobalExceptionFilter centralizado |
| Padrões SOLID | ✅ | DIP, SRP, OCP, ISP, LSP |
| Repositório público no GitHub | ✅ | [github.com/LealdeLimaRodrigo/EstoqueMvp](https://github.com/LealdeLimaRodrigo/EstoqueMvp) |
| README com passo a passo | ✅ | Este documento |

### Funcionalidades Extras (Além do Desafio)

| Funcionalidade | Descrição |
|---|---|
| **CRUD de Setores** | Cadastro, edição, soft delete + restauração |
| **Dashboard** | KPIs animados, gráfico de 7 dias (Chart.js), últimas movimentações |
| **Paginação Server-Side** | `OFFSET/FETCH` com busca paginada |
| **Busca** | Filtro por nome/SKU com paginação |
| **Exportação CSV** | Movimentações com filtros aplicados |
| **Filtros de Movimentação** | Por tipo (botões), por data (intervalo), por texto |
| **Modal de Detalhes** | Botão 👁 para ver detalhes de qualquer registro |
| **Restauração Inteligente** | Ao cadastrar com nome/CPF duplicado inativo, pergunta se quer restaurar |
| **Indicadores de Inativos** | Nome em vermelho + ícone 🚫 em movimentações com registros inativos |
| **Dark Mode** | Toggle com persistência via localStorage |
| **Acessibilidade** | WCAG 2.1 AA (skip-link, aria-live, reduced-motion, keyboard nav) |
| **98 Testes Unitários** | Cobertura de serviços, validações e controllers |
| **Swagger** | Documentação interativa da API |
| **Deploy Azure** | App Service (Web + API) + Azure SQL Database |

---

## 🧪 Testes

O projeto `EstoqueMvp.Testes` contém **98 testes unitários**:

| Categoria | Testes |
|---|---|
| **Serviços** | Produto, Usuário, Setor, EstoqueSetor, MovimentacaoEstoque |
| **Validações** | CPF (algoritmo), Produto, Usuário, Movimentação, Transferência |
| **Controllers** | Produto, Usuário, MovimentacaoEstoque |

**Executar:** Abra o **Test Explorer** no Visual Studio e clique em **Run All**.

---

---

## 📊 API — Exemplos (Swagger: `/swagger`)

**Login:**
```bash
curl -X POST https://estoquemvp-api.azurewebsites.net/api/usuario/login \
     -H "Content-Type: application/json" \
     -d '{"Cpf": "00000000000", "Senha": "admin123"}'
```

**Listar Produtos:**
```bash
curl -X GET https://estoquemvp-api.azurewebsites.net/api/produto \
     -H "Authorization: Bearer <TOKEN>"
```

**Entrada de Estoque:**
```bash
curl -X POST https://estoquemvp-api.azurewebsites.net/api/movimentacao-estoque/entrada \
     -H "Authorization: Bearer <TOKEN>" \
     -H "Content-Type: application/json" \
     -d '{"ProdutoId": 1, "SetorId": 1, "Quantidade": 50}'
```

---

## 🏛️ Princípios SOLID Aplicados

| Princípio | Aplicação |
|---|---|
| **S — Single Responsibility** | Cada classe tem uma responsabilidade: Repositórios acessam dados, Serviços aplicam regras, Controllers orquestram HTTP, Validators validam DTOs |
| **O — Open/Closed** | Novos tipos de movimentação podem ser adicionados sem alterar o `MovimentacaoEstoqueServico` (enum + tabela); Validators extensíveis via FluentValidation |
| **L — Liskov Substitution** | Todas as implementações de repositório são substituíveis via interface (ex: trocar `ProdutoRepositorio` por mock nos testes) |
| **I — Interface Segregation** | Interfaces específicas por entidade (`IProdutoRepositorio`, `ISetorRepositorio`) em vez de um único repositório genérico |
| **D — Dependency Inversion** | Interfaces definidas no `Dominio`, implementadas em `Dados`; Serviços dependem de abstrações; Unity Container faz a injeção |

---

**Desenvolvido por Rodrigo Leal de Lima**