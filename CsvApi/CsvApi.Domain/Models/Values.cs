using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsvApi.Domain.Models
{
    // Модель для хранения значений из CSV файлов
    public class Values
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string FileName { get; set; } = null!;        // ключ для группировки по файлу
        public DateTimeOffset Date { get; set; }
        public double ExecutionTime { get; set; }
        public double Value { get; set; }
    }
}
