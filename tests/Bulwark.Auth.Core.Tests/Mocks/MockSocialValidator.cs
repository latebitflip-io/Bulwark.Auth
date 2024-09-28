using Bulwark.Auth.Core.Exception;
using Bulwark.Auth.Core.Social;

namespace Bulwark.Auth.Core.Tests.Mocks;

public class MockSocialValidator : ISocialValidator
{
    public MockSocialValidator()
    {
    }

    public MockSocialValidator(string name)
	{
        Name = name;
	}

    public string Name { get; set; }

    public async Task<Social.Social> ValidateToken(string token)
    {
        if(token == "validtoken")
        {
            return await Task.FromResult(new Social.Social()
            {
                Email = "test@lateflip.io",
                Provider = Name,
                SocialId = "email"
            });
        }

        throw
            new BulwarkSocialException("Social token cannot be validated");
    }
}