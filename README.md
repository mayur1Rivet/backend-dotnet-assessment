# BankingApi

Learning-focused ASP.NET Core Web API for customer and bank account management. The project uses .NET 10, layered CQRS-style handlers, Entity Framework Core, SQLite, FluentValidation, JWT authentication, and role-based authorization.

## Implemented Features

- Customer create, read, update, and delete operations.
- Account create, read, update, and delete operations.
- Customer-to-account ownership checks.
- Customer email uniqueness validation.
- Indian mobile phone validation.
- Shared validation rules for customer and account create/update requests.
- JWT registration and login.
- Password hashing with `PasswordHasher`.
- `Customer` and `Admin` roles.
- Customer ownership authorization.
- Admin-only collection and customer-management operations.
- Centralized JSON error responses.
- SQLite persistence through Entity Framework Core.
- Automatic migration application during application startup.
- Development admin seeding when admin settings are configured.
- Swagger/OpenAPI in the Development environment.

Transactions are not implemented yet.

## Technology Stack

- .NET 10
- ASP.NET Core Web API
- Entity Framework Core 10
- SQLite
- FluentValidation
- JWT Bearer authentication
- Swashbuckle / Swagger

## Project Structure

```text
src/
  BankingApi.API/             Controllers, middleware, Program.cs, configuration
  BankingApi.AppService/      Dependency injection and JWT token service
  BankingApi.Command/         Commands, handlers, and validators
  BankingApi.DTO/             Request and response models
  BankingApi.Infrastructure/  EF Core DbContext, entities, repositories, migrations
  BankingApi.Query/           Queries and query handlers
  BankingApi.Shared/          CQRS contracts and shared exceptions
```

Dependency direction:

```text
API -> AppService -> Command/Query -> Shared
API -> Infrastructure -> Shared
Command/Query -> Infrastructure and DTO
Infrastructure -> DTO and Shared
```

## Prerequisites

- .NET 10 SDK or later
- Optional: EF Core CLI

Install the EF CLI if needed:

```powershell
dotnet tool install --global dotnet-ef
```

## Configuration

The default SQLite database is configured in `src/BankingApi.API/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=banking.db"
  }
}
```

JWT settings are also configured there:

```json
{
  "Jwt": {
    "Issuer": "BankingApi",
    "Audience": "BankingApi.Client",
    "Key": "change-this-development-key-to-a-long-random-secret-1234567890",
    "ExpiryMinutes": 60
  }
}
```

For production, replace the development JWT key with a long, randomly generated secret and provide it through environment variables or a secret store.

The Development configuration optionally seeds an administrator:

```json
{
  "Admin": {
    "Email": "admin@banking.local",
    "Password": "Admin@12345"
  }
}
```

Do not use these development credentials in production.

## Start the Project

From the `BankingApi` directory:

```powershell
dotnet restore BankingApi.slnx
dotnet build BankingApi.slnx
dotnet run --project src/BankingApi.API/BankingApi.API.csproj
```

The default Development URLs are:

```text
http://localhost:5130
https://localhost:7169
```

To use custom URLs:

```powershell
dotnet run --project src/BankingApi.API/BankingApi.API.csproj --urls "http://localhost:5185;https://localhost:7164"
```

On startup, the application applies pending EF Core migrations and seeds the configured Development administrator if that email does not already exist.

Swagger is available in Development at:

```text
http://localhost:5130/swagger
```

## Authentication

All customer and account endpoints require a JWT bearer token. Register or log in first, then send the token on protected requests:

```http
Authorization: Bearer <access-token>
```

### Register

```http
POST /api/auth/register
Content-Type: application/json
```

Request:

```json
{
  "firstName": "Asha",
  "lastName": "Sharma",
  "email": "asha@example.com",
  "phoneNumber": "9876543210",
  "password": "StrongPass1"
}
```

New registrations receive the `Customer` role and are linked to a customer profile.

### Login

```http
POST /api/auth/login
Content-Type: application/json
```

Request:

```json
{
  "email": "asha@example.com",
  "password": "StrongPass1"
}
```

Response contains:

```json
{
  "accessToken": "<jwt>",
  "expiresAt": "2026-09-15T10:00:00Z",
  "userId": 1,
  "customerId": 1,
  "role": "Customer"
}
```

The JWT includes the user ID, customer ID, email, and role claims.

## Roles and Authorization

### Customer role

Customers can:

- Read their own customer profile.
- Update their own customer profile.
- Delete their own customer profile.
- Read their own account list.
- Read, create, update, and delete their own accounts.

Requests targeting another customer or another customer's account return HTTP `403 Forbidden`.

### Admin role

Admins can:

- List all customers.
- Create customers.
- Read, update, and delete customers.
- List all accounts.
- Read, create, update, and delete accounts.
- Access accounts by any customer ID.

The Development administrator is seeded from `Admin:Email` and `Admin:Password` configuration.

## API Endpoints

### Authentication

| Method | Route | Authentication | Description |
|---|---|---|---|
| `POST` | `/api/auth/register` | Anonymous | Register a customer and issue a JWT. |
| `POST` | `/api/auth/login` | Anonymous | Authenticate and issue a JWT. |

### Customers

| Method | Route | Role | Description |
|---|---|---|---|
| `GET` | `/api/customers` | Admin | List all customers. |
| `GET` | `/api/customers/{id}` | Admin or owner | Get one customer. |
| `POST` | `/api/customers` | Admin | Create a customer. |
| `PUT` | `/api/customers/{id}` | Admin or owner | Update a customer. |
| `DELETE` | `/api/customers/{id}` | Admin or owner | Delete a customer. |

### Accounts

| Method | Route | Role | Description |
|---|---|---|---|
| `GET` | `/api/accounts` | Admin | List all accounts. |
| `GET` | `/api/accounts/{id}` | Admin or owner | Get one account. |
| `GET` | `/api/accounts/customer/{customerId}` | Admin or owner | List accounts for one customer. |
| `POST` | `/api/accounts` | Admin or owner | Create an account for a customer. |
| `PUT` | `/api/accounts/{id}` | Admin or owner | Update account type or balance. |
| `DELETE` | `/api/accounts/{id}` | Admin or owner | Delete an account. |

## Validation Rules

### Customer validation

The shared customer rules are used by both customer create and update validators. They are also used during registration where applicable.

- `FirstName`: required, maximum 100 characters, letters plus spaces, apostrophes, and hyphens.
- `LastName`: required, maximum 100 characters, letters plus spaces, apostrophes, and hyphens.
- `Email`: required, valid email format, maximum 200 characters.
- `PhoneNumber`: optional; when supplied, exactly 10 digits and must start with `6`, `7`, `8`, or `9`.
- Customer email must be unique.

### Account validation

The shared account rules are used by both account create and update validators.

- `CustomerId`: required and greater than zero when creating an account.
- `AccountType`: required, maximum 50 characters.
- `Balance`: must be zero or greater.
- The referenced customer must exist before an account is created.
- Account numbers are generated by the server.

### Authentication validation

- Registration password is required and at least 8 characters.
- Registration password must contain an uppercase letter, lowercase letter, and digit.
- Login requires both email and password.
- Duplicate registration emails are rejected.

## Error Responses

Errors are returned as JSON only. The standard response contains:

```json
{
  "statusCode": 400,
  "errorCode": "VALIDATION_ERROR",
  "message": "One or more validation errors occurred.",
  "details": {
    "PhoneNumber": [
      "Phone number must be a valid Indian mobile number with exactly 10 digits."
    ]
  },
  "traceId": "00-example"
}
```

Common error codes:

| HTTP status | Error code | Meaning |
|---:|---|---|
| `400` | `VALIDATION_ERROR` | Request data failed validation. |
| `400` | `EMAIL_ALREADY_EXISTS` | Customer or user email is already registered. |
| `400` | `INVALID_ARGUMENT` | An argument is invalid. |
| `401` | `UNAUTHORIZED` | Authentication is required. |
| `401` | `INVALID_CREDENTIALS` | Login email or password is incorrect. |
| `403` | `FORBIDDEN` | Authenticated user cannot access the resource. |
| `404` | `CUSTOMER_NOT_FOUND` | Customer does not exist. |
| `404` | `ACCOUNT_NOT_FOUND` | Account does not exist. |
| `404` | `RESOURCE_NOT_FOUND` | A requested resource was not found. |
| `404` | `ROUTE_NOT_FOUND` | The route does not exist. |
| `405` | `METHOD_NOT_ALLOWED` | HTTP method is not supported for the route. |
| `500` | `INTERNAL_SERVER_ERROR` | Unexpected server error. |

## Database and Migrations

The SQLite database is `banking.db` in the API working directory. The current migration creates:

- `Customers`
- `Accounts`
- `Users`
- Foreign keys between accounts/users and customers.
- Unique customer email, user email, and account number indexes.

Migrations are applied automatically during startup. To inspect migrations:

```powershell
dotnet ef migrations list `
  --project src/BankingApi.Infrastructure/BankingApi.Infrastructure.csproj `
  --startup-project src/BankingApi.API/BankingApi.API.csproj
```

To apply migrations manually:

```powershell
dotnet ef database update `
  --project src/BankingApi.Infrastructure/BankingApi.Infrastructure.csproj `
  --startup-project src/BankingApi.API/BankingApi.API.csproj
```

To create a new migration after changing the model:

```powershell
dotnet ef migrations add MigrationName `
  --project src/BankingApi.Infrastructure/BankingApi.Infrastructure.csproj `
  --startup-project src/BankingApi.API/BankingApi.API.csproj `
  --output-dir Migrations
```

## Verification

Build the complete solution:

```powershell
dotnet restore BankingApi.slnx
dotnet build BankingApi.slnx --no-restore
```

Run the API:

```powershell
dotnet run --project src/BankingApi.API/BankingApi.API.csproj
```

Minimum authentication smoke test:

1. Register a customer or log in with the Development admin.
2. Copy the returned `accessToken`.
3. Call a protected endpoint with `Authorization: Bearer <access-token>`.
4. Confirm an anonymous request returns JSON `401`.
5. Confirm an authenticated owner can access their own resources and receives `403` for another owner's resources.

## Current Limitations

- Transactions, deposits, withdrawals, and transfers are not implemented.
- Refresh tokens and logout/revocation are not implemented.
- The JWT signing key is configuration-based and must be secured outside source control for production.
- The Development admin credentials are for local use only.
