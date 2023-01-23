using Microsoft.AspNetCore.Mvc;
using my_new_app.Data;
using my_new_app.Models;

namespace my_new_app.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{

    private WeatherDbContext _context;

    public WeatherForecastController(WeatherDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    [Route("all")]
    public IEnumerable<WeatherForecast> GetAll()
    {
        var q = _context.weatherForecasts.ToList();

        if (q == null)
            return (IEnumerable<WeatherForecast>)NotFound();

        return q;
    }

    [HttpGet]
    [Route("coldest")]
    public IEnumerable<WeatherForecast> GetColdest()
    {
        var q = _context.weatherForecasts.ToList();

        if (q == null)
            return (IEnumerable<WeatherForecast>)NotFound();
        var coldests = new List<WeatherForecast>();
        coldests.Add(q[0]);
        for (int i = 1; i < q.Count; i++)
        {
            if (q[i].TemperatureC == coldests[0].TemperatureC)
            {
                coldests.Add(q[i]);
            }
            else if (q[i].TemperatureC < coldests[0].TemperatureC)
            {
                coldests.RemoveAll(WeatherForecast => WeatherForecast.TemperatureC > q[i].TemperatureC);
                coldests.Add(q[i]);
            }
        }

        return coldests;
    }

    [HttpGet]
    [Route("hottest")]
    public IEnumerable<WeatherForecast> GetHottest()
    {
        var q = _context.weatherForecasts.ToList();

        if (q == null)
            return (IEnumerable<WeatherForecast>)NotFound();

        var hottest = new List<WeatherForecast>();
        hottest.Add(q[0]);
        for (int i = 1; i < q.Count; i++)
        {
            if (q[i].TemperatureC == hottest[0].TemperatureC)
            {
                hottest.Add(q[i]);
            }
            else if (q[i].TemperatureC > hottest[0].TemperatureC)
            {
                hottest.RemoveAll(WeatherForecast => WeatherForecast.TemperatureC < q[i].TemperatureC);
                hottest.Add(q[i]);
            }
        }

        return hottest;
    }
}
