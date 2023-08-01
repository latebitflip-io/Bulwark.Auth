﻿using System;
using System.Collections.Generic;
using JWT.Builder;
using System.Linq;
using System.IdentityModel.Tokens.Jwt;
using Bulwark.Auth.Core.Domain;
using Bulwark.Auth.Core.Exception;
using Bulwark.Auth.Core.SigningAlgs;

namespace Bulwark.Auth.Core;

/// <summary>
/// Part of the token strategy, this is a default implementation supporting RS256 signing.
/// More strategies will be added in the future.
/// </summary>
public class JwtTokenizer : ITokenizer
{
    public string Name { get; }
    public string Issuer { get; }
    public string Audience { get; }

    private readonly SortedList<int,Key> _keys = new();
    private readonly Dictionary<string, ISigningAlgorithm> _signingAlgorithms = new();

    public JwtTokenizer(string issuer, string audience, List<ISigningAlgorithm> signingAlgorithms,
        IEnumerable<Key> keys)
    {
        foreach(var key in keys)
        {
            _keys.Add(key.Generation, key);
        }

        foreach (var alg in signingAlgorithms)
        {
            _signingAlgorithms.Add(alg.Name, alg);
        }
        
        Name = "jwt";
        Issuer = issuer;
        Audience = audience;
    }

    /// <summary>
    /// This will creat an access token for a user with the given roles and permissions.
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="roles"></param>
    /// <param name="permissions"></param>
    /// <returns></returns>
    /// <exception cref="BulwarkTokenException"></exception>
    public string CreateAccessToken(string userId, List<string> roles, List<string> permissions)
    {
        var latestKey = GetLatestKeyGeneration();
        if (latestKey == null)
        {
            throw new BulwarkTokenException("No keys found");
        }

        var token = JwtBuilder.Create()
                  .WithAlgorithm(_signingAlgorithms[latestKey.Algorithm.ToUpper()].GetAlgorithm(latestKey.PrivateKey,
                  latestKey.PublicKey))
                  .AddHeader("use", "access")
                  .AddHeader("gen", latestKey.Generation)
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
    
    /// <summary>
    /// This will create a refresh token for a user. Refresh tokens are longer lived tokens that can be used to
    /// create new access tokens.
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    /// <exception cref="BulwarkTokenException"></exception>
    public string CreateRefreshToken(string userId)
    {
        var latestKey = GetLatestKeyGeneration();
        if (latestKey == null)
        {
            throw new BulwarkTokenException("No keys found");
        }
        
        var token = JwtBuilder.Create()
            .WithAlgorithm(_signingAlgorithms[latestKey.Algorithm.ToUpper()].GetAlgorithm(latestKey.PrivateKey,
                latestKey.PublicKey))
                  .AddHeader("use", "refresh")
                  .AddHeader("gen", latestKey.Generation)
                  .Id(Guid.NewGuid().ToString())
                  .Issuer(Issuer)
                  .Audience(Audience)
                  .AddClaim("exp", DateTimeOffset.UtcNow.AddHours(1)
                  .ToUnixTimeSeconds())
                  .AddClaim("sub", userId)
                  .Encode();

        return token;
    }

    /// <summary>
    /// This will validate a refresh or access token 
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    /// <exception cref="BulwarkTokenException"></exception>
    public string ValidateToken(string userId, string token)
    {
        var handler = new JwtSecurityTokenHandler();
        var decodedValue = handler.ReadJwtToken(token);
        if(userId != decodedValue.Subject)
        {
            throw new BulwarkTokenException("Token user miss match");
        }
        
        var generation = int.Parse(decodedValue.Header["gen"].ToString());
        var key = GetKeyGeneration(generation);

        var json = JwtBuilder.Create()
                 .WithAlgorithm(_signingAlgorithms[key.Algorithm.ToUpper()].GetAlgorithm(key.PrivateKey,
                     key.PublicKey))
                 .MustVerifySignature()
                 .Decode(token);

        return json;
    }
    /// <summary>
    /// Gets the latest cert to generate a token with.
    /// </summary>
    /// <returns></returns>
    private Key GetLatestKeyGeneration()
    {
        if (_keys.Count == 0) { return null; }
        var max = _keys.Keys.Max();
        return _keys[max];
    }

    /// <summary>
    /// Will pull a cert for a specific generation.
    /// </summary>
    /// <param name="generation"></param>
    /// <returns></returns>
    private Key GetKeyGeneration(int generation)
    {
        return _keys[generation];
    }
}
