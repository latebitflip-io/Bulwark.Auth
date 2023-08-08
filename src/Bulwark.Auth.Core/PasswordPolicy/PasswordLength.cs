using Bulwark.Auth.Core.Exception;

namespace Bulwark.Auth.Core.PasswordPolicy;

public class PasswordLength : IPasswordPolicy
{
    private readonly int _maxLength;
    private readonly int _minLength;
    
    public string Name => "PasswordLength";
    
    
    public PasswordLength(int minLength, int maxLength)
    {
        _minLength = minLength;
        _maxLength = maxLength;
    }
    
    public void Validate(string password)
    {
        if (password.Length < _minLength || password.Length > _maxLength)
        {
            throw new BulwarkPolicyException($"Password must be min: {_minLength} and max: " +
                                             $"{_maxLength} characters long.");
        }
    }
}