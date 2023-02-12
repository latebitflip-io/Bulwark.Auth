using Bulwark.Auth.Repositories.Exception;
using Bulwark.Auth.Repositories.Model;

namespace Bulwark.Auth.Repositories;
public class MongoDbAuthToken : ITokenRepository
{
    private readonly IMongoCollection<AuthTokenModel> _authTokenCollection;
   
    public MongoDbAuthToken(IMongoDatabase db)
    {
        _authTokenCollection = db.GetCollection<AuthTokenModel>("authToken");
        CreateIndexes();
    }

    
    public async Task<AuthTokenModel> Get(string userId, string deviceId)
    {
        var authToken = _authTokenCollection.AsQueryable<AuthTokenModel>()
             .Where(r => r.UserId == userId &&
             r.DeviceId == deviceId)
             .FirstOrDefaultAsync();

        return await authToken;
    }


    public async Task Delete(string userId, string deviceId)
    {
        var result = await _authTokenCollection
            .DeleteOneAsync(a => a.UserId == userId
            && a.DeviceId == deviceId);

        if (result.DeletedCount != 1)
        {
            throw new BulwarkDbException("Could not delete token");
        }
    }

    public async Task Delete(string userId)
    {
        var result = await _authTokenCollection
            .DeleteManyAsync(a => a.UserId == userId);

        if (result.DeletedCount < 1)
        {
            throw new BulwarkDbException("Could not delete tokens");
        }
    }

    private void CreateIndexes()
    {
        var indexKeysDefine = Builders<AuthTokenModel>
           .IndexKeys
           .Ascending(indexKey => indexKey.UserId)
           .Ascending(indexKey => indexKey.DeviceId);

        _authTokenCollection.Indexes.CreateOne(
             new CreateIndexModel<AuthTokenModel>(indexKeysDefine,
             new CreateIndexOptions() { Unique = true,
                 Name = "Attr_Unique" }));
    }

    //To make use of JWT the token must be acknowledged, once acknowledged it will
    //invalidate all inflight tokens, there can be more one token per device per user
    public async Task Acknowledge(string userId, string deviceId,
        string accessToken, string refreshToken)
    {
        var now = DateTime.Now;
        var update = Builders<AuthTokenModel>.Update
              .Set(p => p.AccessToken, accessToken)
              .Set(p => p.RefreshToken, refreshToken)
              .Set(p => p.Modified, now)
              .SetOnInsert(p => p.Created, now);
            
        var updateOptions = new UpdateOptions() { IsUpsert = true };

        var result = await _authTokenCollection.
            UpdateOneAsync(a => a.UserId == userId &&
            a.DeviceId == deviceId,
            update, updateOptions);

        if (!(result.ModifiedCount == 1 || result.UpsertedId != null))
        {
            throw
                new BulwarkDbException("Acknowledgement failed");
        }
    }
}

