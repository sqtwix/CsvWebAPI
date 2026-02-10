namespace CsvApi.Application.DTOs
{
    public record ResultFilterDto
    {
        string? FileName = null;
        DateTimeOffset? StartDateFrom = null;
        DateTimeOffset? StartDateTo = null;
        double? AvgValueFrom = null;
        double? AvgValueTo = null;
        double? AvgExecutionTimeFrom = null;
        double? AvgExecutionTimeTo = null;
    } 
}
