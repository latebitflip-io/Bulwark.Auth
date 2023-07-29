namespace Bulwark.Auth.Core.Util;
public class StringKey
{
    public string PrivateKey { get; }
    public string PublicKey { get; }

    public StringKey(string privateKey, string publicKey)
	{
		PrivateKey = privateKey;
		PublicKey = publicKey;
	}
}


