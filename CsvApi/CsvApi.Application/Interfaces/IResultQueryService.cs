using CsvApi.Application.DTOs;

namespace CsvApi.Application.Interfaces
{
    public interface IResultQueryService
    {
        Task<List<ResultDto>> GetResultsAsync(ResultFilterDto filter);
    }
}
