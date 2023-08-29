using System.Collections.Generic;
using System.Linq;
using Bulwark.Auth.Core.Domain;
using Bulwark.Auth.Core.SigningAlgs;
using Bulwark.Auth.Core.Util;
using Bulwark.Auth.Repositories;

namespace Bulwark.Auth.Core;
/// <summary>
/// Cert manager is responsible for generating and storing certificates used for token signing.
/// </summary>
public class SigningKeyService : ISigningKeyService
{
    private readonly ISigningKeyRepository _signingKeyRepository;
    private const string DefaultIssuer = "bulwark";
    public TokenStrategyContext TokenContext { get; }
   
	public SigningKeyService(ISigningKeyRepository signingKeyRepository)
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
        var key = RsaKeyGenerator.MakeKey();
        _signingKeyRepository.AddKey(key.PrivateKey,
            key.PublicKey);
    }

    /// <summary>
    /// Returns all certs used for token signing.
    /// </summary>
    /// <returns></returns>
    public List<Key> GetKeys()
    {
        var keys = _signingKeyRepository.GetAllKeys();

        return keys
            .Select(keyModel => new Key(keyModel.KeyId, keyModel.PrivateKey, keyModel.PublicKey, 
                keyModel.Algorithm, keyModel.Format, keyModel.Created))
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
            var key = RsaKeyGenerator.MakeKey();
            _signingKeyRepository.AddKey(key.PrivateKey,
                key.PublicKey);
        }
        
        var signingAlgorithms = new List<ISigningAlgorithm>
        {
            new Rsa256(),
            new Rsa384(),
            new Rsa512()
        };
        
        var defaultTokenizer = new JwtTokenizer(DefaultIssuer, DefaultIssuer, 
            10,24,
            signingAlgorithms,
            GetKeys().ToArray());

        TokenContext.Add(defaultTokenizer); 
    }
}


