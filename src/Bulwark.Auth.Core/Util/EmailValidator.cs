using System;
using System.Globalization;
using System.Text.RegularExpressions;
using Bulwark.Auth.Core.Exception;

namespace Bulwark.Auth.Core.Util;
/// <summary>
/// https://learn.microsoft.com/en-us/dotnet/standard/base-types/how-to-verify-that-strings-are-in-valid-email-format
/// </summary>
public static class EmailValidator
{
    public static void Validate(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            throw new BulwarkPolicyException("Email is invalid.");
        }

        try
        {
            // Normalize the domain
            email = Regex.Replace(email, @"(@)(.+)$", DomainMapper,
                RegexOptions.None, TimeSpan.FromMilliseconds(200));

            // Examines the domain part of the email and normalizes it.
            string DomainMapper(Match match)
            {
                // Use IdnMapping class to convert Unicode domain names.
                var idn = new IdnMapping();

                // Pull out and process domain name (throws ArgumentException on invalid)
                var domainName = idn.GetAscii(match.Groups[2].Value);

                return match.Groups[1].Value + domainName;
            }
        }
        catch (RegexMatchTimeoutException)
        {
            throw new BulwarkPolicyException("Email is invalid.");
        }
        catch (ArgumentException)
        {
            throw new BulwarkPolicyException("Email is invalid.");
        }

        try
        {
            if (!Regex.IsMatch(email,
                    @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
                    RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250)))
            {
                throw new BulwarkPolicyException("Email is invalid.");
            }

        }
        catch (RegexMatchTimeoutException)
        {
            throw new BulwarkPolicyException("Email is invalid.");
        }
    }
}