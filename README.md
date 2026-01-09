# Finance API

### 📌 Descrição

A **Finance API** é uma aplicação robusta de Back-end para controle de transações financeiras. Desenvolvida com **.NET 8**, ela segue os princípios da **Clean Architecture** para garantir desacoplamento, testabilidade e manutenção.

O projeto foi modernizado para uma abordagem **API-First**, removendo dependências de front-end acopladas e focando em performance, containerização (**Docker**) e observabilidade avançada (**Serilog + Seq**).

---

### 🚀 Funcionalidades

- **Gestão Financeira:** CRUD completo de Categorias e Transações (Receitas/Despesas).
- **Autenticação Segura:** Implementação de Identity e JWT (JSON Web Tokens).
- **Observabilidade:** Logs estruturados centralizados com Serilog e dashboard no Seq.
- **Containerização:** Ambiente de desenvolvimento completo orquestrado via Docker Compose.
- **Documentação:** Swagger/OpenAPI auto-gerado.
- **Testes:** Testes de Integração e Unidade com xUnit.

---

### 🛠️ Tecnologias

**Core:**
- .NET 8 SDK
- ASP.NET Core Web API
- Entity Framework Core

**Infraestrutura & DevOps:**
- **Docker & Docker Compose:** Orquestração de containers.
- **SQL Server 2022:** Banco de dados relacional (Container).
- **Seq:** Servidor de logs estruturados (Container).
- **Serilog:** Biblioteca de logging.

**Arquitetura:**
- Clean Architecture (Domain, Application, Infrastructure, Contracts, API)
- Pattern: Repository & Handler (Mediator style)

---

### 📁 Estrutura de Diretórios do Projeto

```
Finance/
├── Finance.sln
│
├── Finance.Api/                        # Camada de apresentação (API)
│   ├── Controllers/
│   │   ├── AuthController.cs
│   │   ├── CategoriesController.cs
│   │   └── TransactionController.cs
│   ├── Properties/
│   │   ├── launchSettings.json
│   │   └── serviceDependencies.json
│   ├── Extensions/
│   │   ├── ActionResultExtension.cs
│   │   └── BuilderExtension.cs
│   ├── ApiConfiguration.cs
│   ├── appsettings.json
│   ├── DockerFile
│   ├── Program.cs
│   └── Finance.API.csproj
│
├── Finance.Application/                # Camada de aplicação (handlers e claim do JWT)
│   ├── Extensions/
│   │   └── ClaimsPrincipalExtension.cs
│   ├── Handlers/
│   │   ├── CategoryHandler.cs
│   │   ├── TransactionHandler.cs
│   │   └── UserHandler.cs
│   └── Finance.Application.csproj
│
├── Finance.Contracts/                     # Camada de compartilhamento (interfaces, requests, responses)
│   ├── Requests/
│   │   ├── Auth/
│   │   │   ├── LoginRequest.cs
│   │   │   ├── RegisterRequest.cs
│   │   ├── └── UpdateUserProfileRequest.cs
│   │   ├── Categories/
│   │   │   ├── CreateCategoryRequest.cs
│   │   │   ├── DeleteCategoryRequest.cs
│   │   │   ├── GetAllCategoriesRequest.cs
│   │   │   ├── GetCategoryByIdRequest.cs
│   │   ├── └── UpdateCategoryRequest.cs
│   │   ├── Transacations/
│   │   │   ├── CreateTransactionRequest.cs
│   │   │   ├── DeleteTransactionRequest.cs
│   │   │   ├── GetTransactionByIdRequest.cs
│   │   │   ├── GetTransactionReportRequest.cs
│   │   │   ├── GetTransactionByPeriodRequest.cs
│   │   ├── └── UpdateTransactionRequest.cs
│   │   ├── PagedRequest.cs
│   │   └── Request.cs
│   ├── Responses/
│   │   ├── Auth/
│   │   ├── └── UserProfileResponse.cs
│   │   ├── Categories/
│   │   ├── └── CategorySummaryResponse.cs
│   │   ├── Transacations/
│   │   ├── └── TransactionReportResponse.cs
│   │   ├── PagedResponse.cs
│   │   └── Response.cs
│   ├── Interfaces/
│   │   ├── Handlers/
│   │   │   ├── IAppPreferencesHandler.cs
│   │   │   ├── ICategoryHandler.cs
│   │   │   ├── ITransactionHandler.cs
│   │   ├── └── IUserHandler.cs
│   │   ├── Repositories/
│   │   │   ├── ICategoryRepository.cs
│   │   │   ├── ITransactionRepository.cs
│   ├── └── └── IUserRepository.cs
│   └── Finance.Contracts.csproj
│
├── Finance.Domain/                     # Camada de domínio (entidades e contratos)
│   ├── Common/
│   │   └── DateTimeExtension.cs
│   ├── Enums/
│   │   └── ETransactionType.cs
│   ├── Models/
│   │   │   ├── DTOs/
│   │   │   │   ├── CategoryDto.cs
│   │   │   └── └── TransactionDto.cs
│   │   ├── Category.cs
│   │   ├── Transaction.cs
│   │   └── User.cs
│   └── Finance.Domain.csproj
│
├── Finance.Infrastructure/             # Camada de infraestrutura (banco de dados e repositórios)
│   ├── Data/
│   │   ├──  Mappings/
│   │   │    ├── CategoryMapping.cs
│   │   │    ├── TransactionMapping.cs
│   │   └──  └── UserMapping.cs
│   └── FinanceDbContext.cs
│   ├── Migrations/
│   │   ├── InitialCreate.cs
│   │   └── FinanceDbContextModelSnapshot.cs
│   ├── Repositories/
│   │   ├── CategoryRepository.cs
│   │   ├── TransactionRepository.cs
│   │   └── UserRepository.cs
└── └── Finance.Infrastructure.csproj
```

---

### 🐳 Como executar (Modo Docker - Recomendado)

Este é o método mais rápido e limpo, pois sobe o Banco, a API e o Seq automaticamente.

**Pré-requisitos:**
- [Docker Desktop](https://www.docker.com/products/docker-desktop/) instalado.

#### 1. Clone o repositório
```bash
git clone https://github.com/alysonsz/Finance-API.git
cd Finance-API
```

#### 2. Suba o ambiente
Na raiz do projeto (onde está o `docker-compose.yml`), execute:

```bash
docker-compose up -d --build
```

#### 3. Acesse os serviços

| Serviço | URL / Endereço | Credenciais (se houver) |
| :--- | :--- | :--- |
| **API (Swagger)** | [http://localhost:5000/swagger](http://localhost:5000/swagger) | - |
| **Seq (Logs)** | [http://localhost:5341](http://localhost:5341) | User: `admin` <br> Pass: `MyStrongPassword123!` |
| **SQL Server** | `localhost,1433` | User: `sa` <br> Pass: `MyStrongPassword123!` |

---

### 💻 Como executar (Modo Manual / Debug)

Caso queira rodar a API fora do Docker (pelo Visual Studio ou CLI), mas mantendo as dependências (Banco/Seq) no Docker.

1. **Suba apenas a infraestrutura:**
   ```bash
   docker-compose up -d finance-db finance-seq
   ```

2. **Aplique as Migrations (apenas na primeira vez):**
   ```bash
   dotnet ef database update --project Finance.Infrastructure --startup-project Finance.Api
   ```

2. **Rode a API:**
   ```bash
   dotnet run --project Finance.Api
   ```

### 🧪 Testes

O projeto utiliza xUnit para testes automatizados. Para executá-los:
   ```bash
   dotnet test   
   ```

---

### 👨‍💻 Autor

- Alyson Souza Carregosa • .NET Backend Developer

---

### 📝 Licença

Este projeto está disponível sob a licença MIT.
