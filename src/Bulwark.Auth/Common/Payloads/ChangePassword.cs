namespace Bulwark.Auth.Common.Payloads;
public class ChangePassword
{
   public string Email { get; set; }
   public string NewPassword { get; set; }
   public string AccessToken { get; set; }
}


