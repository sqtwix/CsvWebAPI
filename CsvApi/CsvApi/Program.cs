using CsvApi.Application.Interfaces;
using CsvApi.Application.Services;
using CsvApi.Domain.Models;
using CsvApi.Infrastructure.Context;
using CsvApi.Infrastructure.Persistence;
using CsvApi.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// 1. Добавление контроллеров
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });

// 2. Swagger / Open API
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "CSV Processing API",
        Version = "v1",
        Description = "API для загрузки CSV-файлов, сохранения результатов обработки БД",
        Contact = new OpenApiContact
        {
            Name = "Shulga Ivan",
            Email = "ivan20140767@gmail.com"
        }
    });

    // Чтобы Swagger правильно показывал IFormFile
    c.OperationFilter<AddFileUploadParams>();
});

// 3. PostgreSQL + EF Core + TimescaleDB
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(connectionString, npgsqlOptions =>
    {
        npgsqlOptions.SetPostgresVersion(13, 0); // или актуальную версию твоего PostgreSQL
        npgsqlOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
    });
});

// 4. Регистрация зависимостей

// Репозитории
builder.Services.AddScoped<IValueRepository, ValueRepository>();
builder.Services.AddScoped<IResultRepository, ResultRepository>();

// Сервисы 
builder.Services.AddScoped<IFileImportService, FileImportService>();
builder.Services.AddScoped<IResultQueryService, ResultQueryService>();
builder.Services.AddScoped<IValueQueryService, ValueQueryService>();

// UnitOfWork 
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// 5. Дополнительно (CORS, ProblemDetails, etc.)
builder.Services.AddProblemDetails(); 

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Middleware pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "CSV Processing API v1");
        c.RoutePrefix = string.Empty;
    });

    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        // Применяем миграции при запуске приложения
        db.Database.Migrate();       
    }
}

app.UseHttpsRedirection();

app.UseCors();

app.UseAuthorization();

app.MapControllers();

app.Run();


// Вспомогательный фильтр для Swagger (IFormFile)
internal class AddFileUploadParams : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var fileParams = context.MethodInfo.GetParameters()
            .Where(p => p.ParameterType == typeof(IFormFile));

        if (!fileParams.Any()) return;

        operation.RequestBody = new OpenApiRequestBody
        {
            Content = {
                ["multipart/form-data"] = new OpenApiMediaType
                {
                    Schema = new OpenApiSchema
                    {
                        Type = "object",
                        Properties = {
                            ["file"] = new OpenApiSchema { Type = "string", Format = "binary" }
                        },
                        Required = new HashSet<string> { "file" }
                    }
                }
            }
        };
    }
}