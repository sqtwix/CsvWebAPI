using CsvApi.Application.Interfaces;
using CsvApi.Domain.Models;
using CsvApi.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CsvApi.Infrastructure.Repositories
{
    public class ValueRepository : IValueRepository
    {
        // Репозиторий для работы с таблицей Values
        
        private readonly AppDbContext _context;

        public ValueRepository(AppDbContext context)
        {
            _context = context;
        }

        // Добавляет запись в таблицу
        public async Task AddRangeAsync(IEnumerable<Value> entries)
        {
            await _context.Values.AddRangeAsync(entries);
            await _context.SaveChangesAsync(); 
        }

        // Удаление записи по имени файла
        public async Task DeleteByFileNameAsync(string fileName)
        {
            await _context.Values
                .Where(v => v.FileName == fileName)
                .ExecuteDeleteAsync();

            await _context.SaveChangesAsync();
        }

        // Получение последних 10 записей с фильтром по имени файла
        public async Task<List<Value>> GetLast10ByFileNameAsync(string fileName)
        {
            return await _context.Values
                .Where(v => v.FileName == fileName)
                .OrderByDescending(v => v.Date)
                .Take(10)
                .ToListAsync();
        }
    }
}