using System.Collections.Generic;
using System.Linq;
using Bulwark.Auth.Core.Domain;
using Bulwark.Auth.Core.Util;
using Bulwark.Auth.Repositories;

namespace Bulwark.Auth.Core;

public class CertManager : ICertManager
{
    private readonly ICertRepository _certRepository;
    public TokenStrategyContext TokenContext { get; }
   
	public CertManager(ICertRepository certRepository)
	{
        _certRepository = certRepository;
        TokenContext = new TokenStrategyContext();
        Initialize();
    }

    public void GenerateCertificate(int days)
    {
        var stringCert = CertificateGenerator.MakeStringCert(days);
        _certRepository.AddCert(stringCert.PrivateKey,
            stringCert.PublicKey);
    }

    public List<Certificate> GetCerts()
    {
        var certModels = _certRepository.GetAllCerts();

        return certModels
            .Select(certModel => new Certificate(certModel.Generation, certModel.PrivateKey, certModel.PublicKey))
            .ToList();
    }

    private void Initialize()
    {
        var latestCert = _certRepository.GetLatestCert();
        if(latestCert == null)
        {
            var stringCert = CertificateGenerator.MakeStringCert(365);
            _certRepository.AddCert(stringCert.PrivateKey,
                stringCert.PublicKey);
        }

        var defaultTokenizer = new DefaultTokenizer("bulwark", "bulwark",
            GetCerts().ToArray());

        TokenContext.Add(defaultTokenizer); 
    }
}


