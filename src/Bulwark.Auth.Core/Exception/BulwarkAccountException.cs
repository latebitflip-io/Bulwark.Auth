using System;
namespace Bulwark.Auth.Core.Exception;

public class BulwarkAccountException : System.Exception
{
    public BulwarkAccountException(string message) :
        base(message)
    {
    }

    public BulwarkAccountException(string message, System.Exception inner) :
        base(message, inner)
    {
    }
}


