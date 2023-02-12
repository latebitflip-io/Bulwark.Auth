namespace Bulwark.Auth.Repositories.Exception;
public class BulwarkDbDuplicateException : System.Exception
{
    public BulwarkDbDuplicateException(
        string message) : base(message)
    { }

    public BulwarkDbDuplicateException(string message,
        System.Exception inner) : base(message, inner)
    {

    }
}


