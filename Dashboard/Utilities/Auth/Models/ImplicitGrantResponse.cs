// Taken from https://github.com/JohnnyCrazy/SpotifyAPI-NET/blob/master/SpotifyAPI.Web.Auth/Models/Response/ImplicitGrantResponse.cs

using System;

namespace Dashboard.Utilities.Auth.Models;

public class ImplicitGrantResponse
{
    public string AccessToken { get; set; }
    public string TokenType { get; set; }
    public int ExpiresIn { get; set; }
    public string State { get; set; } = default!;

    /// <summary>
    ///   Auto-initialized to UTC Now
    /// </summary>
    /// <value></value>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public bool IsExpired => CreatedAt.AddSeconds(ExpiresIn) <= DateTime.UtcNow;

    public ImplicitGrantResponse(string accessToken, string tokenType, int expiresIn)
    {
        AccessToken = accessToken;
        TokenType = tokenType;
        ExpiresIn = expiresIn;
    }
}
