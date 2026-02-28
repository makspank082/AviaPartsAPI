AviaPartsAPI - Production-ready REST API

Production-ready REST API для управления складом запчастей. Проект создан как case study для демонстрации практических навыков разработки на .NET 8 с использованием современных архитектурных подходов.

## Технологии

- **Backend:** ASP.NET Core 9, C# 10
- **Database:** PostgreSQL, Entity Framework Core 9
- **Architecture:** CQRS, Layered Architecture
- **Infrastructure:** Docker, Docker Compose
- **Documentation:** Swagger/OpenAPI
- **Monitoring:** Custom Health Checks

## Структура проекта
AviaPartsAPI/
├── Commands/ # Command handlers (CQRS)
├── Queries/ # Query handlers (CQRS)
├── Controllers/ # HTTP controllers
├── Data/ # DbContext and migrations
├── DTOs/ # Data Transfer Objects
├── Middleware/ # Global exception handling
├── Services/ # Business logic
├── Program.cs # Application entry point
├── Dockerfile # Container configuration
├── docker-compose.yml # Multi-container setup
└── README.md # This file

## Архитектурные решения

### CQRS Pattern
Разделение операций чтения и записи для соблюдения принципов SOLID:
- `PartQueryService` - операции чтения (GET)
- `PartCommandService` - операции записи (POST, PUT, DELETE)

### Production Health Checks
Кастомные проверки работоспособности с бизнес-логикой:
- Контроль критически низких остатков
- Мониторинг устаревших инвентаризаций

### Centralized Exception Handling
Глобальный обработчик исключений через middleware:
- Стандартизированные JSON-ответы
- Маппинг исключений на HTTP-статусы
- Логирование ошибок

### Containerization
Полная контейнеризация с Docker Compose:
- Автоматический запуск API и PostgreSQL
- Автомиграции базы данных
- Воспроизводимое окружение
 
## Быстрый старт

### Способ 1: Docker (рекомендуется)
1. Клонировать репозиторий:
git clone https://github.com/makspank082/AviaPartsAPI.git
2. Перейти в папку проекта:
cd AviaPartsAPI
3. Запустить контейнеры:
docker-compose up --build

### Способ 2: Локальный запуск
1. Клонировать репозиторий
2. Установить .NET 9 SDK и PostgreSQL
3. Перейти в папку проекта
4. Восстановить зависимости:
dotnet restore
5. Запустить приложение:
dotnet run

## Планы по развитию проекта

- Аутентификация и авторизация (JWT)
- Юнит-тестирование (xUnit)

## Лицензия
© 2026 Максим Панков. Pet-project AviaPartsAPI.
