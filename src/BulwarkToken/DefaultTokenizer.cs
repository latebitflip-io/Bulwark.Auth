using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using Bulwark.Repositories;
using JWT.Algorithms;
using JWT.Builder;

namespace Bulwark.Token
{
    public class DefaultTokenizer : ITokenizer
    {
        public string Name { get; }
        public string Issuer { get; }
        public string Audience { get; }

        private Dictionary<int, Certificates> _certs;
        private RSA _privateKey;
        private RSA _publicKey;
        
        public DefaultTokenizer(string issuer, string audience,
            RSA privateKey, RSA publicKey)
        {
            _privateKey = privateKey;
            _publicKey = publicKey;
            Name = "default";
            Issuer = issuer;
            Audience = audience;
        }

        public string CreateAccessToken(string userId)
        {
            var token = JwtBuilder.Create()
                      .WithAlgorithm(new RS256Algorithm(_publicKey,
                      _privateKey)) 
                      .AddHeader("use", "access")
                      .Id(Guid.NewGuid().ToString())
                      .Issuer(Issuer)
                      .Audience(Audience)
                      .AddClaim("exp",
                            DateTimeOffset.UtcNow.AddHours(1)
                      .ToUnixTimeSeconds())
                      .AddClaim("sub", userId)
                      .Encode();

            return token;
        }

        public string CreateIdToken(
            Dictionary<string, object> idClaims, string salt)
        {
            var token = JwtBuilder.Create()
                      .WithAlgorithm(new HMACSHA256Algorithm())
                      .AddHeader("use", "identity")
                      .Id(Guid.NewGuid().ToString())
                      .WithSecret(salt)
                      .AddClaim("exp", DateTimeOffset.UtcNow.AddHours(1)
                      .ToUnixTimeSeconds())
                      .AddClaim("iss", Issuer)
                      .AddClaim("aud", Audience);

            token.AddClaims(idClaims);     
            return token.Encode();
        }

        public string CreateRefreshToken(string userId, string salt)
        {
            var token = JwtBuilder.Create()
                      .WithAlgorithm(new HMACSHA256Algorithm())
                      .WithSecret(salt)
                      .AddHeader("use", "refresh")
                      .Id(Guid.NewGuid().ToString())
                      .Issuer(Issuer)
                      .Audience(Audience)
                      .AddClaim("exp", DateTimeOffset.UtcNow.AddHours(1)
                      .ToUnixTimeSeconds())
                      .AddClaim("sub", userId)
                      .Encode();

            return token;
        }

        public string ValidateAccessToken(string token)
        {
            var json = JwtBuilder.Create()
                     .WithAlgorithm(new RS256Algorithm(_publicKey))
                     .MustVerifySignature()
                     .Decode(token);

            return json;
        }

        public string ValidateIdToken(string token, string salt)
        {
            var json = JwtBuilder.Create()
                     .WithAlgorithm(new HMACSHA256Algorithm())
                     .WithSecret(salt)
                     .MustVerifySignature()
                     .Decode(token);

            return json;
        }

        public string ValidateRefreshToken(string token, string salt)
        {
            var json = JwtBuilder.Create()
                     .WithAlgorithm(new HMACSHA256Algorithm())
                     .WithSecret(salt)
                     .MustVerifySignature()
                     .Decode(token);

            return json;
        }
    }
}
