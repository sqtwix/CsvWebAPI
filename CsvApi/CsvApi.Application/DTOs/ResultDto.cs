namespace CsvApi.Application.DTOs
{
    public class ResultDto() {
        string FileName;
        double DeltaTimeSeconds;
        DateTimeOffset StartDate;
        double AvgExecutionTime;
        double AvgValue;
        double MedianValue;
        double MaxValue;
        double MinValue;
    }
}
