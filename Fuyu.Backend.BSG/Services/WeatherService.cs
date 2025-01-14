using System;
using Fuyu.Backend.BSG.Models.Responses;
using Fuyu.Backend.BSG.Models.Weather;

namespace Fuyu.Backend.BSG.Services;

public class WeatherService
{
    public static WeatherService Instance => instance.Value;
    private static readonly Lazy<WeatherService> instance = new(() => new WeatherService());

    /// <summary>
    /// The construction of this class is handled in the <see cref="instance"/> (<see cref="Lazy{T}"/>)
    /// </summary>
    private WeatherService()
    {

    }

    public LocationWeatherTime CreateWeatherTime()
    {
        var currentTime = DateTime.Now;
        return new LocationWeatherTime()
        {
            Date = currentTime.ToString("yyyy-MM-dd"),
            Time = currentTime.ToString("HH:mm:ss"),
            Acceleration = 0,
            Season = (byte)Random.Shared.Next(0, 5),
            WeatherNode = CreateDefault()
        };
    }

    public WeatherNode CreateDefault()
    {
        return new()
        {
            // TODO: update with TimeService later
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            Time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
            Date = DateTime.Now.ToString("yyyy-MM-dd"),
            Cloud = -0.443f,
            WindGustiness = 0.334f,
            WindDirection = 1,
            Rain = 1,
            RainIntensity = 0f,
            Fog = 0.002f,
            Pressure = 762f,
            Temperature = 12f,
            WindSpeed = 2f
        };
    }

    // TODO: Make sure we generate default one then return edited values.
    public WeatherNode CreateWeatherType(EWeatherType weatherType)
    {
        return CreateDefault();
    }

    public WeatherResponse CreateLocalWeather()
    {
        var seasons = Enum.GetValues<ESeason>();
        var randomSeason = seasons[Random.Shared.Next(0, seasons.Length)];

        var localWeather = new WeatherResponse()
        {
            Season = randomSeason,
            Acceleration = 7f,
            Date = DateTime.Now.ToString("yyyy-MM-dd"),
            Time = DateTime.Now.ToString("HH:mm:ss")
        };

        localWeather.WeatherNode = CreateWeatherType(EWeatherType.ClearDay);

        return localWeather;
    }
}