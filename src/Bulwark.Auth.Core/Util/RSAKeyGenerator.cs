using System;
using System.Security.Cryptography;
using Bulwark.Auth.Core.Domain;

namespace Bulwark.Auth.Core.Util;

public static class RsaKeyGenerator
{
    public static Key MakeKey()
    {
        const string privateKeyHeader = "-----BEGIN RSA PRIVATE KEY-----\n";
        const string privateKeyFooter = "\n-----END RSA PRIVATE KEY-----";

        const string publicKeyHeader = "-----BEGIN RSA PUBLIC KEY-----\n";
        const string publicKeyFooter = "\n-----END RSA PUBLIC KEY-----";

        using var rsa = RSA.Create();
        var privateKeyData = Convert.ToBase64String(rsa.ExportRSAPrivateKey(), Base64FormattingOptions.InsertLineBreaks);
        var publicKeyData = Convert.ToBase64String(rsa.ExportRSAPublicKey(), Base64FormattingOptions.InsertLineBreaks);

        var privateKey = $"{privateKeyHeader}{privateKeyData}{privateKeyFooter}";
        var publicKey = $"{publicKeyHeader}{publicKeyData}{publicKeyFooter}";
        var key = new Key(1,privateKey,publicKey,"RS256");

        return key;
    }
}
