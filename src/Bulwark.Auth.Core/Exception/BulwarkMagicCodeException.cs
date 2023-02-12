namespace Bulwark.Auth.Core.Exception;

public class BulwarkMagicCodeException : System.Exception
{
    public BulwarkMagicCodeException(string message) :
        base(message)
    {
    }

    public BulwarkMagicCodeException(string message, System.Exception inner) :
        base(message, inner)
    {
    }
}