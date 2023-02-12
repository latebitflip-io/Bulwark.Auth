using System.Collections.Generic;
using System.Text.Json;
using Bulwark.Auth.Core.Domain;

namespace Bulwark.Auth.Core;

public class TokenStrategyContext
{
    private Dictionary<string, ITokenizer> _tokenizers;
    public TokenStrategyContext(List<ITokenizer> tokenizers)
    {
        _tokenizers = new Dictionary<string, ITokenizer>();
        foreach (var tokenizer in tokenizers)
        {
            Add(tokenizer);
        }
    }
    
    public TokenStrategyContext()
    {
        _tokenizers = new Dictionary<string, ITokenizer>();
    }

    public void Add(ITokenizer tokenizer)
    {
        _tokenizers.Add(tokenizer.Name, tokenizer);
    }

    public ITokenizer GetTokenizer(string tokenizerName)
    {
        return _tokenizers[tokenizerName];
    }

    public string CreateAccessToken(string userId, List<string> roles, 
        List<string> permissions, string name = "default")
    {
        var tokenizer = _tokenizers[name];
        return tokenizer.CreateAccessToken(userId, roles, permissions);
    }
    
    public string CreateRefreshToken(string userId,
        string name = "default")
    {
        var tokenizer = _tokenizers[name];
        return tokenizer.CreateRefreshToken(userId);
    }

    public AccessToken ValidateAccessToken(string userId, string token, string name = "default")
    {
        var json = _tokenizers[name].ValidateToken(userId,token);
        var accessToken = JsonSerializer
            .Deserialize<AccessToken>(json);
        return accessToken;
    }

    public RefreshToken ValidateRefreshToken(string userId, string token,
        string name = "default")
    {
        var json = _tokenizers[name].ValidateToken(userId, token);
        var refreshToken = JsonSerializer
            .Deserialize<RefreshToken>(json);
        return refreshToken;
    }
} 
