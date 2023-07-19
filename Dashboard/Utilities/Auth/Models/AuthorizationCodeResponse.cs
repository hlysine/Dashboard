// Taken from https://github.com/JohnnyCrazy/SpotifyAPI-NET/blob/master/SpotifyAPI.Web.Auth/Models/Response/AuthorizationCodeResponse.cs

namespace Dashboard.Utilities.Auth.Models;

public class AuthorizationCodeResponse
{
    public string Code { get; set; }
    public string State { get; set; } = default!;

    public AuthorizationCodeResponse(string code)
    {
        Code = code;
    }
}
