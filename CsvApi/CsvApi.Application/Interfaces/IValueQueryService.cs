using CsvApi.Application.DTOs;

namespace CsvApi.Application.Interfaces
{
    public interface IValueQueryService
    {
        Task<List<ValueDto>> GetLast10Async(string fileName, CancellationToken ct = default);
    }
}
