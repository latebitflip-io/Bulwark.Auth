using Bulwark.Auth.Core.Util;
using Bulwark.Auth.Tests;

namespace Bulwark.Auth.Repositories.Tests;

public class MongoDbCertTests : IClassFixture<MongoDbRandomFixture>
{
    private readonly ICertRepository _certRepository;
   
    public MongoDbCertTests(MongoDbRandomFixture dbFixture)
    {
        _certRepository = new MongoDbCert(dbFixture.Db);
    }

    [Fact]
    public void AddAndGetCertsTest()
    {
        var certs = CertificateGenerator.MakeStringCert(1);
        _certRepository.AddCert(certs.PrivateKey,
            certs.PublicKey);
        var cert = _certRepository.GetCert(1);
        Assert.Equal(1, cert.Generation);

        certs = CertificateGenerator.MakeStringCert(1);
        _certRepository.AddCert(certs.PrivateKey,
            certs.PublicKey);
        cert = _certRepository.GetLatestCert();
        Assert.Equal(2, cert.Generation);

        var allCerts = _certRepository.GetAllCerts();
        Assert.Equal(2, allCerts.Count);
    }
}


