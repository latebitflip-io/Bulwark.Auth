using Bulwark.Auth.Repositories.Exception;
using Bulwark.Auth.Repositories.Model;

namespace Bulwark.Auth.Repositories;

public class MongoDbCert : ICertRepository
{
    private readonly IMongoCollection<CertModel> _certCollection;

    public MongoDbCert(IMongoDatabase db)
    {
        _certCollection = db.GetCollection<CertModel>("certs");
        CreateIndexes();
    }

    private void CreateIndexes()
    {
        var indexKeysDefine = Builders<CertModel>
           .IndexKeys
           .Ascending(indexKey => indexKey.Generation);

        _certCollection.Indexes.CreateOne(
             new CreateIndexModel<CertModel>(indexKeysDefine,
             new CreateIndexOptions()
             {
                 Unique = true,
                 Name = "Attr_Unique"
             }));
    }

    public void AddCert(string privateKey, string publicKey)
    {
        var latest = GetLatestCert();
        var generation = 1;

        if(latest != null)
        {
            generation = latest.Generation + 1;
        }

        var newCert = new CertModel
        {
            Id = ObjectId.GenerateNewId().ToString(),
            Generation = generation,
            PrivateKey = privateKey,
            PublicKey = publicKey,
            Created = DateTime.Now,
        };

        _certCollection.InsertOne(newCert);
    }

    public CertModel GetCert(int generation)
    {
        var cert = _certCollection.AsQueryable()
            .Where(c => c.Generation == generation)
            .FirstOrDefault();

        return cert;
    }

    public CertModel GetLatestCert()
    {
        var cert = _certCollection.AsQueryable()
            .OrderByDescending(c => c.Generation)
            .FirstOrDefault();

        return cert;
    }

    public void DeleteCert(int generation)
    {
        var result = _certCollection
           .DeleteOne(a => a.Generation == generation);

        if (result.DeletedCount != 1)
        {
            throw new BulwarkDbException("Could not delete cert");
        }
    }

    public List<CertModel> GetAllCerts()
    {
        var certs = _certCollection.AsQueryable()
           .OrderByDescending(c => c.Generation)
           .ToList();

        return certs;
    }
}


