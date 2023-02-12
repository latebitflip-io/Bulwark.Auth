namespace Bulwark.Auth.Repositories.Model;
public class CertModel
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    [BsonElement("generation")]
    public int Generation { get; set; }
    [BsonElement("privateKey")]
    public string PrivateKey { get; set; }
    [BsonElement("publicKey")]
    public string PublicKey { get; set; }
    [BsonElement("created")]
    public DateTime Created { get; set; }
}

