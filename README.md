# Warehouse Tracking Service

![CI](https://github.com/Polin-zu/warehouse-tracking-service/actions/workflows/ci.yml/badge.svg)

Учебный pet-проект: сервис учёта товаров на складе. Делал, чтобы разобраться со стеком, который используется в высоконагруженных системах — Kafka, Redis, gRPC, событийная архитектура поверх обычного REST API.

## Что делает сервис

Три операции над остатками товара на складе:

- приёмка товара в ячейку
- списание (отгрузка) товара из ячейки
- перемещение товара между ячейками

Каждая операция сохраняется в Postgres и публикует событие в Kafka. Отдельный consumer слушает эти события и держит актуальный остаток в Redis — то есть "быстрое" чтение остатка не бьёт в базу каждый раз, а идёт в кэш. Читать остаток можно через gRPC.

## Стек

- .NET 8 / C#
- PostgreSQL + EF Core
- Kafka (Confluent.Kafka)
- Redis (StackExchange.Redis)
- gRPC
- Docker / docker-compose
- xUnit + Moq
- GitHub Actions (CI)

## Архитектура

Классическое разделение на слои:

- **Domain** — сущности и бизнес-правила, без зависимостей от инфраструктуры. `StockRecord` сам следит, чтобы нельзя было списать больше, чем есть на остатке.
- **Application** — сценарии использования (`ReceiveStockHandler`, `WithdrawStockHandler`, `MoveStockHandler`). Знает только про интерфейсы (`IStockRepository`, `IEventPublisher`), не про конкретные Postgres/Kafka.
- **Infrastructure** — реализация этих интерфейсов: EF Core репозиторий, Kafka publisher/consumer, Redis.
- **Api** — REST-контроллеры и gRPC-сервис, точка входа.

Зависимости идут в одну сторону: Api → Infrastructure → Application → Domain. Domain не знает вообще ни о чём снаружи.

### Почему так

Это даёт возможность тестировать бизнес-логику (Domain, Application) без реальной базы и без Kafka — в тестах Application-слоя репозиторий и publisher подменяются моками. Позже, если понадобится сменить Postgres на что-то другое, обработчики менять не придётся.

### Поток данных на примере приёмки товара

1. REST-запрос → `ReceiveStockHandler`
2. Хендлер сохраняет изменение в Postgres (через `IStockRepository`)
3. Хендлер публикует `StockReceivedEvent` в Kafka
4. `StockEventConsumer` (фоновый сервис) читает событие, обновляет агрегированный остаток в Redis
5. Остаток читается через gRPC `GetStockLevel` прямо из Redis, без похода в Postgres

Идемпотентность: у каждого события есть `EventId`, перед обработкой consumer проверяет в Redis, не обрабатывалось ли уже такое событие — если Kafka доставит сообщение повторно, остаток не "задвоится".

## Как запустить

Нужен Docker.

```bash
git clone https://github.com/Polin-zu/warehouse-tracking-service.git
cd warehouse-tracking-service
docker compose up -d --build
```

Поднимутся четыре контейнера: Postgres, Kafka, Redis и само приложение. Swagger будет доступен на `http://localhost:8080/swagger`.

Если база поднимается впервые, может понадобиться применить миграции вручную:

```bash
cd Warehouse.Infrastructure
dotnet ef database update --startup-project ../Warehouse.Api
```

## Тесты

```bash
dotnet test
```

Покрыты Domain-правила (`StockRecord`: приём, списание, защита от ухода в минус) и сценарии Application-слоя (`ReceiveStockHandler` — создание новой записи остатка и обновление существующей).

## Структура решения  
```
Warehouse.Domain — сущности, бизнес-правила  
Warehouse.Application — сценарии использования, интерфейсы  
Warehouse.Infrastructure — EF Core, Kafka, Redis  
Warehouse.Api — REST + gRPC  
Warehouse.Domain.Tests  
Warehouse.Application.Tests  
```
