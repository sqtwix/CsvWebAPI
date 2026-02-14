using CsvApi.Application.Exceptions;
using CsvApi.Application.ExtraClasses;
using CsvApi.Application.Interfaces;
using CsvApi.Domain.Models;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsvApi.Application.Services
{
    public class FileImportService : IFileImportService
    {
        // FileImportService - сервис для обработки загружаемых CSV-файлов,
        // валидации данных, расчёта агрегированных результатов и сохранения их в БД.


        private readonly IValueRepository _valueRepository;
        private readonly IResultRepository _resultRepository;
        private readonly IUnitOfWork _uow;


        public FileImportService(IValueRepository valueRepository, IResultRepository resultRepository, IUnitOfWork uow)
        {
            _valueRepository = valueRepository;
            _resultRepository = resultRepository;
            _uow = uow;
        }

        // Загрузка файла в БД
        public async Task ProcessCsvAsync(IFormFile file)
        {
            // Список для сбора ошибок валидации
            var errors = new List<string>();

            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("Файл пустой или не передан");
            }

            if (!file.FileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException("Ожидается файл с расширением .csv");
            }

            string fileName = Path.GetFileNameWithoutExtension(file.FileName)?.Trim()
                ?? throw new ArgumentException("Имя файла некорректно");

            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new ArgumentException("Имя файла не может быть пустым");
            }

            var records = new List<RawCsvRecord>();

            using var stream = file.OpenReadStream();
            using var reader = new StreamReader(stream);
            using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = ";",
                HasHeaderRecord = true,
                TrimOptions = TrimOptions.Trim,

                // Игнорируем ошибки парсинга и конвертации, но собираем их
                MissingFieldFound = null,  
                BadDataFound = null,      

                ReadingExceptionOccurred = ex =>
                {
                    throw new InvalidCsvException($"Ошибка чтения CSV: {ex.ToString}");
                },

                ShouldSkipRecord = args =>
                {
                    return args.Row.Parser.Record == null || args.Row.Parser.Record.All(string.IsNullOrWhiteSpace);
                }
            });

            await foreach (var record in csv.GetRecordsAsync<RawCsvRecord>())
            {
                records.Add(record);
            }

            ValidateRows(records, fileName);

            var valueEntries = records.Select(r => new Value
            {
                FileName = fileName,
                Date = r.Date,
                ExecutionTime = r.ExecutionTime,
                FileValue = r.Value  
            }).ToList();

            if (!valueEntries.Any())
                throw new InvalidOperationException("После валидации не осталось данных");

            var result = CalculateResult(fileName, valueEntries);

            await using var uow = _uow; 

            try
            {
                await uow.BeginTransactionAsync();

                await uow.Values.DeleteByFileNameAsync(fileName);
                await uow.Results.UpsertAsync(result);

                await uow.Values.AddRangeAsync(valueEntries);

                await uow.SaveChangesAsync();

                await uow.CommitTransactionAsync();
            }
            catch
            {
                await uow.RollbackTransactionAsync();
                throw;
            }
        }

        // Дополнительный метод для валидации входных данных из файла
        private void ValidateRows(List<RawCsvRecord> records, string fileName)
        {
            var errors = new List<string>();

            if (records.Count == 0)
                errors.Add("Файл не содержит данных");

            if (records.Count > 10000)
                errors.Add("Превышено максимальное количество строк (10 000)");

            var minDate = new DateTimeOffset(2000, 1, 1, 0, 0, 0, TimeSpan.Zero);
            var now = DateTimeOffset.UtcNow;

            foreach (var (r, index) in records.Select((r, i) => (r, i)))
            {
                if (r.Date < minDate || r.Date > now)
                    errors.Add($"Строка {index + 2}: недопустимая дата {r.Date} (должна быть между 2000-01-01 и текущим моментом)");

                if (r.ExecutionTime < 0)
                    errors.Add($"Строка {index + 2}: отрицательное время выполнения {r.ExecutionTime}");

                if (r.Value < 0)
                    errors.Add($"Строка {index + 2}: отрицательное значение показателя {r.Value}");
            }

            if (errors.Any())
                throw new InvalidCsvException(errors);
        }

        // Вспомогательный метод для агрегации данных
        private Result CalculateResult(string fileName, List<Value> values)
        {
            if (!values.Any()) throw new InvalidOperationException("Нет данных для расчёта");

            var dates = values.Select(v => v.Date).ToList();
            var execTimes = values.Select(v => v.ExecutionTime).ToList();
            var vals = values.Select(v => v.FileValue).ToList();

            double deltaSeconds = (dates.Max() - dates.Min()).TotalSeconds;
            var medianValue = CalculateMedian(vals);

            return new Result
            {
                FileName = fileName,
                DeltaTimeSeconds = deltaSeconds,
                StartDate = dates.Min(),
                AvgExecutionTime = execTimes.Average(),
                AvgValue = vals.Average(),
                MedianValue = medianValue,
                MaxValue = vals.Max(),
                MinValue = vals.Min()
            };
        }

        private static double CalculateMedian(List<double> values)
        {
            var sorted = values.OrderBy(x => x).ToList();
            int n = sorted.Count;

            if (n % 2 == 1)
                return sorted[n / 2];

            return (sorted[n / 2 - 1] + sorted[n / 2]) / 2.0;
        }
    }
}
