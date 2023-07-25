namespace Bulwark.Auth.Repositories.Model;
public class RoleModel
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; init; }
    [BsonElement("name")]
    public string Name { get; init; }
    [BsonElement("permissions")] 
    public List<string> Permissions { get; init; }
    [BsonElement("created")]
    public DateTime Created { get; set; }
    [BsonElement("modified")]
    public DateTime Modified { get; set; }
}


