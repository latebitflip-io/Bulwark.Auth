namespace Bulwark.Auth.Common;
public class ChangePasswordPayload
{
   public string Email { get; set; }
   public string NewPassword { get; set; }
   public string AccessToken { get; set; }
}


