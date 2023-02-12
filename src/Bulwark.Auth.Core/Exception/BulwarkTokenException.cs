using System;
namespace Bulwark.Auth.Core.Exception;

public class BulwarkTokenException : System.Exception
{
    public BulwarkTokenException(string message) :
        base(message)
    {
    }

    public BulwarkTokenException(string message, System.Exception inner) :
        base(message, inner)
    {
    }
}

