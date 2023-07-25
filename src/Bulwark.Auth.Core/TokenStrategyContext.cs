using System.Collections.Generic;
using System.Text.Json;
using Bulwark.Auth.Core.Domain;

namespace Bulwark.Auth.Core;

/// <summary>
/// This classes responsibility is to provide a strategy for token creation and validation based off different
/// algs, currently only supports RS256, but can be easily expanded to support more.
/// </summary>
public class TokenStrategyContext
{
    private readonly Dictionary<string, ITokenizer> _tokenizers = new();
    public TokenStrategyContext(List<ITokenizer> tokenizers)
    {
        foreach (var tokenizer in tokenizers)
        {
            Add(tokenizer);
        }
    }
    
    public TokenStrategyContext()
    {
    }

    /// <summary>
    /// allows the addition of different tokenizers
    /// </summary>
    /// <param name="tokenizer"></param>
    public void Add(ITokenizer tokenizer)
    {
        _tokenizers.Add(tokenizer.Name, tokenizer);
    }

    /// <summary>
    /// will retrieve a specific tokenizer by name can be used to decide which algorithm to sign with
    /// </summary>
    /// <param name="tokenizerName"></param>
    /// <returns></returns>
    public ITokenizer GetTokenizer(string tokenizerName)
    {
        return _tokenizers[tokenizerName];
    }

    /// <summary>
    /// This is the token that would be checked for proper authorization
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="roles"></param>
    /// <param name="permissions"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public string CreateAccessToken(string userId, List<string> roles, 
        List<string> permissions, string name = "default")
    {
        var tokenizer = _tokenizers[name];
        return tokenizer.CreateAccessToken(userId, roles, permissions);
    }
    
    /// <summary>
    /// Creates a refresh token a long lived token that can refresh access tokens
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public string CreateRefreshToken(string userId,
        string name = "default")
    {
        var tokenizer = _tokenizers[name];
        return tokenizer.CreateRefreshToken(userId);
    }

    /// <summary>
    /// Validates access tokens, this can be done at anytime to ensure an account has proper access
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="token"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public AccessToken ValidateAccessToken(string userId, string token, string name = "default")
    {
        var json = _tokenizers[name].ValidateToken(userId,token);
        var accessToken = JsonSerializer
            .Deserialize<AccessToken>(json);
        return accessToken;
    }

    /// <summary>
    /// validates refresh token, validate refresh tokens before renewing access tokens
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="token"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public RefreshToken ValidateRefreshToken(string userId, string token,
        string name = "default")
    {
        var json = _tokenizers[name].ValidateToken(userId, token);
        var refreshToken = JsonSerializer
            .Deserialize<RefreshToken>(json);
        return refreshToken;
    }
} 
