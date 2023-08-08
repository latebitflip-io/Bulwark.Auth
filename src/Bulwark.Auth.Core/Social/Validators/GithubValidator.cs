using System.Threading.Tasks;
using Bulwark.Auth.Core.Exception;
using Octokit;

namespace Bulwark.Auth.Core.Social.Validators;

public class GithubValidator : ISocialValidator
{
    private readonly GitHubClient _client;
        
    public string Name { get; }
    
    public GithubValidator(string githubAppName)
    {
        Name = "github";
        _client = 
            new GitHubClient(new ProductHeaderValue(githubAppName));
    }
    /// <summary>
    /// This takes a github oauth access token or personal access tokens  and validates it,
    /// these tokens are not in JWT format, they are opaque and need to be used purely
    /// as credentials.
    /// The token is used to get the user information from github
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    /// <exception cref="BulwarkSocialException"></exception>
    public async Task<Social> ValidateToken(string token)
    {
        try
        {
            _client.Credentials = new Credentials(token);
            var user = await _client.User.Current();

            return new Social()
            {
                Email = user.Email,
                Provider = Name,
                SocialId = user.Id.ToString()
            };
        }
        catch (System.Exception exception)
        {
            throw new BulwarkSocialException("Github token cannot be validated",
                exception);
        }
    }
}