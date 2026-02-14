using CsvApi.Application.Interfaces;
using CsvApi.Infrastructure.Context;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsvApi.Infrastructure.Persistence
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        private IDbContextTransaction? _transaction;

        public IValueRepository Values { get; }
        public IResultRepository Results { get; }

        public UnitOfWork(
            AppDbContext context,
            IValueRepository values,
            IResultRepository results)
        {
            _context = context;
            Values = values;
            Results = results;
        }

        public async Task BeginTransactionAsync(CancellationToken ct = default)
        {
            _transaction = await _context.Database.BeginTransactionAsync(ct);
        }

        public async Task CommitTransactionAsync(CancellationToken ct = default)
        {
            if (_transaction != null) await _transaction.CommitAsync(ct);
        }

        public async Task RollbackTransactionAsync(CancellationToken ct = default)
        {
            if (_transaction != null) await _transaction.RollbackAsync(ct);
        }

        public async Task<int> SaveChangesAsync(CancellationToken ct = default)
        {
            return await _context.SaveChangesAsync(ct);
        }

        public async ValueTask DisposeAsync()
        {
            if (_transaction != null) await _transaction.DisposeAsync();
            await _context.DisposeAsync();
        }
    }
}
