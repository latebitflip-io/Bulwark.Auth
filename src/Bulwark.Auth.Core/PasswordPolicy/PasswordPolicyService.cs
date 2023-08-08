using System.Collections.Generic;

namespace Bulwark.Auth.Core.PasswordPolicy;

public class PasswordPolicyService : IPasswordPolicy
{
    private readonly List<IPasswordPolicy> _policies;
    public string Name => "PasswordPolicyService";

    public PasswordPolicyService()
    {
        _policies = new List<IPasswordPolicy>();
    }
    
    public PasswordPolicyService(List<IPasswordPolicy> policies)
    {
        _policies = policies;
    }
    
    public void Add(IPasswordPolicy policy)
    {
        _policies.Add(policy);
    }
    
    public void Validate(string password)
    {
        foreach (var policy in _policies)
        {
            policy.Validate(password);
        }
    }
}