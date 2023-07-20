using System.Collections.Generic;
using System.Linq;
using Bulwark.Auth.Core.Domain;
using Bulwark.Auth.Core.Util;
using Bulwark.Auth.Repositories;

namespace Bulwark.Auth.Core;
/// <summary>
/// Cert manager is responsible for generating and storing certificates used for token signing.
/// </summary>
public class CertManager : ICertManager
{
    private readonly ICertRepository _certRepository;
    private const string DefaultIssuer = "bulwark";
    private const int DefaultCertExpiration = 365;
    public TokenStrategyContext TokenContext { get; }
   
	public CertManager(ICertRepository certRepository)
	{
        _certRepository = certRepository;
        TokenContext = new TokenStrategyContext();
        Initialize();
    }

    /// <summary>
    /// Will generate a new certificate and store it in the database.
    /// </summary>
    /// <param name="days">How many days until the cert expires</param>
    public void GenerateCertificate(int days)
    {
        var stringCert = CertificateGenerator.MakeStringCert(days);
        _certRepository.AddCert(stringCert.PrivateKey,
            stringCert.PublicKey);
    }

    /// <summary>
    /// Returns all certs used for token signing.
    /// </summary>
    /// <returns></returns>
    public List<Certificate> GetCerts()
    {
        var certModels = _certRepository.GetAllCerts();

        return certModels
            .Select(certModel => new Certificate(certModel.Generation, certModel.PrivateKey, certModel.PublicKey))
            .ToList();
    }

    /// <summary>
    /// If no certs are found in the database, a new one will be generated and will expire in a year.
    /// </summary>
    private void Initialize()
    {
        var latestCert = _certRepository.GetLatestCert();
        if(latestCert == null)
        {
            var stringCert = CertificateGenerator.MakeStringCert(DefaultCertExpiration);
            _certRepository.AddCert(stringCert.PrivateKey,
                stringCert.PublicKey);
        }

        var defaultTokenizer = new DefaultTokenizer(DefaultIssuer, DefaultIssuer,
            GetCerts().ToArray());

        TokenContext.Add(defaultTokenizer); 
    }
}


