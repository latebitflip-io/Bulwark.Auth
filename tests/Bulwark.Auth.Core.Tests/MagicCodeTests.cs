using Bulwark.Auth.Repositories;
using Bulwark.Auth.TestFixture;


namespace Bulwark.Auth.Core.Tests;

[Collection("Sequential")]
public class MagicCodeTests : IClassFixture<MongoDbRandomFixture>
{
    private readonly MongoDbRandomFixture _dbFixture;

    public MagicCodeTests(MongoDbRandomFixture dbFixture)
	{
        _dbFixture = dbFixture;
    }

    [Fact]
    public async void CreateAndAuthenticateMagicCode()
    {
        var encrypt = new BulwarkBCrypt();
        var accountRepository = new MongoDbAccount(_dbFixture.Db,
            encrypt);
        var certRepository = new MongoDbCert(_dbFixture.Db);
        var certManager = new CertManager(certRepository);
        var magicCodeRepository = new MongoDbMagicCode(_dbFixture.Db);
        var authorizationRepository = new MongoDbAuthorization(_dbFixture.Db);
        var magicCodeManager = new MagicCodeManager(magicCodeRepository,
            accountRepository, authorizationRepository, certManager);
        var accountManager = new AccountManager(accountRepository,
            certManager);
        var user = TestUtils.GenerateEmail();
        var verificationToken = await accountManager.Create(user,
            "strongpassword");
        var hasToken = string.IsNullOrEmpty(verificationToken.Value);
        Assert.True(!hasToken);
        await accountManager.Verify(user, verificationToken.Value);

        var code = await magicCodeManager.CreateCode(user, 60);
        Assert.NotNull(code);
        var authenticated = await magicCodeManager.AuthenticateCode(user,
            code);

        Assert.NotNull(authenticated.AccessToken);
    }
}


