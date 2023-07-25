using System.Collections.Generic;
using Bulwark.Auth.Core.Domain;
using Bulwark.Auth.Repositories.Model;

namespace Bulwark.Auth.Core.Util;

public static class Authenticate
{
    public static Authenticated CreateTokens(AccountModel account, 
        List<string> roles, List<string> permissions, 
        ITokenizer tokenizer)
       
	{
        var accessToken = tokenizer.CreateAccessToken(
            account.Id, roles, permissions);
        var refreshToken = tokenizer.CreateRefreshToken(
            account.Id);

        var authenticated = new Authenticated(accessToken,
            refreshToken);

        return authenticated;
    }
}


