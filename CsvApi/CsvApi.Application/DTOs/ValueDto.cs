namespace CsvApi.Application.DTOs;

public record ValueDto {
    DateTimeOffset Date;
    double ExecutionTime;
    double Value;
}