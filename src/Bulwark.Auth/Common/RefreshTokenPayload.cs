namespace Bulwark.Auth.Common;
public class RefreshTokenPayload
{
    public string Email { get; set; }
    public string refreshToken { get; set; }
    public string DeviceId { get; set; }
}


