namespace Bulwark.Auth.Repositories.Exception;
public class BulwarkDbException : System.Exception
{
    public BulwarkDbException(
        string message) : base(message)
    { }

    public BulwarkDbException(string message,
        System.Exception inner) : base(message, inner)
    {

    }
}


