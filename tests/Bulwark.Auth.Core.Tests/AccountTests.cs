using Bulwark.Auth.Repositories;
using Bulwark.Auth.Repositories.Util;
using Bulwark.Auth.TestFixture;


namespace Bulwark.Auth.Core.Tests;

[Collection("Sequential")]
public class AccountTests : IClassFixture<MongoDbRandomFixture>
{
    private readonly IAccountManager _account;
    private readonly AuthenticationManager _authentication;

    public AccountTests(MongoDbRandomFixture dbFixture)
    {
         var encrypt = new BulwarkBCrypt();
         var accountRepository = new MongoDbAccount(dbFixture.Db, encrypt);
         var certRepository = new MongoDbSigningKey(dbFixture.Db);
         var certManager = new SigningKeyManager(certRepository);
        _account = new AccountManager(accountRepository,certManager);
         var tokenRepository = new MongoDbAuthToken(dbFixture.Db);
        var authorizationRepository = new MongoDbAuthorization(dbFixture.Db);
        _authentication = new AuthenticationManager(
            certManager, tokenRepository, encrypt, accountRepository, authorizationRepository);
    }

    [Fact]
    public async void CreateAccountAndValidate()
    {
        var user = TestUtils.GenerateEmail();
        var verificationToken = await _account.Create(user,
            "strongpassword");
        var hasToken = string.IsNullOrEmpty(verificationToken.Value);
        Assert.True(!hasToken);
        await _account.Verify(user, verificationToken.Value);
    }

    [Fact]
    public async void DeleteAccount()
    {
        var password = "strongpassword";
        var user = TestUtils.GenerateEmail();
        var verificationToken = await _account.Create(user,
            password);
        var hasToken = string.IsNullOrEmpty(verificationToken.Value);
        Assert.True(!hasToken);
        await _account.Verify(user, verificationToken.Value);
        var auth = await _authentication.Authenticate(user, password);
        await _account.Delete(user, auth.AccessToken);
    }

    [Fact]
    public async void ChangeEmail()
    {
        var password = "strongpassword";
        var user = TestUtils.GenerateEmail();
        var newEmail = TestUtils.GenerateEmail();
        var verificationToken = await _account.Create(user,
            password);
        var hasToken = string.IsNullOrEmpty(verificationToken.Value);
        Assert.True(!hasToken);
        await _account.Verify(user, verificationToken.Value);
        var auth = await _authentication.Authenticate(user, password);
        await _account.ChangeEmail(user, newEmail, auth.AccessToken);
    }

    [Fact]
    public async void ChangePassword()
    {
        var password = "strongpassword";
        var user = TestUtils.GenerateEmail();
        var verificationToken = await _account.Create(user,
            password);
        bool hasToken = string.IsNullOrEmpty(verificationToken.Value);
        Assert.True(!hasToken);
        await _account.Verify(user, verificationToken.Value);
        var auth = await _authentication.Authenticate(user, password);
        await _account.ChangePassword(user, "newpassword"
            ,auth.AccessToken);
    }

    [Fact]
    public async void ForgotPassword()
    {
        var password = "strongpassword";
        var newPassword = "resetpasswordwithtoken";
        var user = TestUtils.GenerateEmail();
        var verificationToken = await _account.Create(user,
            password);
        var hasToken = string.IsNullOrEmpty(verificationToken.Value);
        Assert.True(!hasToken);
        await _account.Verify(user, verificationToken.Value);
        var forgotToken = await _account.ForgotPassword(user);
        Assert.NotNull(forgotToken);
        await _account.ResetPasswordWithToken(user, forgotToken,
          newPassword);
        var auth = await _authentication.Authenticate(user, newPassword);
        Assert.NotNull(auth);
    }
}

