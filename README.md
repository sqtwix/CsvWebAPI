# CsvWebAPI
Тренировочный проект webAPI который работает с csv файлами

# CSV Processing API

Web API на .NET 8 для загрузки CSV-файлов, их валидации, расчёта агрегированных показателей и хранения данных в **PostgreSQL**.  
Проект выполнен в Onion Architecture с разделением на слои:  
**Domain → Application → Infrastructure → API**.

## Технологии

- .NET 8  
- ASP.NET Core Web API  
- Entity Framework Core 8 + Npgsql  
- PostgreSQL
- CsvHelper (парсинг CSV)  
- Swagger / OpenAPI  

## Функционал (соответствует ТЗ)

1. **POST /api/results/upload** — загрузка CSV-файла  
   - Формат: `Date;ExecutionTime;Value`  
   - Полная валидация:  
     - дата не позже текущей и не раньше 01.01.2000  
     - ExecutionTime ≥ 0  
     - Value ≥ 0  
     - количество строк от 1 до 10 000  
     - все поля присутствуют и корректны  
   - При ошибке — откат транзакции + понятное сообщение **400 Bad Request** со всеми проблемами  
   - Перезапись данных при повторной загрузке того же файла

2. **GET /api/results** — получение агрегированных результатов с фильтрами  
   - Фильтры: по имени файла, диапазон StartDate, диапазон AvgValue, диапазон AvgExecutionTime

3. **GET /api/results/{fileName}/last10** — последние 10 записей по имени файла  
   - Сортировка по Date DESC

## Как запустить проект (локально)

### Предварительные требования

1. **.NET 8 SDK**  
   Скачать: https://dotnet.microsoft.com/en-us/download/dotnet/8.0  
   Проверить версию:  
   ```bash
   dotnet --version
   ```
Должно показать 8.0.xxx

2. PostgreSQL 13+ (рекомендуется 16)
Windows: https://www.postgresql.org/download/windows/
macOS: brew install postgresql@16
Linux: sudo apt install postgresql-16
При установке задайте пароль для пользователя postgres (например, 123)


### Шаг за шагом

1. Склонируйте репозиторийBashgit clone https://github.com/sqtwix/CsvWebAPI.git
cd CsvWebAPI

2. Восстановите зависимости:
```bash
dotnet restore
```

3. Создайте базу данных (один раз)
Подключитесь к PostgreSQL (через pgAdmin, psql или DBeaver):SQLCREATE DATABASE csv_timescale;
Создайте и примените миграцию вручную

Если автоматическое применение не сработало:
```bash
dotnet ef migrations add InitialCreate `
  --project CsvApi.Infrastructure `
  --startup-project CsvApi.API `
  --context AppDbContextПримените миграцию:Bashdotnet ef database update `
  --project CsvApi.Infrastructure `
  --startup-project CsvApi.API `
 ```
 
4. Запустите приложение:
```bash
dotnet run --project CsvApi.API
```

Или просто из корня проекта:
```bash
dotnet run
```

5. При первом запуске в режиме Development миграции применятся автоматически.
Откройте Swagger
Перейдите в браузере по адресу:
http://localhost:5000 (или https://localhost:5001 — смотрите в консоли)Swagger откроется автоматически.

### Настройки подключения
Строка подключения находится в файле appsettings.Development.json:
```JSON{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=csv_timescale;Username=postgres;Password=1234"
  }
}
```

Если ваш пароль отличается — измените здесь.


### Структура проекта
```
textCsvApi.sln
├── CsvApi.API                ← Web API, контроллеры, Program.cs, Swagger
├── CsvApi.Application        ← сервисы, DTO, бизнес-логика, исключения
├── CsvApi.Domain             ← чистые модели (Value, Result)
└── CsvApi.Infrastructure     ← DbContext, репозитории, UnitOfWork, миграции
```

### Возможные ошибки и решения

1. "password authentication failed"
Проверьте пароль в appsettings.Development.json и в PostgreSQL.
Сбросьте пароль (если нужно):SQLALTER USER postgres WITH PASSWORD '1234';

2. База не найдена
Создайте базу csv_timescale вручную (см. шаг 3)

3. Порт 5000/5001 занят
Измените в Properties/launchSettings.json или запустите с:Bashdotnet run --project CsvApi.API --urls "http://localhost:5100"

4. Миграции не применяются
Выполните вручную (см. шаг 4)

### Тестирование через Swagger

1. POST /api/results/upload
Выберите файл → valid_data.csv
Ожидаемый ответ: 200 

2. GET /api/results
Без параметров → все записи
С фильтром ?fileName=valid_data → только нужный файл

3. GET /api/results/valid_data/last10
Последние 10 записей файла
