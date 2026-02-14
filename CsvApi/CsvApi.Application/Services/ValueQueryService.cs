using CsvApi.Application.Interfaces;
using System;
using CsvApi.Application.DTOs;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsvApi.Application.Services
{
    public class ValueQueryService : IValueQueryService
    {
        // ValueQueryService - сервис для получения последних 10 значений из БД по имени файла.
        private readonly IValueRepository _valueRepository;
        public ValueQueryService(IValueRepository valueRepository)
        {
            _valueRepository = valueRepository;
        }

        public async Task<List<ValueDto>> GetLast10Async(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new ArgumentException("Имя файла не может быть пустым");
            }

            var trimmedFileName = fileName.Trim();

            var values = await _valueRepository.GetLast10ByFileNameAsync(trimmedFileName);

            return values.Select(v => new ValueDto(
                Date: v.Date,           
                ExecutionTime: v.ExecutionTime,   
                Value: v.FileValue
            )).ToList();
        }

    }
}
