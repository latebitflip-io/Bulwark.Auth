namespace Bulwark.Auth.Repositories.Exception;

public class BulwarkDbNotFoundException : System.Exception
{
    public BulwarkDbNotFoundException(string message) :
        base(message)
    {
    }

    public BulwarkDbNotFoundException(string message, System.Exception inner) :
        base(message, inner)
    {
    }
}