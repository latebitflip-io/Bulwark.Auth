using System;

namespace Bulwark.Auth.Common.Payloads;

public class Key
{
    public string KeyId { get; set; }
    public string Format { get; set; }
    public string PublicKey { get; set; }
    public string Algorithm { get; set; }
    public DateTime Created { get; set; }
}