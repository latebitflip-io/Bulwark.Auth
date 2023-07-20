using System.Threading.Tasks;
using Bulwark.Auth.Core.Exception;
using Google.Apis.Auth;

namespace Bulwark.Auth.Core.Social.Validators;

public class GoogleValidator : ISocialValidator
{
    private readonly string _clientId;
    public string Name { get; }
    public GoogleValidator(string clientId)
    {
        Name = "google";
        _clientId = clientId;
    }

    public async Task<Social> ValidateToken(string token)
    {
        var validationSettings = new GoogleJsonWebSignature.ValidationSettings
        { 
            Audience = new string[] { _clientId }
        };

        try { 
            var payload = await GoogleJsonWebSignature.ValidateAsync(token, validationSettings);
                
            return new Social()
            {
                Email = payload.Email,
                Provider = Name,
                SocialId = payload.Subject
            };
        }
        catch (InvalidJwtException exception)
        {
            throw new BulwarkSocialException("Google token cannot be validated",
                exception);
        }
    }
}