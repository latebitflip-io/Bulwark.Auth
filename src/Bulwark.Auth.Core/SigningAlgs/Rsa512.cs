namespace Bulwark.Auth.Core.SigningAlgs;

using System.Security.Cryptography;
using JWT.Algorithms;

public class Rsa512 :  ISigningAlgorithm
{
    public string Name { get; }
    public IJwtAlgorithm GetAlgorithm(string privateKey, string publicKey)
    {
        var rsaPrivateKey = RSA.Create();
        rsaPrivateKey.ImportFromPem(privateKey.ToCharArray());
        var rsaPublicKey = RSA.Create();
        rsaPublicKey.ImportFromPem(publicKey.ToCharArray());
        return new RS512Algorithm(rsaPublicKey, rsaPrivateKey);
    }
        
    public Rsa512()
    {
        Name = "RS512";
    }
}