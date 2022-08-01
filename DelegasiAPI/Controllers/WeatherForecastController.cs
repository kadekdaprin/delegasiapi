using DelegasiAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace DelegasiAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly WeatherForecastData _weatherForecastData;
        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, WeatherForecastData weatherForecastData)
        {
            _weatherForecastData = weatherForecastData;
            _logger = logger;
        }

        // TODO: tambahkan pencarian WeatherForecast berdasarkan summary. Misal ketik "free" maka akan muncul data yang mengandung summary dg kata "free"
        [HttpGet]
        public ActionResult<List<WeatherForecast>> Get(string? summary = null, int? pageIndex = 0, int? pageSize = 2)
        {
            var data = _weatherForecastData.Data;

            if (!string.IsNullOrEmpty(summary))
            {
                data = data.Where(x => x.Summary != null && x.Summary.Contains(summary, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            var count = data.Count;

            data = data.Skip(pageIndex * pageSize ?? 0).Take(pageSize ?? 2).ToList();

            var result = new PageResult<WeatherForecast>(data, pageIndex.GetValueOrDefault(), pageSize.GetValueOrDefault(), count);

            return Ok(result);
        }

        [HttpGet("{id}", Name = "GetWeatherForecastById")]
        public ActionResult<WeatherForecast> Get(int id)
        {
            var result = _weatherForecastData.Data.FirstOrDefault(x => x.Id == id);

            if (result == null) return NotFound();

            return Ok(result);
        }

        [HttpPost]
        public ActionResult<WeatherForecast> Post([FromBody] WeatherForecast model)
        {
            if (model == null)
            {
                return BadRequest(new { message = "Model tidak ada" });
            }

            if (_weatherForecastData.Data.Any(x => x.Id == model.Id))
            {
                return BadRequest(new { message = "Data duplikat" });
            }

            _weatherForecastData.Data.Add(model);

            return Created(Url.Action("Get", new { model.Id }), model);
        }

        [HttpPut("{id}")]
        public ActionResult<WeatherForecast> Put(int id, [FromBody] WeatherForecast model)
        {
            if (model == null)
            {
                return BadRequest(new { message = "Model tidak ada" });
            }

            var existingModel = _weatherForecastData.Data.FirstOrDefault(x => x.Id == id);

            if (existingModel == null) return NotFound();

            existingModel.Date = model.Date;
            existingModel.TemperatureC = model.TemperatureC;
            existingModel.Summary = model.Summary;

            return Ok(existingModel);
        }

        [HttpDelete("{id}")]
        public ActionResult<WeatherForecast> Put(int id)
        {
            var existingModel = _weatherForecastData.Data.FirstOrDefault(x => x.Id == id);

            if (existingModel == null) return NotFound();

            _weatherForecastData.Data.Remove(existingModel);

            return Ok(existingModel);
        }
    }
}