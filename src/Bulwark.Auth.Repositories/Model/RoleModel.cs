namespace Bulwark.Auth.Repositories.Model;
public class RoleModel
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    [BsonElement("name")]
    public string Name { get; set; }
    [BsonElement("permissions")] 
    public List<string> Permissions { get; set; }
    [BsonElement("created")]
    public DateTime Created { get; set; }
    [BsonElement("modified")]
    public DateTime Modified { get; set; }
}


