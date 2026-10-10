# Pharmacy Inventory API — Clean Architecture Demo
en English | [🇷🇺 Russian](README.ru.md)

A demo REST API for tracking medicines and pharmacy inventory, built using **layered (clean) architecture** principles with **Repository** and **Unit of Work** patterns.

This project is intentionally designed to show more than just "working CRUD". It demonstrates an architectural approach: clear separation of responsibilities between layers, business logic independence from infrastructure, and testability via mocks.

## Architecture

```
dotnet-clean-api/
├── src/
│   ├── Pharmacy.Domain/          # Entities and interfaces. Depends on nothing.
│   │   ├── Entities/              # Medicine, Supplier, StockMovement
│   │   └── Interfaces/            # IRepository, IMedicineRepository, IUnitOfWork
│   ├── Pharmacy.Application/     # Business logic. Depends only on Domain.
│   │   ├── DTOs/
│   │   ├── Services/              # MedicineService — all business logic is here
│   │   └── Exceptions/
│   ├── Pharmacy.Infrastructure/  # EF Core, repository implementations. Depends on Domain.
│   │   ├── Data/                  # AppDbContext
│   │   └── Repositories/          # Repository<T>, MedicineRepository, UnitOfWork
│   └── Pharmacy.Api/             # Controllers, DI, app startup.
│       └── Controllers/
└── tests/
    └── Pharmacy.Application.Tests/  # xUnit + Moq — business logic tests without a DB
```

**Key idea:** `Pharmacy.Application` knows nothing about Entity Framework or how data is stored in general. It works only with interfaces from `Pharmacy.Domain`. This makes it possible to test business logic (for example, stock validation on outgoing movements) using repository mocks, without spinning up a real database.

## Features

- CRUD for medicines
- Get low-stock items (`/low-stock`)
- Register stock movements (incoming/outgoing) with validation that outgoing quantity cannot exceed current stock
- Consistent error handling (404 / 400 / 409)

## Tech Stack

ASP.NET Core 8 Web API · Entity Framework Core (SQLite) · Repository + Unit of Work · xUnit · Moq · Swagger

## Run

```bash
cd src/Pharmacy.Api
dotnet restore
dotnet run
```

Swagger UI will be available at `https://localhost:5000/swagger` (the port is shown in the console on startup). The database is a SQLite file created automatically on first run with seed data (2 medicines, 1 supplier).

## Tests

```bash
cd tests/Pharmacy.Application.Tests
dotnet test
```

6 tests validate service business logic using `IUnitOfWork`/`IMedicineRepository` mocks, without touching a real database.

## Request Example

```
POST /api/medicines/stock-movements
{
  "medicineId": 2,
  "type": "Outgoing",
  "quantity": 100,
  "note": "Batch sale"
}
```
→ `409 Conflict`, because only 8 units are in stock (see seed data), and the business logic does not allow negative inventory.
