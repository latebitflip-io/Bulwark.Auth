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