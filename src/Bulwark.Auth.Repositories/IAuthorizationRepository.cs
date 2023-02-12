namespace Bulwark.Auth.Repositories;

public interface IAuthorizationRepository
{
    Task<List<string>> ReadAccountPermissions(string userId);
    Task<List<string>> ReadAccountRoles(string userId);
}
