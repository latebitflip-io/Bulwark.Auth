using Bulwark.Auth.Core.Util;
using Bulwark.Auth.Repositories;
using Bulwark.Auth.TestFixture;


namespace Bulwark.Auth.Core.Tests;
public class SigningKeyTests : IClassFixture<MongoDbRandomFixture>
{
    private readonly MongoDbRandomFixture _dbFixture;

    public SigningKeyTests(MongoDbRandomFixture dbFixture)
    {
        _dbFixture = dbFixture;
    }

    [Fact]
    public void CreateKeys()
    {
        var key = RsaKeyGenerator.MakeKey();
        Assert.NotNull(key);
    }

    [Fact]
    public void SigningKeyManagerInitialize()
    {
        var signingRepository = new MongoDbSigningKey(_dbFixture.Db);
        var signingKeyManager = new SigningKeyService(signingRepository);
        var signingKeyModel = signingRepository.GetLatestKey();
        Assert.NotNull(signingKeyModel);
        Assert.NotNull(signingKeyManager.TokenContext);
        signingKeyManager.GenerateKey();
        var signingKeyModel2 = signingRepository.GetLatestKey();
        Assert.NotEqual(signingKeyModel.KeyId, signingKeyModel2.KeyId);
    }
}

