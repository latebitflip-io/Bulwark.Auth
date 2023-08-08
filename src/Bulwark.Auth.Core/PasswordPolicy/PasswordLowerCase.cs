using System.Linq;
using Bulwark.Auth.Core.Exception;

namespace Bulwark.Auth.Core.PasswordPolicy;

public class PasswordLowerCase : IPasswordPolicy
{
    public string Name => "PasswordLowerCase";

    public void Validate(string password)
    {
        if (!password.Any(char.IsLower))
        {
            throw new BulwarkPolicyException("Password must contain at least one lowercase character.");
        }
    }
}