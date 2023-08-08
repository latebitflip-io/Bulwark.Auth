namespace Bulwark.Auth.Core.PasswordPolicy;

public interface IPasswordPolicy
{
    public string Name { get; }
    void Validate(string password);
}