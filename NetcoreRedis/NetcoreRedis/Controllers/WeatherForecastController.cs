using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using NetcoreRedis.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace NetcoreRedis.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly string _cacheKey = "Temperatures";

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            return this.getTemperatures();
        }

        [HttpGet]
        [Route("getRedis")]
        public IEnumerable<WeatherForecast> GetRedis([FromServices] RedisConnection redisConnection)
        {
            return this.getFromRedis(redisConnection);
        }

        [HttpGet]
        [Route("setRedis")]
        public string SetRedis([FromServices] RedisConnection redisConnection)
        {
            try
            {
                var temperatures = this.getTemperatures();                
                var json = JsonSerializer.Serialize<List<WeatherForecast>>(temperatures.ToList());

                if(redisConnection.SetValue(_cacheKey, json))
                {
                    return "sucesso";
                }
                return "falha ao gravar";
                
            }
            catch (Exception e)
            {
                return e.Message.ToString();
            }
        }

        private IEnumerable<WeatherForecast> getTemperatures()
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            });
        }

        public IEnumerable<WeatherForecast> getFromRedis(RedisConnection redisConnection)
        {
            var temperatures = new List<WeatherForecast>();

            try
            {
                var json = redisConnection.GetValueFromKey(_cacheKey);

                if (json != null)
                {
                    temperatures = JsonSerializer.Deserialize<List<WeatherForecast>>(json);
                }
                return temperatures.ToArray();
            }
            catch(Exception e)
            {
                Debug.WriteLine("Erro=> " + e.Message.ToString());
                return temperatures.ToArray();
            }
        }

    }
}
