namespace Bulwark.Auth.Core.Domain;

public class Authenticated
{
    public string AccessToken { get; }
    public string RefreshToken { get; }

    public Authenticated(string accessToken,
        string refreshToken)
    {
        //jwt with a short lifespan and used as bearer
        AccessToken = accessToken;
        //jwt to use to request a new access token
        RefreshToken = refreshToken;
    }
    
    public Authenticated()
    {
        AccessToken = string.Empty;
        RefreshToken = string.Empty;
    }
}
