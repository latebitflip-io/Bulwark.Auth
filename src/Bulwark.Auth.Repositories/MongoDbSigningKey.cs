using Bulwark.Auth.Repositories.Model;

namespace Bulwark.Auth.Repositories;

public class MongoDbSigningKey : ISigningKeyRepository
{
    private readonly IMongoCollection<SigningKeyModel> _certCollection;

    public MongoDbSigningKey(IMongoDatabase db)
    {
        _certCollection = db.GetCollection<SigningKeyModel>("signingKeys");
        CreateIndexes();
    }

    private void CreateIndexes()
    { 
        var indexKeysDefine = Builders<SigningKeyModel>
           .IndexKeys
           .Ascending(indexKey => indexKey.Generation);

        _certCollection.Indexes.CreateOne(
             new CreateIndexModel<SigningKeyModel>(indexKeysDefine,
             new CreateIndexOptions()
             {
                 Unique = true,
                 Name = "Attr_Unique"
             }));
    }

    public void AddKey(string privateKey, string publicKey)
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
            Created = DateTime.Now,
        };

        _certCollection.InsertOne(newCert);
    }

    public SigningKeyModel GetKey(int generation)
    {
        var cert = _certCollection.AsQueryable()
            .Where(c => c.Generation == generation)
            .FirstOrDefault();

        return cert;
    }

    public SigningKeyModel GetLatestKey()
    {
        var cert = _certCollection.AsQueryable()
            .OrderByDescending(c => c.Generation)
            .FirstOrDefault();

        return cert;
    }

    public List<SigningKeyModel> GetAllKeys()
    {
        var certs = _certCollection.AsQueryable()
           .OrderByDescending(c => c.Generation)
           .ToList();

        return certs;
    }
}


