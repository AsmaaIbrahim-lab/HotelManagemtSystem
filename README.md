# HotelManagement API

A .NET 10 REST API for hotel room, reservation, and report management. Built with Clean Architecture (Application / Infrastructure / API layers), EF Core + SQL Server, JWT authentication, and SignalR for real-time updates.

## Tech Stack

- **.NET 10** (net10.0)
- **ASP.NET Core Web API** + Swagger / OpenAPI
- **Entity Framework Core 10** (SQL Server)
- **MediatR** (CQRS)
- **ASP.NET Core Identity** (JWT Bearer auth)
- **SignalR** (real-time hub at `/hubs/hotel`)

## Demo Account

| | |
|---|---|
| **Email** | `admin@hotel.com` |
| **Password** | `Admin@123` |

The demo user, along with a few rooms and reservations, is seeded automatically on first startup (Development). This ensures reports and the dashboard show meaningful data out of the box.

## API Base URL

In Development the API listens on:

```
http://localhost:5188
```

Swagger UI is available at:

```
http://localhost:5188/swagger
```

## How to Run

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- SQL Server (LocalDB or a local instance)

### Steps

1. Clone the repository and open the solution folder.

2. Make sure the connection string in `HotelManagement.API/appsettings.json` points to a reachable SQL Server. See [SQL Server Connection](#sql-server-connection).

3. Run the API (the project detects Development and migrates + seeds the database automatically):

   ```bash
   dotnet run --project HotelManagement.API
   ```

   Or, from the `HotelManagement.API` folder:

   ```bash
   dotnet run
   ```

4. Open Swagger at `http://localhost:5188/swagger`, log in with the demo account, and paste the returned JWT into the `Authorize` dialog as `Bearer <token>`.

> The database is migrated and seeded automatically on startup only in the **Development** environment. If an existing environment lacks migrations, run them manually:
>
> ```bash
> dotnet ef database update --project HotelManagement.Infrastructure --startup-project HotelManagement.API
> ```

## SQL Server Connection

The default connection string lives in `HotelManagement.API/appsettings.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=.;Database=HotelManagementDb;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

- `Server=.` targets the default local SQL Server instance (use `(localdb)\\MSSQLLocalDB` for LocalDB).
- `Database=HotelManagementDb` — created automatically by the migration.
- `Trusted_Connection=True` uses Windows authentication (no SQL login required).
- Override it per environment by adding a `ConnectionStrings:DefaultConnection` entry to `appsettings.Development.json` (or an environment variable).

## JWT Settings

JWT is configured in `HotelManagement.API/appsettings.json`:

| Setting | Value | Purpose |
|---|---|---|
| `JwtKey` | `9fA7!sD23kL$gR91@XmZ#pT84QwVbNc5` | Symmetric signing key (HS256) |
| `ValidIssuer` | `http://localhost:5188` | Expected token issuer |
| `ValidAudience` | `http://localhost:5188` | Expected token audience |

- Tokens expire after **8 hours**.
- **Production note:** replace `JwtKey` with a strong, randomly generated secret and store it safely (e.g. user-secrets, environment variable, or a secret manager). Do not commit real secrets.

The access token is returned by the `/api/auth/login` (and register) endpoints. Authenticated endpoints expect the header:

```
Authorization: Bearer <token>
```

## Project Structure

```
HotelManagement.API/
├── HotelManagement.API/            # Presentation layer (controllers, hub, auth config)
│   ├── Controllers/                # Rooms, Reservation, Reports, Dashboard, AuditLog, Auth
│   ├── Hubs/HotelHub.cs            # SignalR hub
│   └── Extensions/                 # DI, Identity, Auth, CORS setup
├── HotelManagement.Application/    # Domain entities + MediatR commands/queries
│   ├── Domain/Entities/            # Room, User, Reservation, AuditLog
│   └── Features/                   # Auth, Room, Reservation, Reports, Dashboard, AuditLogs
└── HotelManagement.Infrastructure/ # EF Core, persistence, migrations, seed data, JWT
    ├── Persistence/AppDbContext.cs
    ├── Migrations/
    └── Seed/DbSeeder.cs            # Demo user, rooms, reservations
```

## Main Endpoints

| Area | Endpoints |
|---|---|
| Auth | `POST /api/auth/register`, `POST /api/auth/login` |
| Rooms | `GET /api/rooms`, `GET /api/rooms/available`, `GET /api/rooms/{id}`, `POST /api/rooms`, `PUT /api/rooms/{id}`, `DELETE /api/rooms/{id}` |
| Reservations | `GET /api/reservation`, `GET /api/reservation/{id}`, `POST /api/reservation`, `PUT /api/reservation/{id}/cancel`, `GET /api/reservation/search` |
| Reports | Occupancy, revenue, top rooms |
| Dashboard | Summary + recent activity |
| Audit Logs | `GET /api/auditlog` |
