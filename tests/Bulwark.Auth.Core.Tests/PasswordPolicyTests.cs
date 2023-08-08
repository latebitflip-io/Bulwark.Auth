using Bulwark.Auth.Core.Exception;
using Bulwark.Auth.Core.PasswordPolicy;

namespace Bulwark.Auth.Core.Tests;

public class PasswordPolicyTests
{
    public PasswordPolicyTests()
    {
    }
    
    [Fact]
    public void PasswordLengthPolicy()
    {
        var policy = new PasswordLength(8, 64);
        policy.Validate("password");
        try
        {
            policy.Validate("pass");
        }
        catch(BulwarkPolicyException)
        {
            Assert.True(true);
        }
    }
    
    [Fact]
    public void PasswordUpperCase()
    {
        var policy = new PasswordUpperCase();
        policy.Validate("Password");
        try
        {
            policy.Validate("password");
        }
        catch(BulwarkPolicyException)
        {
            Assert.True(true);
        }
    }
    
    [Fact]
    public void PasswordLowerCase()
    {
        var policy = new PasswordLowerCase();
        policy.Validate("password");
        try
        {
            policy.Validate("PASSWORD");
        }
        catch(BulwarkPolicyException)
        {
            Assert.True(true);
        }
    }
    
    [Fact]
    public void PasswordSymbol()
    {
        var policy = new PasswordSymbol();
        policy.Validate("password#");
        try
        {
            policy.Validate("password");
        }
        catch(BulwarkPolicyException)
        {
            Assert.True(true);
        }
    }
    
    [Fact]
    public void PasswordNumber()
    {
        var policy = new PasswordNumber();
        policy.Validate("password1");
        try
        {
            policy.Validate("password");
        }
        catch(BulwarkPolicyException)
        {
            Assert.True(true);
        }
    }
}