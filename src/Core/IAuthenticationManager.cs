using System.Threading.Tasks;
using Bulwark.Auth.Core.Domain;

namespace Bulwark.Auth.Core;

public interface IAuthenticationManager
{
    Task<Authenticated> Renew(string email, string refreshToken,
       string deviceId, string tokenizerName = "default");
    Task<AccessToken> ValidateAccessToken(string email, string accessToken,
        string deviceId);
    Task Acknowledge(Authenticated authenticated,
        string email, string deviceId);
    Task<Authenticated> Authenticate(string email,
        string password, string tokenizerName = "default");
    Task Revoke(string email, string accessToken, string deviceId);
}


