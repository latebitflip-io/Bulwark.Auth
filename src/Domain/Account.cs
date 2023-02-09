using System;
namespace Bulwark.Domain
{
    public class Account
    {
        public Guid Id { get; }
        public string Email { get; }
        public string Password { get; }
        public DateTime Created { get; }
        public DateTime Modified { get; }
    }
}
