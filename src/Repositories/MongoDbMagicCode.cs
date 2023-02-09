using Bulwark.Auth.Repositories;
using Bulwark.Auth.Repositories.Exception;
using Bulwark.Auth.Repositories.Model;

namespace Bulwark.Repositories;
public class MongoDbMagicCode : IMagicCodeRepository
{
    private readonly IMongoCollection<MagicCodeModel> _magicCodeCollection;

    public MongoDbMagicCode(IMongoDatabase db)
    { 
        _magicCodeCollection = db.GetCollection<MagicCodeModel>("magicCode");
        CreateIndexes();
    }

    public async Task Add(string userId, string code,
        DateTime expires)
    {
        var magicCode = new MagicCodeModel()
        {
            Id = new ObjectId().ToString(),
            UserId = userId,
            Code = code,
            Expires = expires,
            Created = DateTime.Now
        };

        var update = Builders<MagicCodeModel>.Update
               .Set(c => c.Code, magicCode.Code)
               .Set(c => c.Expires, magicCode.Expires)
               .Set(c => c.Created, magicCode.Created);

        var result = await _magicCodeCollection.
            UpdateOneAsync(c => c.UserId == magicCode.UserId,
            update,new UpdateOptions(){ IsUpsert = true });
    }

    public async Task Delete(string userId, string code)
    {
        var result = await _magicCodeCollection
            .DeleteOneAsync(mc => mc.UserId == userId
            && mc.Code == code);

        if (result.DeletedCount < 1)
        {
            throw new BulwarkDbException("Could not delete magic code");
        }
    }

    public Task<MagicCodeModel> Get(string userId, string code)
    {
        var magicCodeModel = _magicCodeCollection
           .AsQueryable<MagicCodeModel>()
           .Where(mc => mc.UserId == userId &&
            mc.Code == code)
           .FirstOrDefaultAsync();

        return magicCodeModel;
    }

    private void CreateIndexes()
    {
        var indexKeysDefine = Builders<MagicCodeModel>
           .IndexKeys
           .Ascending(indexKey => indexKey.UserId);

        _magicCodeCollection.Indexes.CreateOne(
            new CreateIndexModel<MagicCodeModel>(indexKeysDefine,
            new CreateIndexOptions() { Unique = true, Name = "UserId_Unique" }));
    }
}


