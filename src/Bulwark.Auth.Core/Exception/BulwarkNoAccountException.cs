namespace Bulwark.Auth.Core.Exception;

public class BulwarkNoAccountException : System.Exception
{
    public BulwarkNoAccountException(string message) :
        base(message)
    {
    }

    public BulwarkNoAccountException(string message, System.Exception inner) :
        base(message, inner)
    {
    }
}