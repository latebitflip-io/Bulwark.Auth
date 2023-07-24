using Bulwark.Auth.Repositories.Model;

namespace Bulwark.Auth.Repositories;

/// <summary>
/// Used to create and use magic codes
/// </summary>
public interface IMagicCodeRepository
{
	Task Add(string userId, string code, DateTime expires);
	Task Delete(string userId, string code);
    Task<MagicCodeModel> Get(string userId, string code);
}


