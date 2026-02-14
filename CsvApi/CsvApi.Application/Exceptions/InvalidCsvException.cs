using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsvApi.Application.Exceptions
{
    public class InvalidCsvException : Exception
    {
        public InvalidCsvException(string message) : base(message) { }

        public InvalidCsvException(IEnumerable<string> errors)
            : base("Найдены ошибки в файле:\n" + string.Join("\n", errors)) { }
    }
}
