using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using my_new_app.Data;
using my_new_app.Models;

namespace my_new_app.Controllers
{
    [Route("api/[controller]/[action]")]
    public class WeatherForecastsController : Controller
    {
        private WeatherDbContext _context;

        public WeatherForecastsController(WeatherDbContext context) {
            _context = context;
        }

        [HttpGet]
        [Route("all")]
        public async Task<IActionResult> Get(DataSourceLoadOptions loadOptions) {
            var weatherforecasts = _context.weatherForecasts.Select(i => new {
                i.Id,
                i.City,
                i.Date,
                i.TemperatureC,
                i.TemperatureF,
                i.Summary
            });

            // If underlying data is a large SQL table, specify PrimaryKey and PaginateViaPrimaryKey.
            // This can make SQL execution plans more efficient.
            // For more detailed information, please refer to this discussion: https://github.com/DevExpress/DevExtreme.AspNet.Data/issues/336.
            // loadOptions.PrimaryKey = new[] { "Id" };
            // loadOptions.PaginateViaPrimaryKey = true;

            return Json(await DataSourceLoader.LoadAsync(weatherforecasts, loadOptions));
        }

        [HttpPost]
        public async Task<IActionResult> Post(string values) {
            var model = new WeatherForecast();
            var valuesDict = JsonConvert.DeserializeObject<IDictionary>(values);
            PopulateModel(model, valuesDict);

            if(!TryValidateModel(model))
                return BadRequest(GetFullErrorMessage(ModelState));

            var result = _context.weatherForecasts.Add(model);
            await _context.SaveChangesAsync();

            return Json(new { result.Entity.Id });
        }

        [HttpPut]
        public async Task<IActionResult> Put(int key, string values) {
            var model = await _context.weatherForecasts.FirstOrDefaultAsync(item => item.Id == key);
            if(model == null)
                return StatusCode(409, "Object not found");

            var valuesDict = JsonConvert.DeserializeObject<IDictionary>(values);
            PopulateModel(model, valuesDict);

            if(!TryValidateModel(model))
                return BadRequest(GetFullErrorMessage(ModelState));

            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete]
        public async Task Delete(int key) {
            var model = await _context.weatherForecasts.FirstOrDefaultAsync(item => item.Id == key);

            _context.weatherForecasts.Remove(model);
            await _context.SaveChangesAsync();
        }


        private void PopulateModel(WeatherForecast model, IDictionary values) {
            string ID = nameof(WeatherForecast.Id);
            string CITY = nameof(WeatherForecast.City);
            string DATE = nameof(WeatherForecast.Date);
            string TEMPERATURE_C = nameof(WeatherForecast.TemperatureC);
            string SUMMARY = nameof(WeatherForecast.Summary);

            if(values.Contains(ID)) {
                model.Id = Convert.ToInt32(values[ID]);
            }

            if(values.Contains(CITY)) {
                model.City = Convert.ToString(values[CITY]);
            }

            if(values.Contains(DATE)) {
                model.Date = Convert.ToDateTime(values[DATE]);
            }

            if(values.Contains(TEMPERATURE_C)) {
                model.TemperatureC = Convert.ToInt32(values[TEMPERATURE_C]);
            }

            if(values.Contains(SUMMARY)) {
                model.Summary = Convert.ToString(values[SUMMARY]);
            }
        }

        private string GetFullErrorMessage(ModelStateDictionary modelState) {
            var messages = new List<string>();

            foreach(var entry in modelState) {
                foreach(var error in entry.Value.Errors)
                    messages.Add(error.ErrorMessage);
            }

            return String.Join(" ", messages);
        }
    }
}