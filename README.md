# Finance API

### 📌 Descrição do Projeto

A **Finance API** é uma aplicação RESTful desenvolvida em .NET 8 com foco no gerenciamento de transações financeiras e categorias. O sistema é estruturado seguindo os princípios da Arquitetura Limpa (Clean Architecture), promovendo organização, manutenibilidade e escalabilidade.

---

### 🚀 Objetivos do Projeto

- Utilizar **Clean Architecture** com separação clara de responsabilidades.
- Permitir o registro de **transações financeiras** com data, valor e tipo.
- Oferecer controle de **categorias personalizadas** para organização das finanças.
- Persistir dados com **Entity Framework Core** e banco de dados **SQL Server**.
- Expor endpoints RESTful com documentação via **Swagger**.

---

### 🛠️ Tecnologias Utilizadas

**Backend:**
- .NET 8
- ASP.NET Core Web API
- Entity Framework Core
- SQL Server

**Padrões e Ferramentas:**
- Clean Architecture
- Injeção de Dependência (DI)
- AutoMapper
- Swagger / OpenAPI

---

### 🔄 Funcionalidades Principais

- **Gerenciamento de Categorias:**
  - Cadastro, listagem e exclusão de categorias
- **Gerenciamento de Transações:**
  - Registro de receitas e despesas
  - Associação de transações com categorias
  - Filtro por tipo e data

---

### 📁 Estrutura de Diretórios do Projeto

```
Finance/
├── Finance.sln
│
├── Finance.API/                         # Camada de apresentação (Web API)
│   ├── Controllers/
│   │   ├── CategoryController.cs
│   │   └── TransactionController.cs
│   ├── Properties/
│   │   ├── launchSettings.json
│   │   └── serviceDependencies.json
│   ├── Extensions/
│   │   └── BuilderExtension.cs
│   ├── ApiConfiguration.cs
│   ├── appsettings.json
│   ├── Program.cs
│   └── Finance.API.csproj
│
├── Finance.Application/                # Camada de aplicação (handlers e interfaces)
│   ├── Handlers/
│   │   ├── CategoryHandler.cs
│   │   └── TransactionHandler.cs
│   ├── Requests/
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
│   │   │   ├── GetTransactionByPeriodRequest.cs
│   │   ├── └── UpdateTransactionRequest.cs
│   │   ├── PagedRequest.cs
│   │   └── Request.cs
│   ├── Responses/
│   │   ├── PagedResponse.cs
│   │   └── Response.cs
│   ├── Interfaces/
│   │   ├── Handlers/
│   │   │   ├── ICategoryHandler.cs
│   │   ├── └── ITransactionHandler.cs
│   │   ├── Repositories/
│   │   │   ├── ICategoryRepository.cs
│   └── └── └── ITransactionRepository.cs
│   └── Finance.Application.csproj
│
├── Finance.Domain/                     # Camada de domínio (entidades e contratos)
│   ├── Common/
│   │   └── DateTimeExtension.cs
│   ├── Enums/
│   │   └── ETransactionType.cs
│   ├── Models/
│   │   ├── Category.cs
│   │   └── Transaction.cs
│   └── Finance.Domain.csproj
│
├── Finance.Infrastructure/             # Camada de infraestrutura (banco de dados e repositórios)
│   ├── Data/
│   │   ├──  Mappings/
│   │   │    ├── CategoryMapping.cs
│   │   └──  └── TransactionMapping.cs
│   └── FinanceDbContext.cs
│   ├── Migrations/
│   │   ├── InitialCreate.cs
│   │   └── FinanceDbContextModelSnapshot.cs
│   ├── Repositories/
│   │   ├── CategoryRepository.cs
│   │   └── TransactionRepository.cs
│   └── Finance.Infrastructure.csproj
│
├── Finance.Web/                         # Camada Web (Frontend Razor Pages)
│   ├── Handles/
│   │   ├── CategoryHandler.cs
│   │   └── TransactionHandler.cs
│   ├── Layout/
│   │   ├── App.razor
│   │   ├── MainLayout.razor
│   │   └── NavMenu.razor
│   ├── Pages/
│   │   ├── Categories/
│   │   │   └── GetAllCategories.razor
│   │   ├── Transactions/
│   │   │   └── GetAllTransactions.razor
│   │   └── Home.razor
│   ├── wwwroot/
│   │   └── css/
│   │       └── app.css
│   ├── WebConfiguration.cs
│   ├── Program.cs
│   ├── _Imports.razor
└── └── Finance.Web.csproj
```

---

### ⚙️ Como Rodar o Projeto

1. Clone o repositório:
   ```
   git clone https://github.com/alysonsz/Finance-API.git
   ```

2. Acesse a pasta raiz do projeto:
   ```
   cd Finance-API
   ```

3. Verifique a `ConnectionString` no arquivo `Finance.API/appsettings.json`.

4. Aplique as migrations e atualize o banco:
   ```
   dotnet ef database update --project Finance.Infrastructure --startup-project Finance.API
   ```

5. Execute a aplicação:
   ```
   dotnet run --project Finance.API
   ```

6. Acesse a documentação Swagger:
   ```
   https://localhost:{porta}/swagger
   ```

---

### 📌 Próximos Passos

- Implementar autenticação de usuários (JWT)
- Adicionar testes unitários
- Implementar filtros de busca por período, valor e categoria
- Configurar CI/CD com GitHub Actions
- Adicionar controle de saldo por usuário

---

### 👨‍💻 Autor

- Alyson Souza Carregosa 👨‍💻 Back-end Developer

---

### 📝 Licença

Este projeto está disponível sob a licença MIT.
