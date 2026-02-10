using Microsoft.AspNetCore.Http;

namespace CsvApi.Application.Interfaces
{
    public interface IFileImportService
    {
        Task ProcessCsvAsync(IFormFile file);
    }
}
