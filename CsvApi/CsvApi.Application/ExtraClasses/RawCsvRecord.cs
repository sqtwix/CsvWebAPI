using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsvApi.Application.ExtraClasses
{
    public class RawCsvRecord
    {
        public DateTimeOffset Date { get; set; }
        public double ExecutionTime { get; set; }
        public double Value { get; set; }
    }
}
