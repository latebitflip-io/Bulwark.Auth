using Bulwark.Auth.Repositories.Model;

namespace Bulwark.Auth.Repositories;
/// <summary>
/// Manages certificates for signing JWTs.
/// </summary>
public interface ICertRepository
{
    void AddCert(string privateKey, string publicKey);
    CertModel GetCert(int generation);
    CertModel GetLatestCert();
    List<CertModel> GetAllCerts();
}

