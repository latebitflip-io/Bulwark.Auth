namespace Bulwark.Auth.Core.Exception;

public class BulwarkPolicyException : System.Exception
{
    public BulwarkPolicyException(string message) :
        base(message)
    {
    }

    public BulwarkPolicyException(string message, System.Exception inner) :
        base(message, inner)
    {
    }
}