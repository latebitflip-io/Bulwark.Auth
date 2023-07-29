using System.Security.Cryptography;

namespace Bulwark.Auth.Core.Domain;

public class Key
{
    public RSA PrivateKey { get; }
    public RSA PublicKey { get; }
    public int Generation { get; }

    public Key(int generation,
        string privateKey, string publicKey )
    {
        Generation = generation;
        PrivateKey = RSA.Create();
        PrivateKey.ImportFromPem(privateKey.ToCharArray());
        PublicKey = RSA.Create();
        PublicKey.ImportFromPem(publicKey.ToCharArray());
    }
}


