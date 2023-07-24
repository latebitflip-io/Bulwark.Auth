using Bulwark.Auth.Repositories.Model;

namespace Bulwark.Auth.Repositories;
/// <summary>
/// Data layer for account management.
/// </summary>
public interface IAccountRepository
{
    Task<VerificationModel> Create(string email, string password);
    Task Verify(string email, string verificationToken);
    Task<AccountModel> GetAccount(string email);
    Task Delete(string email); 
    Task Disable(string email);
    Task Enable(string email);
    Task ChangeEmail(string oldEmail, string newEmail);
    Task ChangePassword(string email, string newPassword);
    Task LinkSocial(string email, SocialProvider provider);
    Task<ForgotModel> ForgotPassword(string email);
    Task ResetPasswordWithToken(string email,
        string token, string newPassword);
}

