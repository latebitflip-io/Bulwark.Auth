using System.Collections.Generic;

namespace Bulwark.Auth.Core;
public interface ITokenizer
{
    string Issuer { get; }
    string Audience { get; }
    string Name { get; }
    string CreateAccessToken(string userId, List<string> roles, List<string> permissions);
    string CreateRefreshToken(string userId);
    string ValidateToken(string userId, string token);
}

