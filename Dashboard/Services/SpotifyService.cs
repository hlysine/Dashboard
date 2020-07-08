using Dashboard.Config;
using Dashboard.Utilities;
using Dashboard.Utilities.Auth.Models;
using SpotifyAPI.Web;
using Swan.Parsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Dashboard.Services
{
    public class SpotifyService : AuthCodeService
    {
        private SpotifyClient spotify;

        public override bool IsAuthorized => spotify != null;

        /// <summary>
        /// Start the authroization code flow or request for an access token if a refresh token is present and the scopes match.
        /// </summary>
        /// <param name="cancel">A <see cref="CancellationToken"/> to cancel the wait for users to authorize on their browsers</param>
        /// <exception cref="OperationCanceledException">Thrown if the wait is canceled</exception>
        public override async Task Authorize(CancellationToken cancel = default)
        {
            AuthorizationCodeTokenResponse tokenResponse;
            if (RefreshToken.IsNullOrEmpty() || !requiredScopes.IsSubsetOf(AuthorizedScopes))
            {
                var taskCompletionSource = new TaskCompletionSource<AuthorizationCodeResponse>();

                EmbedIOAuthServer _server = new EmbedIOAuthServer(new Uri("http://localhost:5000/callback"), 5000);
                await _server.Start();

                _server.AuthorizationCodeReceived += (_, response) =>
                {
                    taskCompletionSource.SetResult(response);
                    return Task.CompletedTask;
                };

                var request = new SpotifyAPI.Web.LoginRequest(_server.BaseUri, ClientId, SpotifyAPI.Web.LoginRequest.ResponseType.Code)
                {
                    Scope = requiredScopes
                };
                Helper.OpenUri(request.ToUri());

                while (!taskCompletionSource.Task.IsCompleted)
                {
                    cancel.ThrowIfCancellationRequested();
                    await Task.Delay(500);
                }

                await _server.Stop();

                var response = taskCompletionSource.Task.Result;
                tokenResponse = await new OAuthClient().RequestToken(
                  new AuthorizationCodeTokenRequest(
                    ClientId, ClientSecret, response.Code, new Uri("http://localhost:5000/callback")
                  )
                );
                RefreshToken = tokenResponse.RefreshToken;
            }
            else
            {
                var response = await new OAuthClient().RequestToken(new AuthorizationCodeRefreshRequest(ClientId, ClientSecret, RefreshToken));
                tokenResponse = new AuthorizationCodeTokenResponse()
                {
                    RefreshToken = RefreshToken,
                    AccessToken = response.AccessToken,
                    CreatedAt = response.CreatedAt,
                    ExpiresIn = response.ExpiresIn,
                    Scope = response.Scope,
                    TokenType = response.TokenType
                };
            }
            AccessToken = tokenResponse.AccessToken;
            AuthorizedScopes = tokenResponse.Scope.Split(' ').ToList();
            var config = SpotifyClientConfig
              .CreateDefault()
              .WithAuthenticator(new AuthorizationCodeAuthenticator(ClientId, ClientSecret, tokenResponse));

            spotify = new SpotifyClient(tokenResponse.AccessToken);
            RaiseConfigUpdated(EventArgs.Empty);
        }

        public override async Task Unauthorize(CancellationToken cancel = default)
        {
            //TODO: delete saved RefreshToken
            RefreshToken = null;
            AccessToken = null;
            AuthorizedScopes.Clear();
            RaiseConfigUpdated(EventArgs.Empty);
        }

        public async Task<CurrentlyPlaying> GetCurrentlyPlaying()
        {
            return await spotify.Player.GetCurrentlyPlaying(new PlayerCurrentlyPlayingRequest());
        }

        public async Task<bool> ResumePlayback()
        {
            return await spotify.Player.ResumePlayback();
        }

        public async Task<bool> PausePlayback()
        {
            return await spotify.Player.PausePlayback();
        }
    }
}
