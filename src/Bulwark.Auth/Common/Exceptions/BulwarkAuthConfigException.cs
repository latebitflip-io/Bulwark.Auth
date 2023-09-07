namespace Bulwark.Auth.Common.Exceptions;

public class BulwarkAuthConfigException : System.Exception
{
    public BulwarkAuthConfigException(string message) :
        base(message)
    { }

    public BulwarkAuthConfigException(string message, System.Exception inner) :
        base(message, inner)
    { }
}
