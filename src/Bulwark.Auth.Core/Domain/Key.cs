using System;
namespace Bulwark.Auth.Core.Domain;

public class Key
{
    public string KeyId { get; set;  }
    public string PrivateKey { get; set; }
    public string PublicKey { get; set; }
    public string Algorithm { get; set; }
    
    public string Format { get; set; }
    public DateTime Created { get; set; }
    
    public Key(string privateKey, string publicKey, string algorithm, string format)
    {
        KeyId = Guid.NewGuid().ToString();
        PrivateKey = privateKey;
        PublicKey = publicKey;
        Algorithm = algorithm;
        Format = format;
        Created = DateTime.UtcNow;
    }
    public Key(string keyId, string privateKey, string publicKey, 
        string algorithm, string format, DateTime created)
    {
        KeyId = keyId;
        PrivateKey = privateKey;
        PublicKey = publicKey;
        Algorithm = algorithm;
        Format = format;
        Created = created;
    }
}


