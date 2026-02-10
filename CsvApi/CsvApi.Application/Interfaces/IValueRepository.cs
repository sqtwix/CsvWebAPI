using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvApi.Domain.Models;

namespace CsvApi.Application.Interfaces
{
    public interface IValueRepository
    {
        Task AddRangeAsync(IEnumerable<Value> entries);
        Task DeleteByFileNameAsync(string fileName);
        Task<List<Value>> GetLast10ByFileNameAsync(string fileName);
    }
}
