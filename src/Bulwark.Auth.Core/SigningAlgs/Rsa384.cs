namespace Bulwark.Auth.Core.SigningAlgs;

using System.Security.Cryptography;
using JWT.Algorithms;


public class Rsa384 : ISigningAlgorithm
{
    public string Name { get; }
    public IJwtAlgorithm GetAlgorithm(string privateKey, string publicKey)
    {
        var rsaPrivateKey = RSA.Create();
        rsaPrivateKey.ImportFromPem(privateKey.ToCharArray());
        var rsaPublicKey = RSA.Create();
        rsaPublicKey.ImportFromPem(publicKey.ToCharArray());
        return new RS384Algorithm(rsaPublicKey, rsaPrivateKey);
    }
    
    public Rsa384()
    {
        Name = "RS384";
    }
}