using System;
namespace Bulwark.Auth.Core.Exception;

public class BulwarkTokenExpiredException : System.Exception
{
    public BulwarkTokenExpiredException(string message) :
        base(message)
    {
    }

    public BulwarkTokenExpiredException(string message, System.Exception inner) :
        base(message, inner)
    {
    }
}


