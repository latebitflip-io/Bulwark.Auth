using Bulwark.Auth.Repositories.Util;

namespace Bulwark.Auth.Repositories.Tests;
public class BulwarkBCryptTests
{
    [Fact]
    public void EncryptVerifyTest()
    {
        string aValue = "string";

        BulwarkBCrypt bCrypt
            = new();

        var hash = bCrypt.Encrypt(aValue);
        Assert.True(bCrypt.Verify(aValue, hash));
    }
}

