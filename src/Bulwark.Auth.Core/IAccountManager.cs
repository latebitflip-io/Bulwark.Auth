using System.Threading.Tasks;
using Bulwark.Core;

namespace Bulwark.Auth.Core;

public interface IAccountManager
{
    Task<VerificationToken> Create(string email,
        string password);
    Task Verify(string email, string verificationToken);
    Task Delete(string email, string accessToken);
    Task ChangeEmail(string oldEmail, string newEmail,
        string accessToken);
    Task ChangePassword(string email, string newPassword,
        string accessToken);
    Task<string> ForgotPassword(string email);
    Task ResetPasswordWithToken(string email,
        string token, string newPassword);
}

