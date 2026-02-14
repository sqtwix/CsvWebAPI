using CsvApi.Application.Interfaces;
using CsvApi.Domain.Models;
using CsvApi.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CsvApi.Infrastructure.Repositories
{
    public class ResultRepository : IResultRepository
    {
        // Репозиторий для работы с таблицей Results

        private readonly AppDbContext _context;

        public ResultRepository(AppDbContext context)
        {
            _context = context;
        }

        // Перезапись старого файла новым
        public async Task UpsertAsync(Result result)
        {
            // Удаление старой записи
            await _context.Results
                .Where(r => r.FileName == result.FileName)
                .ExecuteDeleteAsync();

             // Добавляем новую
             await _context.Results.AddAsync(result);
             await _context.SaveChangesAsync();
        }

        // Получение записей по фильтрам
        public async Task<List<Result>> GetByFiltersAsync(
        string? fileName,
        DateTimeOffset? startFrom,
        DateTimeOffset? startTo,
        double? avgValueFrom,
        double? avgValueTo,
        double? avgExecFrom,
        double? avgExecTo)
        {
            IQueryable<Result> query = _context.Results.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(fileName))
            {
                query = query.Where(r => r.FileName == fileName);
            }

            if (startFrom.HasValue)
                query = query.Where(r => r.StartDate >= startFrom.Value);

            if (startTo.HasValue)
                query = query.Where(r => r.StartDate <= startTo.Value);

            if (avgValueFrom.HasValue)
                query = query.Where(r => r.AvgValue >= avgValueFrom.Value);

            if (avgValueTo.HasValue)
                query = query.Where(r => r.AvgValue <= avgValueTo.Value);

            if (avgExecFrom.HasValue)
                query = query.Where(r => r.AvgExecutionTime >= avgExecFrom.Value);

            if (avgExecTo.HasValue)
                query = query.Where(r => r.AvgExecutionTime <= avgExecTo.Value);

            // Сортировка по умолчанию — по дате запуска (новые сверху)
            query = query.OrderByDescending(r => r.StartDate);

            return await query.ToListAsync();
        }
    }
}
