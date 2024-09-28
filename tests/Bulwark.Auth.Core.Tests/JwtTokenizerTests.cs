using System.Collections.Generic;
using Bulwark.Auth.Core.Domain;
using Bulwark.Auth.Core.SigningAlgs;
using Bulwark.Auth.Core.Util;
using Bulwark.Auth.Repositories;
using Bulwark.Auth.TestFixture;
using JWT.Exceptions;
using Xunit.Abstractions;

namespace Bulwark.Auth.Core.Tests;

public class JwtTokenizerTests : IClassFixture<MongoDbRandomFixture>
{
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly JwtTokenizer _tokenizer;

    public JwtTokenizerTests(MongoDbRandomFixture dbFixture, ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
        var signingKey = new SigningKey(new MongoDbSigningKey(dbFixture.Db));
        var key = RsaKeyGenerator.MakeKey();
        var keys = new Key[1];
        keys[0] = key;
        var signingAlgorithms = new List<ISigningAlgorithm>
        {
            new Rsa256()
        };
        _tokenizer = new JwtTokenizer("test", "test", 2,2,
            signingAlgorithms,signingKey);
    }

    [Fact]
    public void CreateAccessTokenAndValidate()
    {
        var roles = new List<string>() { "role1", "role2" };
        var permissions = new List<string>() { "permission1", "permission2" };
        var jwt = _tokenizer.CreateAccessToken("userid", roles, permissions);
        var accessToken = _tokenizer.ValidateAccessToken("userid", jwt);

        Assert.True(accessToken.Sub == "userid");
    }

    [Fact]
    public void CreateRefreshTokenAndValidate()
    {
        var account = Guid.NewGuid().ToString() + "@bulwark.io";
        var jwt = _tokenizer.CreateRefreshToken(account);
        var refreshToken = _tokenizer.ValidateRefreshToken(account,jwt);
        Assert.True(refreshToken.Sub == account);
    }

    [Fact]
    public async Task RefreshTokenAndValidateExpired()
    {
        var account = Guid.NewGuid().ToString() + "@bulwark.io";
        var jwt = _tokenizer.CreateRefreshToken(account);
        await Task.Delay(2500);
        try
        {
            var refreshToken = _tokenizer.ValidateRefreshToken(account, jwt);
            Assert.False(refreshToken.Sub == account);
        }
        catch (TokenExpiredException e)
        {
            _testOutputHelper.WriteLine(e.Message);
            Assert.True(true);
        }
    }
}