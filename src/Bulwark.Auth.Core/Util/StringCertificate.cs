namespace Bulwark.Auth.Core.Util;
public class StringCertificate
{
    public string PrivateKey { get; }
    public string PublicKey { get; }

    public StringCertificate(string privateKey, string publicKey)
	{
		PrivateKey = privateKey;
		PublicKey = publicKey;
	}
}


