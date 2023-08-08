using System.Linq;
using Bulwark.Auth.Core.Exception;

namespace Bulwark.Auth.Core.PasswordPolicy;

public class PasswordSymbol : IPasswordPolicy
{
    public string Name => "PasswordSymbol";
    public void Validate(string password)
    {
        if (!password.Any(char.IsPunctuation))
        {
            throw new BulwarkPolicyException("Password must contain at least one symbol.");
        }
    }
}