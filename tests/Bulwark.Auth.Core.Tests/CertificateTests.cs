using Bulwark.Auth.Core.Util;
using Bulwark.Auth.Repositories;
using Bulwark.Auth.TestFixture;


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
        var cert = RsaKeyGenerator.MakeKey();
        Assert.NotNull(cert);
        Assert.Equal(1, cert.Generation);
    }

    [Fact]
    public void CertManagerInitialize()
    {
        var certRepository = new MongoDbSigningKey(_dbFixture.Db);
        var certManager = new SigningKeyManager(certRepository);
        var certModel = certRepository.GetLatestKey();
        Assert.Equal(1, certModel.Generation);
        Assert.NotNull(certManager.TokenContext);
        certManager.GenerateKey();
        certModel = certRepository.GetLatestKey();
        Assert.Equal(2, certModel.Generation);
    }
}

