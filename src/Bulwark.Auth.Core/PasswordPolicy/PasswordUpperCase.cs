using System.Linq;
using Bulwark.Auth.Core.Exception;

namespace Bulwark.Auth.Core.PasswordPolicy;

public class PasswordUpperCase : IPasswordPolicy
{
    public string Name => "PasswordUpperCase";
    public void Validate(string password)
    {
        if (!password.Any(char.IsUpper))
        {
            throw new BulwarkPolicyException("Password must contain at least one uppercase character.");
        }
    }
}