using Bulwark.Auth.Repositories.Model;

namespace Bulwark.Auth.Repositories;

public class MongoDbSigningKey : ISigningKeyRepository
{
    private readonly IMongoCollection<SigningKeyModel> _keyCollection;

    public MongoDbSigningKey(IMongoDatabase db)
    {
        _keyCollection = db.GetCollection<SigningKeyModel>("signingKey");
        CreateIndexes();
    }

    private void CreateIndexes()
    { 
        var indexKeysDefine = Builders<SigningKeyModel>
           .IndexKeys
           .Ascending(indexKey => indexKey.KeyId);

        _keyCollection.Indexes.CreateOne(
             new CreateIndexModel<SigningKeyModel>(indexKeysDefine,
             new CreateIndexOptions()
             {
                 Unique = true,
                 Name = "Attr_Unique"
             }));
    }

    public void AddKey(string privateKey, string publicKey, string algorithm = "RS256")
    {
        var newKey = new SigningKeyModel
        {
            Id = ObjectId.GenerateNewId().ToString(),
            KeyId = Guid.NewGuid().ToString(),
            Format = "PKCS#1",
            PrivateKey = privateKey,
            PublicKey = publicKey,
            Algorithm = algorithm,
            Created = DateTime.Now,
        };

        _keyCollection.InsertOne(newKey);
    }

    public SigningKeyModel GetKey(string keyId)
    {
        var key = _keyCollection.AsQueryable()
            .Where(c => c.KeyId == keyId)
            .FirstOrDefault();

        return key;
    }

    public SigningKeyModel GetLatestKey()
    {
        var key = _keyCollection.AsQueryable()
            .OrderByDescending(c => c.Created)
            .FirstOrDefault();

        return key;
    }

    public List<SigningKeyModel> GetAllKeys()
    {
        var keys = _keyCollection.AsQueryable()
           .OrderByDescending(c => c.Created)
           .ToList();

        return keys;
    }
}


