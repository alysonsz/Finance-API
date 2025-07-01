# Finance API

### 📌 Descrição

A **Finance API** é uma aplicação completa para controle de transações financeiras, desenvolvida com .NET 8 e Blazor WebAssembly, estruturada com Clean Architecture para promover organização, reutilização e clareza entre suas camadas de domínio, aplicação, infraestrutura, API e interface web.

---

### 🚀 Funcionalidades

- Cadastro e gerenciamento de categorias
- Registro de receitas e despesas
- Autenticação via JWT
- Interface web com Blazor integrada
- Documentação automática via Swagger
- Testes automatizados

---

### 🛠️ Tecnologias

- Backend: .NET 8, ASP.NET Core, Entity Framework Core  
- Frontend: Blazor WebAssembly  
- Arquitetura: Clean Architecture (Domain, Application, Infrastructure, API, Web)  
- Banco de dados: SQL Server LocalDB (via EF Core Migrations)  
- Ferramentas: AutoMapper, Swagger/OpenAPI  
- Testes: XUnit

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
│   └── Finance.Infrastructure.csproj
│
├── Finance.Web/                         # Camada Web (Frontend Blazor Pages)
│   ├── Authentication/
│   │   └── CustomAuthenticationStateProvider.cs
│   ├── Handlers/
│   │   ├── AppPreferencesHandler.cs
│   │   ├── AppThemeHandler.cs
│   │   ├── AuthHandler.cs
│   │   ├── AuthMessageHandler.cs
│   │   ├── CategoryHandler.cs
│   │   └── TransactionHandler.cs
│   ├── Layout/
│   │   ├── LoginLayout.razor
│   │   └── MainLayout.razor
│   ├── Pages/
│   │   ├── Categories/
│   │   │   ├── CreateCategory.razor
│   │   │   ├── EditCategory.razor
│   │   │   └── GetAllCategories.razor
│   │   │        └── GetAllCategories.razor.cs
│   │   ├── Transactions/
│   │   │   ├── CreateTransaction.razor
│   │   │   ├── EditTransaction.razor
│   │   │   └── GetAllTransactions.razor
│   │   │        └── GetAllTransactions.razor.cs
│   │   ├── About.razor
│   │   ├── Home.razor
│   │   ├── Login.razor
│   │   ├── RedirectToLogin.razor
│   │   ├── Register.razor
│   │   ├── Reports.razor
│   │   └── Settings.razor
│   ├── Shared/
│   │   ├── CategoryForm.razor
│   │   └── TransactionForm.razor
│   ├── wwwroot/
│   │   └── css/
│   │       └── app.css
│   ├── WebConfiguration.cs
│   ├── Program.cs
│   ├── App.razor
│   ├── _Imports.razor
└── └── Finance.Web.csproj
```

---

### ✅ Como executar o projeto

#### 1. Clone o repositório

```bash
git clone https://github.com/alysonsz/Finance-API.git
cd Finance-API
```

#### 2. Restaure os pacotes

```bash
dotnet restore
```

#### 3. Crie o banco de dados

```bash
dotnet ef database update --project Finance.Infrastructure --startup-project Finance.Api
```

#### 4. Execute a API + Front-end juntos

**Escolha conforme seu sistema operacional:**

- 🪟 **Windows**  
  Execute o arquivo `start.bat` (clique duas vezes ou rode no terminal):

  ```bash
  start.bat
  ```

- 🐧 **Linux / macOS / WSL**  
  Dê permissão e execute o script:

  ```bash
  chmod +x start.sh
  ./start.sh
  ```

> Isso iniciará automaticamente a API e o front-end Blazor WebAssembly.

---

### 🔗 Endpoints úteis

- API: [https://localhost:7279/swagger](https://localhost:7279/swagger)
- Frontend (Blazor): aberto automaticamente ao executar o projeto

---

### 👨‍💻 Autor

- Alyson Souza Carregosa • Back-end Developer

---

### 📝 Licença

Este projeto está disponível sob a licença MIT.
