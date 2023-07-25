using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Bulwark.Auth.Core.Domain;

namespace Bulwark.Auth.Core.Util;

public static class CertificateGenerator
{
    public static Certificate MakeCert(int certLifeInDays)
    {
        // const string CRT_HEADER = "-----BEGIN CERTIFICATE-----\n";
        // const string CRT_FOOTER = "\n-----END CERTIFICATE-----";

        const string privateKeyHeader = "-----BEGIN RSA PRIVATE KEY-----\n";
        const string privateKeyFooter = "\n-----END RSA PRIVATE KEY-----";

        const string publicKeyHeader = "-----BEGIN RSA PUBLIC KEY-----\n";
        const string publicKeyFooter = "\n-----END RSA PUBLIC KEY-----";

        using var rsa = RSA.Create();
        //var certRequest = new CertificateRequest("cn=bulwark", rsa, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        //var certificate = certRequest.CreateSelfSigned(DateTimeOffset.Now, DateTimeOffset.Now.AddDays(certLifeInDays));
        var privateKeyData = Convert.ToBase64String(rsa.ExportRSAPrivateKey(), Base64FormattingOptions.InsertLineBreaks);
        var publicKeyData = Convert.ToBase64String(rsa.ExportRSAPublicKey(), Base64FormattingOptions.InsertLineBreaks);

        var privateKey = $"{privateKeyHeader}{privateKeyData}{privateKeyFooter}";
        var publicKey = $"{publicKeyHeader}{publicKeyData}{publicKeyFooter}";
        var cert = new Certificate(1,privateKey,publicKey);
        //var exportData = certificate.Export(X509ContentType.Cert);

        //var crt = Convert.ToBase64String(exportData, Base64FormattingOptions.InsertLineBreaks);
        //certs.Cert = CRT_HEADER + crt + CRT_FOOTER;

        return cert;
    }

   //creates a string versions of the public private keys for easy storage
    public static StringCertificate MakeStringCert(int certLifeInDays)
    {
        //const string CRT_HEADER = "-----BEGIN CERTIFICATE-----\n";
        //const string CRT_FOOTER = "\n-----END CERTIFICATE-----";

        const string privateKeyHeader = "-----BEGIN RSA PRIVATE KEY-----\n";
        const string privateKeyFooter = "\n-----END RSA PRIVATE KEY-----";

        const string publicKeyHeader = "-----BEGIN RSA PUBLIC KEY-----\n";
        const string publicKeyFooter = "\n-----END RSA PUBLIC KEY-----";

        using var rsa = RSA.Create();
        //var certRequest = new CertificateRequest("cn=bulwark", rsa, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        //var certificate = certRequest.CreateSelfSigned(DateTimeOffset.Now, DateTimeOffset.Now.AddDays(certLifeInDays));
        var privateKeyData = Convert.ToBase64String(rsa.ExportRSAPrivateKey(), Base64FormattingOptions.InsertLineBreaks);
        var publicKeyData = Convert.ToBase64String(rsa.ExportRSAPublicKey(), Base64FormattingOptions.InsertLineBreaks);

        var privateKey = $"{privateKeyHeader}{privateKeyData}{privateKeyFooter}";
        var publicKey = $"{publicKeyHeader}{publicKeyData}{publicKeyFooter}";
        var cert = new StringCertificate(privateKey, publicKey);
        //var exportData = certificate.Export(X509ContentType.Cert);

        //var crt = Convert.ToBase64String(exportData, Base64FormattingOptions.InsertLineBreaks);
        //certs.Cert = CRT_HEADER + crt + CRT_FOOTER;

        return cert;
    }
}
