using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace FloodSystem.API.MongoDB;

public class WeatherRawData
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    public int LocationId { get; set; }
    public string LocationName { get; set; } = string.Empty;
    public decimal Temperature { get; set; }
    public decimal Rainfall { get; set; }
    public decimal Humidity { get; set; }
    public string WeatherCondition { get; set; } = string.Empty;
    public decimal WindSpeed { get; set; }
    public decimal Pressure { get; set; }
    public DateTime FetchedAt { get; set; } = DateTime.UtcNow;
    public string Source { get; set; } = "OpenWeatherMap";
}