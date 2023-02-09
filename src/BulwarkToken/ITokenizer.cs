using System;
using System.Collections.Generic;

namespace Bulwark.Token
{
    public interface ITokenizer
    {
        string Issuer { get; }
        string Audience { get; }
        string Name { get; }
        string CreateAccessToken(string userId);
        string CreateIdToken(
            Dictionary<string, object> idClaims, string salt);
        string CreateRefreshToken(string userId, string salt);
        string ValidateAccessToken(string token);
        string ValidateIdToken(string token, string salt);
        string ValidateRefreshToken(string token, string salt);
    }
}
