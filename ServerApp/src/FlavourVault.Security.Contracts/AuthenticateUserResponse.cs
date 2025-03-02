using System.Text.Json.Serialization;

namespace FlavourVault.Security.Contracts;

public class AuthenticateUserResponse
{
    /// <summary>
    /// Access token to be used in api header to get data
    /// </summary>        
    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; }

    /// <summary>
    /// Type of token e.g. bearer
    /// </summary>
    [JsonPropertyName("token_type")]
    public string TokenType { get; set; } = "Bearer";

    /// <summary>
    /// The lifetime in seconds of the access token.
    /// </summary>
    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; set; }

    [JsonPropertyName("refresh_token")]
    public string RefreshToken { get; set; }
}
