using Bulwark.Auth.Repositories.Model;

namespace Bulwark.Auth.Repositories;
public interface ICertRepository
{
    void AddCert(string privateKey, string publicKey);
    void DeleteCert(int generation);
    CertModel GetCert(int generation);
    CertModel GetLatestCert();
    List<CertModel> GetAllCerts();
}

