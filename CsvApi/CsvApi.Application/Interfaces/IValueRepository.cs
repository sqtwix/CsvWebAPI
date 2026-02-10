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
        Task AddRangeAsync(IEnumerable<Values> entries);
        Task DeleteByFileNameAsync(string fileName);
        Task<List<Values>> GetLast10ByFileNameAsync(string fileName);
    }
}
