// Taken from https://github.com/JohnnyCrazy/SpotifyAPI-NET/blob/master/SpotifyAPI.Web.Auth/Models/Response/AuthorizationCodeResponse.cs

namespace Dashboard.Utilities.Auth.Models;

public class AuthorizationCodeResponse
{
    public AuthorizationCodeResponse(string code)
    {
        Code = code;
    }

    public string Code { get; set; } = default!;
    public string State { get; set; } = default!;
}