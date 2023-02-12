using System;
using System.Collections.Generic;
using Bulwark.Auth.Core;
using Bulwark.Auth.Core.Domain;
using Bulwark.Auth.Core.Util;
using Xunit;

namespace Bulwark.Token.Tests;

public class TokenStrategyContextTests
{
    private readonly TokenStrategyContext _tokenStrategy;

    public TokenStrategyContextTests()
    {
        var cert = CertificateGenerator.MakeCert(1);
        var certificates = new Certificate[1];
        certificates[0] = cert;
        var defaultTokenizer = new DefaultTokenizer("test", "test", certificates);
        _tokenStrategy = new TokenStrategyContext();
        _tokenStrategy.Add(defaultTokenizer);
    }

    [Fact]
    public void CreateAccessTokenAndValidate()
    {
        var roles = new List<string>() { "role1", "role2" };
        var permissions = new List<string>() { "permission1", "permission2" };
        var jwt = _tokenStrategy.CreateAccessToken("userid", roles, permissions);
        var accessToken = _tokenStrategy.ValidateAccessToken("userid", jwt);
        
        Assert.True(accessToken.Sub == "userid");
    }

    [Fact]
    public void CreateRefreshTokenAndValidate()
    {
        var account = Guid.NewGuid().ToString() + "@bulwark.io";
        var jwt = _tokenStrategy.CreateRefreshToken(account);
        var refreshToken = _tokenStrategy.ValidateRefreshToken(account,jwt);
        Assert.True(refreshToken.Sub == account);
    }
}

