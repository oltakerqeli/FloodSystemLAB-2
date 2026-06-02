using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace FloodSystem.API.MongoDB;

public class LoginActivityLog
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    public int? UserId { get; set; }

    public string Email { get; set; } = string.Empty;

    public string Action { get; set; } = string.Empty;

    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}