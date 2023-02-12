using System.Linq;
using Bulwark.Auth.Repositories.Exception;
using Bulwark.Auth.Repositories.Model;

namespace Bulwark.Auth.Repositories;
/// <summary>
/// Bulwark.Auth.Repositories for reading and writing roles and permissions
/// </summary>
public class MongoDbAuthorization : IAuthorizationRepository
{
    private readonly IMongoCollection<RoleModel> _mongoRoleCollection;
    private readonly IMongoCollection<AccountModel> _mongoAccountCollection;

    /// <summary>
    /// Constructor for MongoDbAuthorization
    /// </summary>
    /// <param name="db">MongoDb database to use</param>
    public MongoDbAuthorization(IMongoDatabase db)
    {
        _mongoRoleCollection = 
            db.GetCollection<RoleModel>("role");
        _mongoAccountCollection =
            db.GetCollection<AccountModel>("account");
    }
    /// <summary>
    /// Async - Get permissions assigned to roles that are assigned to an account
    /// </summary>
    /// <param name="accountId">The database Id of the account</param>
    /// <returns>a list of permissions as strings, this is easier for JWT claim</returns>
    /// <exception cref="BulwarkDbException"></exception>
    public async Task<List<string>> ReadAccountPermissions(string accountId)
    {
        var accountModel = await _mongoAccountCollection
            .Find(x => x.Id == accountId)
            .FirstOrDefaultAsync();

        if (accountModel == null)
        {
            throw new BulwarkDbException($"Account - {accountId} not found");
        }

        var roles = await _mongoRoleCollection
            .Find(x => accountModel.Roles.Contains(x.Id)) 
            .ToListAsync();

        var permissions = new List<string>();
        foreach (var role in roles)
        {
            permissions
                .AddRange(role.Permissions.Select(x => x.ToString()));
        }

        return permissions.Distinct().ToList();

    }
    /// <summary>
    /// Async - Read all the roles for an account
    /// </summary>
    /// <param name="accountId">Database Id of the account</param>
    /// <returns>List of roles as strings this is easier for JWT claim</returns>
    /// <exception cref="BulwarkDbException"></exception>
    public async Task<List<string>> ReadAccountRoles(string accountId)
    {
        var accountModel = await _mongoAccountCollection
            .Find(x => x.Id == accountId)
            .FirstOrDefaultAsync();

        if (accountModel == null)
        {
            throw new BulwarkDbException($"Account - {accountId} not found");
        }

        var roles = await _mongoRoleCollection
            .Find(x => accountModel.Roles.Contains(x.Id))
            .ToListAsync();

        return roles.Select(x => x.Name).ToList();
    }
}