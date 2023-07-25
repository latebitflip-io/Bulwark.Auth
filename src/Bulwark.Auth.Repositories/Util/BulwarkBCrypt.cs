using BCryptNet = BCrypt.Net.BCrypt;

namespace Bulwark.Auth.Repositories.Util;

public class BulwarkBCrypt : IEncrypt
{
    public string Encrypt(string value, string salt = null)
    {
        return BCryptNet.HashPassword(value);
    }

    public bool Verify(string value, string hash, string salt = null)
    {
        return BCryptNet.Verify(value, hash);
    }
}