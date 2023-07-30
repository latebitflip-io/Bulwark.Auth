using System.Security.Cryptography;

namespace Bulwark.Auth.Core.Domain;

public class Key
{
    public string PrivateKey { get; }
    public string PublicKey { get; }
    public int Generation { get; }
    public string Algorithm { get; }

    public Key(int generation,
        string privateKey, string publicKey, string algorithm)
    {
        Algorithm = algorithm;
        Generation = generation;
        PrivateKey = privateKey;
        PublicKey = publicKey;
    }
}


