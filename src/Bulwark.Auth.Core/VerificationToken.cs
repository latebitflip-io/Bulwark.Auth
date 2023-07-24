using System;

namespace Bulwark.Auth.Core
{
    public class VerificationToken
    {
        public string Value { get; }
        public DateTime Created { get; }

        public VerificationToken(string token, DateTime created)
        {
            Value = token;
            Created = created;
        }
    }
}
