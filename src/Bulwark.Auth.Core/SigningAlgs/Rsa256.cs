using System.Security.Cryptography;
using JWT.Algorithms;

namespace Bulwark.Auth.Core.SigningAlgs;

public class Rsa256 : ISigningAlgorithm
{
    public string Name { get; }
    public IJwtAlgorithm GetAlgorithm(string privateKey, string publicKey)
    {
        var rsaPrivateKey = RSA.Create();
        rsaPrivateKey.ImportFromPem(privateKey.ToCharArray());
        var rsaPublicKey = RSA.Create();
        rsaPublicKey.ImportFromPem(publicKey.ToCharArray());
        return new RS256Algorithm(rsaPublicKey, rsaPrivateKey);
    }
    
    public Rsa256()
    {
        Name = "RS256";
    }
}