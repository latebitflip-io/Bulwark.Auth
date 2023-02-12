using System;
using System.Collections.Generic;
using JWT.Algorithms;
using JWT.Builder;
using System.Linq;
using System.IdentityModel.Tokens.Jwt;
using Bulwark.Auth.Core.Domain;
using Bulwark.Auth.Core.Exception;

namespace Bulwark.Auth.Core;

public class DefaultTokenizer : ITokenizer
{
    public string Name { get; }
    public string Issuer { get; }
    public string Audience { get; }

    private readonly SortedList<int,Certificate> _certificates
        = new SortedList<int, Certificate>();

    public DefaultTokenizer(string issuer, string audience,
        Certificate[] certificates)
    {
        foreach(var cert in certificates)
        {
            _certificates.Add(cert.Generation, cert);
        }

        Name = "default";
        Issuer = issuer;
        Audience = audience;
    }

    public string CreateAccessToken(string userId, List<string> roles, List<string> permissions)
    {
        var cert = GetLatestCertGeneration();
        if (cert == null)
        {
            throw new BulwarkTokenException("No certificates found");
        }

        var token = JwtBuilder.Create()
                  .WithAlgorithm(new RS256Algorithm(cert.PublicKey,
                  cert.PrivateKey)) 
                  .AddHeader("use", "access")
                  .AddHeader("gen", cert.Generation)
                  .Id(Guid.NewGuid().ToString())
                  .Issuer(Issuer)
                  .Audience(Audience)
                  .AddClaim("roles", roles)
                  .AddClaim("permissions", permissions)
                  .AddClaim("exp",
                        DateTimeOffset.UtcNow.AddHours(1)
                  .ToUnixTimeSeconds())
                  .AddClaim("sub", userId)
                  
                  .Encode();

        return token;
    }
    
    public string CreateRefreshToken(string userId)
    {
        var cert = GetLatestCertGeneration();
        if (cert == null)
        {
            throw new BulwarkTokenException("No certificates found");
        }
        
        var token = JwtBuilder.Create()
                  .WithAlgorithm(new RS256Algorithm(cert.PublicKey, cert.PrivateKey))
                  .AddHeader("use", "refresh")
                  .AddHeader("gen", cert.Generation)
                  .Id(Guid.NewGuid().ToString())
                  .Issuer(Issuer)
                  .Audience(Audience)
                  .AddClaim("exp", DateTimeOffset.UtcNow.AddHours(1)
                  .ToUnixTimeSeconds())
                  .AddClaim("sub", userId)
                  .Encode();

        return token;
    }

    public string ValidateToken(string userId, string token)
    {
        var handler = new JwtSecurityTokenHandler();
        var decodedValue = handler.ReadJwtToken(token);
        if(userId != decodedValue.Subject)
        {
            throw new BulwarkTokenException("Token user miss match");
        }
        
        var generation = int.Parse(decodedValue.Header["gen"].ToString());
        var cert = GetCertGeneration(generation);

        var json = JwtBuilder.Create()
                 .WithAlgorithm(new RS256Algorithm(cert.PublicKey))
                 .MustVerifySignature()
                 .Decode(token);

        return json;
    }
    private Certificate GetLatestCertGeneration()
    {
        if (_certificates.Count == 0) { return null; }
        var max = _certificates.Keys.Max();
        return _certificates[max];
    }

    private Certificate GetCertGeneration(int generation)
    {
        return _certificates[generation];
    }
}

