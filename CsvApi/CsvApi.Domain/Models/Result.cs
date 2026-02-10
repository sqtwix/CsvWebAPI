using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsvApi.Domain.Models
{
    // Модель для хранения результатов интегрального чанализа CSV файлов
    public class Result
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string FileName { get; set; } = null!;        // unique
        public double DeltaTimeSeconds { get; set; }
        public DateTimeOffset StartDate { get; set; }
        public double AvgExecutionTime { get; set; }
        public double AvgValue { get; set; }
        public double MedianValue { get; set; }
        public double MaxValue { get; set; }
        public double MinValue { get; set; }
    }
}
