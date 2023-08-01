using Bulwark.Auth.Repositories.Model;

namespace Bulwark.Auth.Repositories;
/// <summary>
/// Manages Keys for signing JWTs.
/// </summary>
public interface ISigningKeyRepository
{
    void AddKey(string privateKey, string publicKey, string algorithm = "RS256");
    SigningKeyModel GetKey(string keyId);
    SigningKeyModel GetLatestKey();
    List<SigningKeyModel> GetAllKeys();
}

