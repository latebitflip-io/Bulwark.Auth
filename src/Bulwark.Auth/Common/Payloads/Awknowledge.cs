namespace Bulwark.Auth.Common.Payloads;
public class Acknowledge
{
    public string Email { get; set; }
	public string DeviceId { get; set; }
    public string AccessToken { get; set; }
    public string IdToken { get; set; }
    public string RefreshToken { get; set; }
}


