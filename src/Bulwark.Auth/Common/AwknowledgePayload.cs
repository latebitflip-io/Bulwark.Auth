namespace Bulwark.Auth.Common;
public class AcknowledgePayload
{
    public string Email { get; set; }
	public string DeviceId { get; set; }
    public string AccessToken { get; set; }
    public string IdToken { get; set; }
    public string RefreshToken { get; set; }
}


