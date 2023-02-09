using System;
namespace Bulwark.Auth.Core.Exception;

public class BulwarkSocialException : System.Exception
{
    public BulwarkSocialException(string message) :
        base(message)
    {
    }

    public BulwarkSocialException(string message, System.Exception inner) :
        base(message, inner)
    {
    }
}

