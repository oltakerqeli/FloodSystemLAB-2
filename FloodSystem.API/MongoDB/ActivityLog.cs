using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace FloodSystem.API.MongoDB;

public class ActivityLog
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }
    public int UserId { get; set; }
    public string Action { get; set; } = string.Empty;
    public string Entity { get; set; } = string.Empty;
    public string Time { get; set; } = DateTime.UtcNow.ToString();
}