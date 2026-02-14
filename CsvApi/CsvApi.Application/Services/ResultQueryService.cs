using CsvApi.Application.DTOs;
using CsvApi.Application.Interfaces;
using CsvApi.Domain.Models;

namespace CsvApi.Application.Services
{
    public class ResultQueryService : IResultQueryService
    {
        // ResultQueryService - сервис для получения агрегированных
        // результатов из БД с поддержкой фильтрации.
        private readonly IResultRepository _resultRepository;

        public ResultQueryService(IResultRepository resultRepository)
        {
            _resultRepository = resultRepository;
        }

        public async Task<List<ResultDto>> GetResultsAsync(ResultFilterDto filter)
        {
            // Если фильтр null — можно вернуть пустой список или все записи (по выбору)
            // Здесь предполагаем, что контроллер уже проверил и передал нормальный объект
            if (filter == null)
            {
                return new List<ResultDto>();
            }

            var results = await _resultRepository.GetByFiltersAsync(
                fileName: filter.FileName,
                startFrom: filter.StartDateFrom,
                startTo: filter.StartDateTo,
                avgValueFrom: filter.AvgValueFrom,
                avgValueTo: filter.AvgValueTo,
                avgExecFrom: filter.AvgExecutionTimeFrom,
                avgExecTo: filter.AvgExecutionTimeTo);

            // Маппинг в DTO
            return results.Select(r => new ResultDto(
                 FileName: r.FileName,
                 DeltaTimeSeconds: r.DeltaTimeSeconds,
                 StartDate: r.StartDate,
                 AvgExecutionTime: r.AvgExecutionTime,
                 AvgValue: r.AvgValue,
                 MedianValue: r.MedianValue,
                 MaxValue: r.MaxValue,
                 MinValue: r.MinValue
             )).ToList();
        }
    }
}