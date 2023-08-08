using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using Bulwark.Auth.Core.Exception;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

namespace Bulwark.Auth.Core.Social.Validators;

public class MicrosoftValidator : ISocialValidator
{
    private readonly string _clientId;
    private readonly string _tenantId;
    private readonly Uri _configUri = 
        new Uri("https://login.microsoftonline.com/common/v2.0/.well-known/openid-configuration");
    public string Name { get; }
    
    public MicrosoftValidator(string clientId, string tenantId)
    {
        Name = "microsoft";
        _clientId = clientId;
        _tenantId = tenantId;
    }
    /// <summary>
    /// takes an idToken from microsoft and validates it, access tokens are not supported as they are
    /// opaque
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    /// <exception cref="BulwarkSocialException"></exception>
    public async Task<Social> ValidateToken(string token)
    {
        try
        {
            var openIdConfigManager = new ConfigurationManager<OpenIdConnectConfiguration>(
                _configUri.ToString(),
                new OpenIdConnectConfigurationRetriever()
            );

            var openIdConfig = await openIdConfigManager.GetConfigurationAsync().ConfigureAwait(false);
            var validationParams = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                ValidateAudience = true,
                ValidateIssuer = true,
                ValidateLifetime = true,
                ValidateTokenReplay = true,
                ValidAudience = _clientId,
                ValidIssuer = openIdConfig.Issuer.Replace("{tenantid}", _tenantId),
                IssuerSigningKeys = openIdConfig.SigningKeys,
            };

            var jwtTokenHandler = new JwtSecurityTokenHandler();
            jwtTokenHandler.ValidateToken(token, validationParams, out SecurityToken validToken);

            var securityToken = validToken as JwtSecurityToken
                   ?? throw new BulwarkSocialException("Microsoft token cannot be validated");
            
            return new Social()
            {
                Email = securityToken.Claims.FirstOrDefault(c => c.Type == "preferred_username")?.Value,
                Provider = Name,
                SocialId = securityToken.Subject
            };
        }
        catch (System.Exception exception)
        {
            throw new BulwarkSocialException("Microsoft token cannot be validated",
                exception);
        }
    }
}