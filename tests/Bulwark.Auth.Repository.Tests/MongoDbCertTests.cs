using Bulwark.Auth.Core.Util;
using Bulwark.Auth.TestFixture;


namespace Bulwark.Auth.Repositories.Tests;

public class MongoDbSigningKeyTests : IClassFixture<MongoDbRandomFixture>
{
    private readonly ISigningKeyRepository _signingKeyRepository;
   
    public MongoDbSigningKeyTests(MongoDbRandomFixture dbFixture)
    {
        _signingKeyRepository = new MongoDbSigningKey(dbFixture.Db);
    }

    [Fact]
    public void AddAndGetKeyTest()
    {
        var newKey = RsaKeyGenerator.MakeKey();
        _signingKeyRepository.AddKey(newKey.PrivateKey,
            newKey.PublicKey);
        var key = _signingKeyRepository.GetLatestKey();
        Assert.Equal(newKey.PrivateKey, key.PrivateKey);

        var newKey2 = RsaKeyGenerator.MakeKey();
        _signingKeyRepository.AddKey(newKey2.PrivateKey,
            newKey2.PublicKey);
        var key2 = _signingKeyRepository.GetLatestKey();
        Assert.NotEqual(key.KeyId, key2.KeyId);

        var allKeys = _signingKeyRepository.GetAllKeys();
        Assert.Equal(2, allKeys.Count);
    }
}


