using System.Text.Json.Serialization;
namespace Bulwark.Auth.Core.Domain;
public class RefreshToken
{
    [JsonPropertyName("jti")]
    public string Jti { get; set; }
    [JsonPropertyName("iss")]
    public string Iss { get; set; }
    [JsonPropertyName("aud")]
    public string Aud { get; set; }
    
    [JsonPropertyName("exp")]
    public int Exp { get; set; }
    [JsonPropertyName("sub")]
    public string Sub { get; set; }
}

