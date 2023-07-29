using System.Collections.Generic;
using System.Linq;
using Bulwark.Auth.Core.Domain;
using Bulwark.Auth.Core.Util;
using Bulwark.Auth.Repositories;

namespace Bulwark.Auth.Core;
/// <summary>
/// Cert manager is responsible for generating and storing certificates used for token signing.
/// </summary>
public class SigningKeyManager : ISigningKeyManager
{
    private readonly ISigningKeyRepository _signingKeyRepository;
    private const string DefaultIssuer = "bulwark";
    public TokenStrategyContext TokenContext { get; }
   
	public SigningKeyManager(ISigningKeyRepository signingKeyRepository)
	{
        _signingKeyRepository = signingKeyRepository;
        TokenContext = new TokenStrategyContext();
        Initialize();
    }

    /// <summary>
    /// Will generate a new key and store it in the database.
    /// </summary>
    public void GenerateKey()
    {
        var stringCert = RsaKeyGenerator.MakeStringKey();
        _signingKeyRepository.AddKey(stringCert.PrivateKey,
            stringCert.PublicKey);
    }

    /// <summary>
    /// Returns all certs used for token signing.
    /// </summary>
    /// <returns></returns>
    public List<Key> GetKeys()
    {
        var certModels = _signingKeyRepository.GetAllKeys();

        return certModels
            .Select(certModel => new Key(certModel.Generation, certModel.PrivateKey, certModel.PublicKey))
            .ToList();
    }

    /// <summary>
    /// If no certs are found in the database, a new one will be generated and will expire in a year.
    /// </summary>
    private void Initialize()
    {
        var latestCert = _signingKeyRepository.GetLatestKey();
        if(latestCert == null)
        {
            var stringCert = RsaKeyGenerator.MakeStringKey();
            _signingKeyRepository.AddKey(stringCert.PrivateKey,
                stringCert.PublicKey);
        }

        var defaultTokenizer = new DefaultTokenizer(DefaultIssuer, DefaultIssuer,
            GetKeys().ToArray());

        TokenContext.Add(defaultTokenizer); 
    }
}


