using CsvApi.Domain.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvApi.Domain.Models;

namespace CsvApi.Application.Interfaces
{
    public interface IResultRepository
    {
        public interface IResultRepository
        {
            Task UpsertAsync(Results result); 
            Task<List<Results>> GetByFiltersAsync(string? fileName, DateTimeOffset? startFrom, DateTimeOffset? startTo,
                double? avgValueFrom, double? avgValueTo, double? avgExecFrom, double? avgExecTo);
        }
    }
}
