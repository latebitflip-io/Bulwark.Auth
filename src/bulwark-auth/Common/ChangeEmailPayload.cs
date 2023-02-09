namespace Bulwark.Auth.Common;
public class ChangeEmailPayload
{
    public string Email { get; set; }
    public string NewEmail { get; set; }
    public string AccessToken { get; set; }
}


