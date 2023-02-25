

using Bulwark.Auth.TestFixture;

namespace Bulwark.Auth.Repositories.Tests;

public class MongoDbMagicCodeTests : IClassFixture<MongoDbRandomFixture>
{
    private readonly IMagicCodeRepository _magicCodeRepository;
    
    public MongoDbMagicCodeTests(MongoDbRandomFixture dbFixture)
    {
        _magicCodeRepository = new MongoDbMagicCode(dbFixture.Db);
    }

    [Fact]
    public async void AddGetDelete()
    {
        try
        {
            var userId = ObjectId.GenerateNewId();
            var code = "008009";
            await _magicCodeRepository.Add(userId.ToString(), code,
                DateTime.Now.AddMinutes(2));
            var doc = await _magicCodeRepository.Get(userId.ToString(),
               code);
            Assert.True(doc.Code == code);
            Assert.True(doc.UserId == userId.ToString());
            await _magicCodeRepository.Delete(userId.ToString(),
                code);
        }
        catch (System.Exception exception)
        {
            Assert.True(false, exception.Message);
        }
    }
}


