namespace Bulwark.Auth.Repositories.Model;

public class VerificationModel
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    [BsonElement("token")]
    public string Token { get; set; }
    [BsonElement("email")]
    public string Email { get; set; }
    [BsonElement("created")]
    public DateTime Created { get; set; }

    public VerificationModel(string email, string token)
    {
        Token = token;
        Email = email;
        Created = DateTime.Now;
        Id = string.Empty;
    }
}

