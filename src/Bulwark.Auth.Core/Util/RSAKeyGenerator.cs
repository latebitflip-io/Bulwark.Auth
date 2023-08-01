using System;
using System.Security.Cryptography;
using Bulwark.Auth.Core.Domain;

namespace Bulwark.Auth.Core.Util;

public static class RsaKeyGenerator
{
    public static Key MakeKey(int byteSize = 256)
    {
        var bits = byteSize * 8;
        const string privateKeyHeader = "-----BEGIN RSA PRIVATE KEY-----\n";
        const string privateKeyFooter = "\n-----END RSA PRIVATE KEY-----";

        const string publicKeyHeader = "-----BEGIN RSA PUBLIC KEY-----\n";
        const string publicKeyFooter = "\n-----END RSA PUBLIC KEY-----";

        using var rsa = RSA.Create(bits);
        var privateKeyData = Convert.ToBase64String(rsa.ExportRSAPrivateKey(), Base64FormattingOptions.InsertLineBreaks);
        var publicKeyData = Convert.ToBase64String(rsa.ExportRSAPublicKey(), Base64FormattingOptions.InsertLineBreaks);

        var privateKey = $"{privateKeyHeader}{privateKeyData}{privateKeyFooter}";
        var publicKey = $"{publicKeyHeader}{publicKeyData}{publicKeyFooter}";
        var key = new Key(privateKey,publicKey,$"RS{byteSize}", "PKCS#1");
        

        return key;
    }
}
