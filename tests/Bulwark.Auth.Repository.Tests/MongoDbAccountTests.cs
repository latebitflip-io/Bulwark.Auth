using Bulwark.Auth.Repositories.Model;
using Bulwark.Auth.Repositories.Util;
using Bulwark.Auth.TestFixture;


namespace Bulwark.Auth.Repositories.Tests;
public class MongoDbAccountTests : IClassFixture<MongoDbRandomFixture>
{
    private readonly IAccountRepository _accountRepository;
    public MongoDbAccountTests(MongoDbRandomFixture dbFixture)
    {
        var encryption = new BulwarkBCrypt();
        _accountRepository = new MongoDbAccount(dbFixture.Db,
            encryption);
    }

    [Fact]
    public async void CreateAccountTest()
    {
        var user = TestUtils.GenerateEmail();
        try
        {
            var verfication = await
                _accountRepository.Create(user,
                "strongpassword");
            Assert.IsType<VerificationModel>(verfication);
        }
        catch (System.Exception exception)
        {
            Assert.True(false, exception.Message);
        }
    }

    [Fact]
    public async void VerfiyAccountTest()
    {
        var user = TestUtils.GenerateEmail();
       
        try
        {
            var verfication = await
                _accountRepository.Create(user,
                "strongpassword");

            await _accountRepository.Verify(verfication.Email, verfication.Token);
            var account = await _accountRepository.GetAccount(user);
            Assert.True(account.IsEnabled && !account.IsDeleted
                && account.IsVerified);
        }
        catch (System.Exception exception)
        {
            Assert.True(false, exception.Message);
        } 
    }

    [Fact]
    public async void GetAccountTest()
    {
        var user = TestUtils.GenerateEmail();
       
        try
        {
            var verfication = await
                _accountRepository.Create(user,
                "strongpassword");

            var account = await _accountRepository.GetAccount(user);
            Assert.True(account.Email == user);
        }
        catch (System.Exception exception)
        {
            Assert.True(false, exception.Message);
        }
    }

    [Fact]
    public async void DeleteAccountTest()
    {
        var user = TestUtils.GenerateEmail();
       
        try
        {
            var verfication = await
                _accountRepository.Create(user,
                "strongpassword");

            await _accountRepository.Delete(user);
            var account = await _accountRepository.GetAccount(user);
            Assert.True(account.IsDeleted);
        }
        catch (System.Exception exception)
        {
            Assert.True(false, exception.Message);
        }
    }

    [Fact]
    public async void DisableAccountTest()
    {
        var user = TestUtils.GenerateEmail();

        try
        {
            var verfication = await
                _accountRepository.Create(user,
                "strongpassword");

            await _accountRepository.Verify(verfication.Email, verfication.Token);
            await _accountRepository.Disable(user);
            var account = await _accountRepository.GetAccount(user);
            Assert.False(account.IsEnabled);
        }
        catch (System.Exception exception)
        {
            Assert.True(false, exception.Message);
        }
    }

    [Fact]
    public async void EnableAccountTest()
    {
        var user = TestUtils.GenerateEmail();

        try
        {
            var verfication = await
                _accountRepository.Create(user,
                "strongpassword");

            await _accountRepository.Verify(verfication.Email, verfication.Token);
            await _accountRepository.Enable(user);
            var account = await _accountRepository.GetAccount(user);
            Assert.True(account.IsEnabled);
        }
        catch (System.Exception exception)
        {
            Assert.True(false, exception.Message);
        }
    }

    [Fact]
    public async void ChangeEmailTest()
    {
        var user = TestUtils.GenerateEmail();

        try
        {
            var verfication = await
                _accountRepository.Create(user,
                "strongpassword");

            await _accountRepository.Verify(verfication.Email, verfication.Token);
            var newEmail = TestUtils.GenerateEmail();
            await _accountRepository.ChangeEmail(user, newEmail);
            var account = await _accountRepository.GetAccount(newEmail);
            Assert.True(account.Email == newEmail);
        }
        catch (System.Exception exception)
        {
            Assert.True(false, exception.Message);
        }
    }

    [Fact]
    public async void ChangeEmailCollisionTest()
    {
        var user = TestUtils.GenerateEmail();
        var user2 = TestUtils.GenerateEmail();

        try
        {
            var verfication = await
                _accountRepository.Create(user,
                "strongpassword");

            await _accountRepository.Verify(verfication.Email, verfication.Token);
            verfication = await
                _accountRepository.Create(user2,
                "strongpassword");

            await _accountRepository.Verify(verfication.Email, verfication.Token);
            
            await _accountRepository.ChangeEmail(user, user2);

            Assert.True(false);
        }
        catch (System.Exception exception)
        {
            Assert.True(true, exception.Message);
        }
    }

    [Fact]
    public async void ChangePasswordTest()
    {
        var user = TestUtils.GenerateEmail();
        var encrypt = new BulwarkBCrypt();

        try
        {
            var verfication = await
                _accountRepository.Create(user,
                "strongpassword");

            await _accountRepository.Verify(verfication.Email, verfication.Token);
            await _accountRepository.ChangePassword(user, "newstrongpassword");
            var account = await _accountRepository.GetAccount(user);
            Assert.True(encrypt.Verify("newstrongpassword",
                account.Password));
        }
        catch (System.Exception exception)
        {
            Assert.True(false, exception.Message);
        }
    }

    [Fact]
    public async void GetForgotToken()
    {
        var user = TestUtils.GenerateEmail();
        var verfication = await
                _accountRepository.Create(user,
                "strongpassword");

        await _accountRepository.Verify(verfication.Email, verfication.Token);

        var forgotToken = await
            _accountRepository.ForgotPassword(user);
        Assert.True(forgotToken.Email == user);
    }

    [Fact]
    public async void ResetPasswordWithToken()
    {
        var user = TestUtils.GenerateEmail();
        var verfication = await
                _accountRepository.Create(user,
                "strongpassword");

        await _accountRepository.Verify(verfication.Email, verfication.Token);

        var forgotToken = await
            _accountRepository.ForgotPassword(user);
        Assert.True(forgotToken.Email == user);

        var before = await _accountRepository.GetAccount(user);

        await _accountRepository.ResetPasswordWithToken(user,
            forgotToken.Token,
            "newpassword");

        var after = await _accountRepository.GetAccount(user);
        Assert.True(before.Password != after.Password);
    }

    [Fact]
    public async void LinkSocialTest()
    {
        var user = TestUtils.GenerateEmail();

        try
        {
            var verfication = await
                _accountRepository.Create(user,
                "strongpassword");

            await _accountRepository.Verify(verfication.Email, verfication.Token);
            var account = await _accountRepository.GetAccount(user);
            await _accountRepository.LinkSocial(user,new SocialProvider() { Name ="bulwark",
            SocialId = "123456"});
            account = await _accountRepository.GetAccount(user);
            Assert.True(account.SocialProviders.Count > 0);
        }
        catch (System.Exception exception)
        {
            Assert.True(false, exception.Message);
        }
    }
}

