namespace Bulwark.Auth.Repositories.Util;

public interface IEncrypt
{
    string Encrypt(string value, string salt = null);
    bool Verify(string value, string hash, string salt = null);
}

