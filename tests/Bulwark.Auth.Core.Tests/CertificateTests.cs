using Bulwark.Auth.Core.Util;
using Bulwark.Auth.Repositories;
using Bulwark.Auth.Tests;

namespace Bulwark.Auth.Core.Tests;
public class CertificateTests : IClassFixture<MongoDbRandomFixture>
{
    private readonly MongoDbRandomFixture _dbFixture;

    public CertificateTests(MongoDbRandomFixture dbFixture)
    {
        _dbFixture = dbFixture;
    }

    [Fact]
    public void CreateCertificates()
    {
        var cert = CertificateGenerator.MakeCert(365);
        Assert.NotNull(cert);
        Assert.Equal(1, cert.Generation);
    }

    [Fact]
    public void CertManagerInitialize()
    {
        var certRepository = new MongoDbCert(_dbFixture.Db);
        var certManager = new CertManager(certRepository);
        var certModel = certRepository.GetLatestCert();
        Assert.Equal(1, certModel.Generation);
        Assert.NotNull(certManager.TokenContext);
        certManager.GenerateCertificate(365);
        certModel = certRepository.GetLatestCert();
        Assert.Equal(2, certModel.Generation);
    }
}

