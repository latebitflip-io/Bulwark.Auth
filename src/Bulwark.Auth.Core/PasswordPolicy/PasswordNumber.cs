using System.Linq;
using Bulwark.Auth.Core.Exception;

namespace Bulwark.Auth.Core.PasswordPolicy;

public class PasswordNumber : IPasswordPolicy
{
    public string Name => "PasswordSymbol";
    public void Validate(string password)
    {
        if (!password.Any(char.IsNumber))
        {
            throw new BulwarkPolicyException("Password must contain at least one number.");
        }
    }
}