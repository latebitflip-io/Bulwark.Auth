using Bulwark.Auth.Core.Util;
using Bulwark.Auth.TestFixture;


namespace Bulwark.Auth.Repositories.Tests;

public class MongoDbCertTests : IClassFixture<MongoDbRandomFixture>
{
    private readonly ISigningKeyRepository _signingKeyRepository;
   
    public MongoDbCertTests(MongoDbRandomFixture dbFixture)
    {
        _signingKeyRepository = new MongoDbSigningKey(dbFixture.Db);
    }

    [Fact]
    public void AddAndGetCertsTest()
    {
        var certs = RsaKeyGenerator.MakeStringKey();
        _signingKeyRepository.AddKey(certs.PrivateKey,
            certs.PublicKey);
        var cert = _signingKeyRepository.GetKey(1);
        Assert.Equal(1, cert.Generation);

        certs = RsaKeyGenerator.MakeStringKey();
        _signingKeyRepository.AddKey(certs.PrivateKey,
            certs.PublicKey);
        cert = _signingKeyRepository.GetLatestKey();
        Assert.Equal(2, cert.Generation);

        var allCerts = _signingKeyRepository.GetAllKeys();
        Assert.Equal(2, allCerts.Count);
    }
}


