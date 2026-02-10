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
        Task UpsertAsync(Result result);

        Task<List<Result>> GetByFiltersAsync(
            string? fileName,
            DateTimeOffset? startFrom,
            DateTimeOffset? startTo,
            double? avgValueFrom,
            double? avgValueTo,
            double? avgExecFrom,
            double? avgExecTo);
    }
}
