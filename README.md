# BankingApi

A learning-focused ASP.NET Core Web API for a banking domain. The solution uses a layered CQRS structure with Entity Framework Core and SQLite planned for persistence.

## Status

The solution structure and project references are in place. The API currently runs as the default ASP.NET Core controller template; customer, account, transaction, CQRS, validation, and SQLite features are the next implementation steps.

## Architecture

```text
src/
  BankingApi.API/             HTTP controllers, Program.cs, application configuration
  BankingApi.AppService/      Dependency-injection registrations
  BankingApi.Command/         Commands, command handlers, and validators
  BankingApi.DTO/             Request DTOs and response models
  BankingApi.Infrastructure/  EF Core entities, DbContext, repositories, SQLite
  BankingApi.Query/           Queries and query handlers
  BankingApi.Shared/          CQRS contracts and custom ISender implementation
```

Planned dependency direction:

```text
API -> AppService -> Command/Query -> Shared
API -> Infrastructure -> Shared
Command/Query -> Infrastructure and DTO
Infrastructure -> DTO and Shared
```

## Banking Domain

The API will manage these resources:

```text
Customer 1 ---- many Account
Account  1 ---- many Transaction
```

- `Customer`: customer profile and contact information.
- `Account`: account number, account type, balance, and owner.
- `Transaction`: immutable ledger record for deposits, withdrawals, and transfers.

Money values should use `decimal`. Account balances must change only through explicit commands, and a transfer must update both accounts and write debit and credit ledger entries atomically.

## Prerequisites

- .NET 10 SDK or later
- EF Core command-line tool, when migrations are added:

```powershell
dotnet tool install --global dotnet-ef
```

## Run Locally

From the repository root:

```powershell
dotnet restore
dotnet build BankingApi.slnx
dotnet run --project src/BankingApi.API/BankingApi.API.csproj
```

The API uses the development launch profile by default. Once running, its OpenAPI document is available in the Development environment at `/openapi/v1.json`.

To use fixed local ports:

```powershell
dotnet run --project src/BankingApi.API/BankingApi.API.csproj --urls "http://localhost:5185;https://localhost:7164"
```

## Planned Endpoints

```text
GET    /api/customers
GET    /api/customers/{id}
POST   /api/customers
PUT    /api/customers/{id}
DELETE /api/customers/{id}

GET    /api/accounts
GET    /api/accounts/{id}
POST   /api/accounts
PUT    /api/accounts/{id}
DELETE /api/accounts/{id}

GET    /api/transactions
GET    /api/transactions/account/{accountId}
POST   /api/transactions/deposit
POST   /api/transactions/withdraw
POST   /api/transactions/transfer
```

Transactions should not expose update or delete endpoints.

## Database Plan

The project will use EF Core with the SQLite provider. The intended local connection string is:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=banking.db"
  }
}
```

After the DbContext and entities are implemented, create and apply the first migration:

```powershell
dotnet ef migrations add InitialCreate `
  --project src/BankingApi.Infrastructure/BankingApi.Infrastructure.csproj `
  --startup-project src/BankingApi.API/BankingApi.API.csproj `
  --output-dir Migrations

dotnet ef database update `
  --project src/BankingApi.Infrastructure/BankingApi.Infrastructure.csproj `
  --startup-project src/BankingApi.API/BankingApi.API.csproj
```

Local SQLite database files are ignored by Git.
