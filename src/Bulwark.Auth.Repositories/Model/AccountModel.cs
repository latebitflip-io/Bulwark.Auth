namespace Bulwark.Auth.Repositories.Model;

[BsonIgnoreExtraElements]
public class AccountModel
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; init; }
    [BsonElement("email")]
    public string Email { get; init; }
    [BsonElement("password")]
    public string Password { get; init; }
    [BsonElement("salt")]
    public string Salt { get; set; }
    [BsonElement("isVerified")]
    public bool IsVerified { get; init; }
    [BsonElement("isEnabled")]
    public bool IsEnabled { get; init; }
    [BsonElement("isDeleted")]
    public bool IsDeleted { get; init; }
    [BsonElement("roles")]
    public List<string> Roles { get; set; } = new();

    [BsonElement("socialProviders")]
    public List<SocialProvider> SocialProviders { get; set; } = new();

    [BsonElement("created")]
    public DateTime Created { get; set; }
    [BsonElement("modified")]
    public DateTime Modified { get; init; }
}

public class SocialProvider
{
    [BsonElement("name")]
    public string Name { get; init; }
    [BsonElement("socialId")]
    public string SocialId { get; init; }
}

