# Pharmacy Inventory API — Clean Architecture Demo

Демонстрационный REST API для учёта медикаментов и складских остатков в аптеке, построенный по принципам **слоистой (чистой) архитектуры** с паттернами **Repository** и **Unit of Work**.

Проект специально спроектирован так, чтобы показать не просто "работающий CRUD", а архитектурный подход: разделение ответственности между слоями, независимость бизнес-логики от инфраструктуры, тестируемость через моки.

## Архитектура

```
dotnet-clean-api/
├── src/
│   ├── Pharmacy.Domain/          # Сущности и интерфейсы. Ни от чего не зависит.
│   │   ├── Entities/              # Medicine, Supplier, StockMovement
│   │   └── Interfaces/            # IRepository, IMedicineRepository, IUnitOfWork
│   ├── Pharmacy.Application/     # Бизнес-логика. Зависит только от Domain.
│   │   ├── DTOs/
│   │   ├── Services/              # MedicineService — вся бизнес-логика здесь
│   │   └── Exceptions/
│   ├── Pharmacy.Infrastructure/  # EF Core, реализация репозиториев. Зависит от Domain.
│   │   ├── Data/                  # AppDbContext
│   │   └── Repositories/          # Repository<T>, MedicineRepository, UnitOfWork
│   └── Pharmacy.Api/             # Контроллеры, DI, запуск приложения.
│       └── Controllers/
└── tests/
    └── Pharmacy.Application.Tests/  # xUnit + Moq — тесты бизнес-логики без БД
```

**Ключевая идея:** `Pharmacy.Application` не знает про Entity Framework и вообще про то, как хранятся данные — она работает только с интерфейсами из `Pharmacy.Domain`. Это позволяет тестировать бизнес-логику (например, проверку остатков при списании) через моки репозитория, без поднятия реальной базы.

## Функциональность

- CRUD для медикаментов
- Получение списка товаров с низким остатком (`/low-stock`)
- Регистрация движения товара (приход/расход) с проверкой, что нельзя списать больше, чем есть на складе
- Единообразная обработка ошибок (404 / 400 / 409)

## Стек

ASP.NET Core 8 Web API · Entity Framework Core (SQLite) · Repository + Unit of Work · xUnit · Moq · Swagger

## Запуск

```bash
cd src/Pharmacy.Api
dotnet restore
dotnet run
```

Swagger UI будет доступен на `https://localhost:xxxx/swagger` (порт покажет консоль при запуске). База данных — SQLite-файл, создаётся автоматически при первом запуске с тестовыми данными (2 медикамента, 1 поставщик).

## Тесты

```bash
cd tests/Pharmacy.Application.Tests
dotnet test
```

6 тестов проверяют бизнес-логику сервиса через моки `IUnitOfWork`/`IMedicineRepository` — без обращения к реальной базе данных.

## Пример запроса

```
POST /api/medicines/stock-movements
{
  "medicineId": 2,
  "type": "Outgoing",
  "quantity": 100,
  "note": "Продажа партии"
}
```
→ `409 Conflict`, так как на складе только 8 единиц (см. seed-данные) — бизнес-логика не позволяет уйти в минус.
