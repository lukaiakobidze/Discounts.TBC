# TBC Discounts WebApp

A full-stack discount marketplace platform built with **.NET 8**, **Clean Architecture**, and **CQRS**. The platform connects merchants offering discounts with customers, providing a complete workflow for offer management, coupon purchasing, reservations, reviews, and admin oversight.

---

## Features

### Customer
- Browse and search discount offers by keyword, category, and price range
- Purchase coupons for active offers
- Reserve offers (temporary hold)
- Mark offers as favorites
- Leave star ratings and reviews
- View coupon and reservation history

### Merchant
- Create and manage discount offers with image uploads
- Submit offers for admin approval
- View coupon sales history and revenue analytics

### Admin
- Approve or reject merchant offers (with rejection reason)
- Manage users (block/unblock) across all roles
- Manage offer categories
- Configure system-wide global settings
- View dashboard statistics

### System
- JWT authentication with refresh tokens
- Role-based access control (Admin, Merchant, Customer)
- Background worker to auto-expire offers past their `ValidTo` date
- Background worker to clean up expired reservations
- Structured logging via Serilog

---

## Tech Stack

| Layer | Technology |
|---|---|
| Language | C# 12 / .NET 8 |
| Architecture | Clean Architecture + CQRS (MediatR) |
| Frontend | ASP.NET MVC (Razor Views) |
| API | ASP.NET Core Web API (versioned) |
| ORM | Entity Framework Core 8 |
| Database | SQL Server |
| Authentication | JWT Bearer + ASP.NET Identity |
| Validation | FluentValidation |
| Mapping | Mapster |
| Logging | Serilog |
| API Docs | Swagger / Swashbuckle |
| Testing | xUnit, Moq, FluentAssertions |

---

## Project Structure

```
DiscountsWebApp/
├── Discounts.API/                  # REST API (controllers, middleware, auth config)
├── Discounts.MVC/                  # ASP.NET MVC frontend (Views, Areas)
├── Discounts.Application/          # Business logic, CQRS features, DTOs, validators
├── Discounts.Domain/               # Domain entities, enums, constants
├── Discounts.Infrastructure/       # EF Core, repositories, identity, background services
└── Discounts.Application.Tests/    # Unit tests
```

---

## Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- SQL Server (LocalDB, Express, or full edition)
- Visual Studio 2022 / Rider / VS Code (optional)

---

## Getting Started

### 1. Clone the repository

```bash
git clone https://github.com/<your-username>/DiscountsWebApp.git
cd DiscountsWebApp
```

### 2. Configure the database connection

By default the app connects to a local SQL Server instance. Edit the connection string in:

**`Discounts.API/appsettings.json`**
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=TBC_Discounts;Trusted_Connection=True;TrustServerCertificate=True"
}
```

**`Discounts.MVC/appsettings.json`** — update the same field if running the MVC frontend.

Alternatively, override via an environment variable (no file edits required):

```bash
# Linux / macOS
export ConnectionStrings__DefaultConnection="Server=...;Database=TBC_Discounts;..."

# Windows (Command Prompt)
set ConnectionStrings__DefaultConnection=Server=...;Database=TBC_Discounts;...

# Windows (PowerShell)
$env:ConnectionStrings__DefaultConnection="Server=...;Database=TBC_Discounts;..."
```

### 3. Apply migrations & seed data

Migrations and seeding run **automatically on startup**. The application will:
1. Create the `TBC_Discounts` database if it does not exist.
2. Apply all pending EF Core migrations.
3. Seed roles (`Admin`, `Merchant`, `Customer`), a default admin user, categories, and global settings.

You can also apply migrations manually:

```bash
cd DiscountsWebApp
dotnet ef database update --project Discounts.Infrastructure --startup-project Discounts.API
```

---

## Running the Application

### API (Discounts.API)

```bash
cd Discounts.API
dotnet run
```

- HTTPS: `https://localhost:<port>`
- Swagger UI: `https://localhost:<port>/swagger`

### MVC Frontend (Discounts.MVC)

```bash
cd Discounts.MVC
dotnet run
```

- Opens the web interface for customers, merchants, and admins.
- Login page: `/Account/Login`
- Admin area: `/Admin`
- Merchant area: `/Merchant`

> You can run both projects simultaneously — the MVC app communicates with the API.

---

## Default Admin Credentials

A seed admin account is created automatically on first run. Check `Discounts.Infrastructure/Data/Seed/AdminSeed.cs` for the seeded email and password, and change them before deploying to production.

---

## Running Tests

```bash
cd Discounts.Application.Tests
dotnet test
```

Tests cover application-layer handlers, validators, and business logic using xUnit, Moq, and FluentAssertions.

---

## API Overview

Base path: `api/v1/`

| Resource | Endpoint | Access |
|---|---|---|
| Register | `POST /auth/register` | Public |
| Login | `POST /auth/login` | Public |
| Refresh token | `POST /auth/refresh-token` | Authenticated |
| List offers | `GET /offers` | Public |
| Search offers | `GET /offers/search` | Public |
| Create offer | `POST /offers` | Merchant |
| Purchase coupon | `POST /coupons/purchase` | Customer |
| Use coupon | `POST /coupons/use` | Customer |
| Reserve offer | `POST /reservations/reserve` | Customer |
| List categories | `GET /categories` | Public |
| Admin dashboard | `GET /admin/dashboard` | Admin |
| Approve offer | `POST /admin/offers/{id}/approve` | Admin |
| Reject offer | `POST /admin/offers/{id}/reject` | Admin |
| Block user | `POST /admin/users/{id}/block` | Admin |

Full interactive documentation is available via Swagger at `/swagger` when the API is running.

---

## Configuration Reference

### JWT Settings (`appsettings.json`)

```json
"JwtSettings": {
  "SecretKey": "<change-in-production>",
  "Issuer": "TBC.WebApi",
  "Audience": "TBC.WebApi",
  "AccessTokenExpirationHours": 1,
  "RefreshTokenbExpirationHours": 168,
  "CookieExpirationHours": 168
}
```

> **Important:** Replace `SecretKey` with a strong, random secret before deploying.

The secret key is resolved in this priority order:
1. Environment variable `JwtSettings__SecretKey`
2. `appsettings.json` value
3. Built-in default (development only)

Override via environment variable (recommended for production):

```bash
# Linux / macOS
export JwtSettings__SecretKey="your-strong-secret-here"

# Windows (Command Prompt)
set JwtSettings__SecretKey=your-strong-secret-here

# Windows (PowerShell)
$env:JwtSettings__SecretKey="your-strong-secret-here"
```

---

## Background Services

| Service | Description |
|---|---|
| `OfferExpirationWorker` | Periodically marks offers as `Expired` once their `ValidTo` date has passed |
| `ReservationCleanupWorker` | Removes expired reservations to keep inventory accurate |

---

## License

This project was developed as a final project for **TBC Academy**.
