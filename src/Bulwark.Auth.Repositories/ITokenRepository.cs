using Bulwark.Auth.Repositories.Model;

namespace Bulwark.Auth.Repositories;
public interface ITokenRepository
{
    Task Delete(string userId, string deviceId);
    Task Delete(string userId);
    Task Acknowledge(string userId, string deviceId, string accessToken,
        string refreshToken);
    Task<AuthTokenModel> Get(string userId, string deviceId);  
}

