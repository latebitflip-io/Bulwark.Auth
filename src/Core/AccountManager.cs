using System.Threading.Tasks;
using Bulwark.Auth.Core.Domain;
using Bulwark.Auth.Core.Exception;
using Bulwark.Auth.Repositories;
using Bulwark.Auth.Repositories.Exception;
using Bulwark.Core;

namespace Bulwark.Auth.Core;
public class AccountManager : IAccountManager
{
    private readonly IAccountRepository _accountRepository;
    private readonly TokenStrategyContext _tokenStrategy;

    public AccountManager(IAccountRepository accountRepository,
        ICertManager certManager)
    {
        _accountRepository = accountRepository;
        _tokenStrategy = certManager.TokenContext;
    }

    public async Task<VerificationToken> Create(string email,
        string password)
    {
        try
        {
            var verificationModel = await _accountRepository.Create(email,
                password);

            return new VerificationToken(verificationModel.Token,
                verificationModel.Created);
        }
        catch (BulwarkDbDuplicateException exception)
        {
            throw new BulwarkAccountException($"Email: {email} in use", exception);
        }
        catch(BulwarkDbException exception)
        {
            throw new BulwarkAccountException("Cannot create account", exception);
        }
    }

    public async Task Verify(string email, string verificationToken)
    {
        try
        {
            await _accountRepository.Verify(email, verificationToken);
        }
        catch (BulwarkDbException exception)
        {
            throw new BulwarkAccountException($"Cannot verify account: {email}", exception);
        }
    }

    public async Task Delete(string email, string accessToken)
    {
        try
        {
            var token = await ValidAccessToken(email, accessToken);
            if (token != null)
            {
                await _accountRepository.Delete(email);
            }
        }
        catch (BulwarkDbException exception)
        {
            throw new BulwarkAccountException($"Cannot delete account: {email}", exception);
        }
    }

    public async Task ChangeEmail(string email, string newEmail,
        string accessToken)
    {
        try
        { 
             var token = await ValidAccessToken(email, accessToken);
             if (token != null)
             {
                 await _accountRepository.ChangeEmail(email, newEmail);
             }
        }
        catch (BulwarkDbDuplicateException exception)
        {
            throw new BulwarkAccountException($"Email: {email} in use");
        }
        catch (BulwarkDbException exception)
        {
            throw new BulwarkAccountException($"Cannot change email for account: ${email}", exception);
        }
    }

    public async Task ChangePassword(string email, string newPassword,
        string accessToken)
    {
        try
        {
            var token = await ValidAccessToken(email, accessToken);
            if (token != null)
            {
                await _accountRepository.ChangePassword(email, newPassword);
            }
        }
        catch (BulwarkDbException exception)
        {
            throw new BulwarkAccountException($"Cannot change password for account: {email}", exception);
        }
    }

    public async Task<string> ForgotPassword(string email)
    {
        try
        {
            var model = await _accountRepository.ForgotPassword(email);
            return model.Token;
        }
        catch (BulwarkDbException exception)
        {
            throw new BulwarkAccountException($"Cannot generate forgot token: {email}", exception);
        }
    }
    
    public async Task ResetPasswordWithToken(string email,
        string token, string newPassword)
    {
        try
        {
            await _accountRepository.ResetPasswordWithToken(email,
                token, newPassword);
        }
        catch (BulwarkDbException exception)
        {
            throw new BulwarkAccountException($"Cannot reset password: {email}");
        }
    }

    private async Task<AccessToken> ValidAccessToken(string email, string accessToken)
    {
        var account = await _accountRepository.GetAccount(email);
        var token = _tokenStrategy.ValidateAccessToken(account.Id,
            accessToken);
        return token;
    }
}