using System;
using System.Collections.Generic;

namespace Bulwark.Auth.Core.Domain;

public class Account
{
    public string Id { get; set; }
    public string Email { get; set; }
    public bool Verified { get; set; }
    public bool Enabled { get; set; }
    public bool Deleted { get; set; }

    public List<string> Roles { get; set; }
    public List<string> Permissions { get; set; }

    public DateTime Created { get; set; }
    
    
    public Account()
    {
    }
}

