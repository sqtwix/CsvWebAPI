using CsvApi.Application.DTOs;
using CsvApi.Application.Exceptions;
using CsvApi.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CsvApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ResultsController : ControllerBase
    {
        // ResultsController - основной контроллер с 3 endpoint'ами для:
        //  - загрузки CSV-файла и сохранения данных в БД
        //  - получения агрегированных результатов
        //  - получения последних 10 значений по имени файла 

        private readonly IFileImportService _fileImportService;
        private readonly IResultQueryService _resultQueryService;
        private readonly IValueQueryService _valueQueryService;

        public ResultsController(
            IFileImportService fileImportService,
            IResultQueryService resultQueryService,
            IValueQueryService valueQueryService)
        {
            _fileImportService = fileImportService;
            _resultQueryService = resultQueryService;
            _valueQueryService = valueQueryService;
        }

        /// <summary>
        /// Загрузка CSV-файла и сохранение результатов обработки
        /// </summary>
        /// <param name="file">CSV-файл в формате multipart/form-data</param>
        /// <response code="200">Файл успешно обработан</response>
        /// <response code="400">Неверный формат файла или данные не прошли валидацию</response>
        [HttpPost("upload")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadCsv(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Файл не передан или пустой");

            try
            {
                await _fileImportService.ProcessCsvAsync(file);
                return Ok("Файл успешно обработан и сохранён");
            }
            catch (InvalidCsvException ex)
            {
                return BadRequest(new
                {
                    Message = "Файл не прошёл валидацию",
                    Errors = ex.Message.Split('\n', StringSplitOptions.RemoveEmptyEntries)
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (FormatException ex)
            {
                return BadRequest($"Ошибка формата CSV: {ex.Message}");
            }
            //catch (Exception)
            //{
            //    return StatusCode(500, "Internal server error");
            //}
        }

        /// <summary>
        /// Получение списка агрегированных результатов с фильтрами
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<List<ResultDto>>> GetResults(
            [FromQuery] ResultFilterDto filter)
        {
            if (filter == null)
                return BadRequest("Фильтр не передан");

            try
            {
                var results = await _resultQueryService.GetResultsAsync(filter);
                return Ok(results);
            }
            catch (Exception)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Получение последних 10 значений по имени файла
        /// </summary>
        /// <param name="fileName">Имя файла без расширения</param>
        [HttpGet("{fileName}/last10")]
        public async Task<ActionResult<List<ValueDto>>> GetLast10(string fileName)
        {
            try
            {
                var values = await _valueQueryService.GetLast10Async(fileName);
                return Ok(values);
            }
            catch (Exception)
            {
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
