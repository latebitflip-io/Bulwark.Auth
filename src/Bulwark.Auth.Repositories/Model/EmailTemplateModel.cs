namespace Bulwark.Auth.Repositories.Model;

public class EmailTemplateModel
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    
    [BsonElement("name")]
    public string Name { get; init; }
    
    [BsonElement("template")]
    public string Template { get; init; }
    
    [BsonElement("created")]
    public DateTime Created { get; set; }
    
    [BsonElement("modified")]
    public DateTime Modified { get; set; }
    
    public EmailTemplateModel(string name, string template)
    {
        Name = name;
        Template = template;
        Created = DateTime.Now;
        Modified = Created;
        Id = string.Empty;
    }
}