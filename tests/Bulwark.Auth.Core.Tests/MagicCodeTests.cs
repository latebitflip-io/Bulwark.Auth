using System.Collections.Generic;
using Bulwark.Auth.Core.SigningAlgs;
using Bulwark.Auth.Repositories;
using Bulwark.Auth.Repositories.Util;
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
        var accountRepository = new MongoDbAccount(_dbFixture.Client, _dbFixture.Db,
            encrypt);
        var signingKeyRepository = new MongoDbSigningKey(_dbFixture.Db);
        var signingKey = new SigningKey(signingKeyRepository);
        var jwtTokenizer = new JwtTokenizer("test", "test", 10, 24,
            new List<ISigningAlgorithm> {new Rsa256()}, signingKey);
        var magicCodeRepository = new MongoDbMagicCode(_dbFixture.Db);
        var authorizationRepository = new MongoDbAuthorization(_dbFixture.Db);
        var magicCodeManager = new MagicCode(magicCodeRepository,
            accountRepository, authorizationRepository, jwtTokenizer);
        var accountManager = new Account(accountRepository,
            jwtTokenizer);
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


