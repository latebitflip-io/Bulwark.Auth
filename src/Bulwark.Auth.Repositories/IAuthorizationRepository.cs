namespace Bulwark.Auth.Repositories;

/// <summary>
/// Data layer for authorizations.
/// </summary>
public interface IAuthorizationRepository
{
    Task<List<string>> ReadAccountPermissions(string accountId);
    Task<List<string>> ReadAccountRoles(string accountId);
}
