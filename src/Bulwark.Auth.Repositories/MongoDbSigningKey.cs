using Bulwark.Auth.Repositories.Model;

namespace Bulwark.Auth.Repositories;

public class MongoDbSigningKey : ISigningKeyRepository
{
    private readonly IMongoCollection<SigningKeyModel> _keyCollection;

    public MongoDbSigningKey(IMongoDatabase db)
    {
        _keyCollection = db.GetCollection<SigningKeyModel>("signingKeys");
        CreateIndexes();
    }

    private void CreateIndexes()
    { 
        var indexKeysDefine = Builders<SigningKeyModel>
           .IndexKeys
           .Ascending(indexKey => indexKey.Generation);

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
        var latest = GetLatestKey();
        var generation = 1;

        if(latest != null)
        {
            generation = latest.Generation + 1;
        }

        var newCert = new SigningKeyModel
        {
            Id = ObjectId.GenerateNewId().ToString(),
            Generation = generation,
            PrivateKey = privateKey,
            PublicKey = publicKey,
            Algorithm = algorithm,
            Created = DateTime.Now,
        };

        _keyCollection.InsertOne(newCert);
    }

    public SigningKeyModel GetKey(int generation)
    {
        var key = _keyCollection.AsQueryable()
            .Where(c => c.Generation == generation)
            .FirstOrDefault();

        return key;
    }

    public SigningKeyModel GetLatestKey()
    {
        var cert = _keyCollection.AsQueryable()
            .OrderByDescending(c => c.Generation)
            .FirstOrDefault();

        return cert;
    }

    public List<SigningKeyModel> GetAllKeys()
    {
        var keys = _keyCollection.AsQueryable()
           .OrderByDescending(c => c.Generation)
           .ToList();

        return keys;
    }
}


