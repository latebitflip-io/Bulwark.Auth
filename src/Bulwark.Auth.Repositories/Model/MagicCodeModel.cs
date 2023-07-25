namespace Bulwark.Auth.Repositories.Model;

public class MagicCodeModel
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    [BsonElement("userId")]
    public string UserId { get; init; }
    [BsonElement("code")]
    public string Code { get; init; }
    [BsonElement("expires")]
    public DateTime Expires { get; init; }
    [BsonElement("created")]
    public DateTime Created { get; init; }
}


