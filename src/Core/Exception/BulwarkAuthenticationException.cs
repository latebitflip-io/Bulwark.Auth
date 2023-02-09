namespace Bulwark.Auth.Core.Exception;

public class BulwarkAuthenticationException : System.Exception
{
	public BulwarkAuthenticationException(string message) :
		base(message)
	{
	}

    public BulwarkAuthenticationException(string message, System.Exception inner) :
        base(message, inner)
    {
    }
}


