using Bulwark.Auth.Core.Exception;
using Bulwark.Auth.Repositories;
using Bulwark.Auth.Repositories.Util;
using Bulwark.Auth.TestFixture;


namespace Bulwark.Auth.Core.Tests;

[Collection("Sequential")]
public class AuthenticateTest : IClassFixture<MongoDbRandomFixture>
{
    private readonly IAccountManager _account;
    private readonly AuthenticationManager _authentication;
    private string _user;
    private string _password = "strongpassword";

    public AuthenticateTest(MongoDbRandomFixture dbFixture)
    {
        
        var encrypt = new BulwarkBCrypt();
        var accountRepository = new MongoDbAccount(dbFixture.Db,
            encrypt);
        var certRepository = new MongoDbSigningKey(dbFixture.Db);
        var certManager = new SigningKeyManager(certRepository);
        _account = new AccountManager(accountRepository, certManager);
        var tokenRepository = new MongoDbAuthToken(dbFixture.Db);
        var authorizationRepository = new MongoDbAuthorization(dbFixture.Db);
        _authentication = new AuthenticationManager(
            certManager, tokenRepository, encrypt, accountRepository, authorizationRepository);
    }

    [Fact]
    public async void AuthenticatePassword()
    {
        await CreateAndValidateAccount();
        var authenticated =
            await _authentication.Authenticate(_user, _password);
        Assert.NotNull(authenticated.AccessToken);
    }

    [Fact]
    public async void AuthenticateWithWrongPassword()
    {
        await CreateAndValidateAccount();
        try
        {
            var authenticated =
            await _authentication.Authenticate(_user, "wrongpassword");
        }
        catch(BulwarkAuthenticationException)
        {
            Assert.True(true);
        }
    }

    [Fact]
    public async void AuthenticateAwknowledgePassword()
    {
        await CreateAndValidateAccount();
        var deviceId = Guid.NewGuid().ToString();
        var authenticated =
            await _authentication.Authenticate(_user, _password);
        Assert.NotEmpty(authenticated.AccessToken);

        await _authentication.Acknowledge(authenticated, _user,
            deviceId);
   
        await _authentication.ValidateAccessToken(_user,
            authenticated.AccessToken, deviceId);
    }

    [Fact]
    public async void AuthenticateBadAwknowledgePassword()
    {
        await CreateAndValidateAccount();
        var deviceId = Guid.NewGuid().ToString();
        var authenticated =
            await _authentication.Authenticate(_user, _password);
        Assert.NotEmpty(authenticated.AccessToken);

        await _authentication.Acknowledge(authenticated, _user,
            deviceId);
        try
        {
            await _authentication.ValidateAccessToken(_user,
                authenticated.AccessToken + "jsjsj", deviceId);
            Assert.Fail("Acknowledge should fail");
        }catch(BulwarkTokenException exception)
        {
            Assert.True(true, exception.Message);
        }
    }

    private async Task CreateAndValidateAccount()
    {
        _user = TestUtils.GenerateEmail();
        var verificationToken = await _account.Create(_user,
            _password);
        var hasToken = string.IsNullOrEmpty(verificationToken.Value);
        await _account.Verify(_user, verificationToken.Value);
    }

    [Fact]
    public async void RenewAuthentication()
    {
        await CreateAndValidateAccount();
        var deviceId = Guid.NewGuid().ToString();
        var authenticated =
            await _authentication.Authenticate(_user, _password);
        Assert.NotNull(authenticated.AccessToken);

        await _authentication.Acknowledge(authenticated, _user,
            deviceId);

        await _authentication.ValidateAccessToken(_user,
            authenticated.AccessToken, deviceId);

        authenticated = await _authentication.Renew(_user,
            authenticated.RefreshToken,
            deviceId);

        Assert.NotNull(authenticated.AccessToken);

        try
        {
            await _authentication.ValidateAccessToken(_user,
                authenticated.AccessToken, deviceId);
            Assert.Fail("Should be awknowledged first.");
        }
        catch(BulwarkTokenException exception)
        {
            Assert.True(true, exception.Message);
        }

        await _authentication.Acknowledge(authenticated, _user,
           deviceId);

        await _authentication.ValidateAccessToken(_user,
           authenticated.AccessToken, deviceId);
    }

    [Fact]
    public async void Revoke()
    {
        await CreateAndValidateAccount();
        var deviceId = Guid.NewGuid().ToString();
        var authenticated =
            await _authentication.Authenticate(_user, _password);
        Assert.NotNull(authenticated.AccessToken);

        await _authentication.Acknowledge(authenticated, _user,
            deviceId);

        await _authentication.ValidateAccessToken(_user,
            authenticated.AccessToken, deviceId);
        
        await _authentication.Revoke(_user,
            authenticated.AccessToken,
            deviceId);
        try
        {
            await _authentication.ValidateAccessToken(_user,
                authenticated.AccessToken, deviceId);

        }
        catch(BulwarkTokenException exception)
        {
            Assert.True(true, exception.Message);
        }
    }
}


