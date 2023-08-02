namespace Bulwark.Auth.Repositories.Model;
public class SigningKeyModel
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    [BsonElement("keyId")]
    public string KeyId { get; init; }
    [BsonElement("format")]
    public string Format { get; init; }
    [BsonElement("algorithm")]
    public string Algorithm { get; init; }
    [BsonElement("privateKey")]
    public string PrivateKey { get; init; }
    [BsonElement("publicKey")]
    public string PublicKey { get; init; }
    [BsonElement("created")]
    public DateTime Created { get; set; }
}

