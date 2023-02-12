namespace Bulwark.Auth.Repositories.Model;

[BsonIgnoreExtraElements]
public class AccountModel
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    [BsonElement("email")]
    public string Email { get; set; }
    [BsonElement("password")]
    public string Password { get; set; }
    [BsonElement("salt")]
    public string Salt { get; set; }
    [BsonElement("isVerified")]
    public bool IsVerified { get; set; }
    [BsonElement("isEnabled")]
    public bool IsEnabled { get; set; }
    [BsonElement("isDeleted")]
    public bool IsDeleted { get; set; }
    [BsonElement("roles")]
    public List<string> Roles { get; set; }

    [BsonElement("socialProviders")]
    public List<SocialProvider> SocialProviders { get; set; }
    [BsonElement("created")]
    public DateTime Created { get; set; }
    [BsonElement("modified")]
    public DateTime Modified { get; set; }

    public AccountModel()
    {
        SocialProviders = new List<SocialProvider>();
        Roles = new List<string>();
    }
}

public class SocialProvider
{
    [BsonElement("name")]
    public string Name { get; set; }
    [BsonElement("socialId")]
    public string SocialId { get; set; }
}

