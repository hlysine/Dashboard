using Dashboard.Config;
using Dashboard.ViewModels;
using Dashboard.Utilities;
using Dashboard.Utilities.Auth.Models;
using RestSharp;
using RestSharp.Serialization.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Dashboard.Services
{
    public class OsuService : AuthCodeService
    {
        private RestClient osu;

        public override bool IsAuthorized => osu != null;

        /// <summary>
        /// Start the authroization code flow or request for an access token if a refresh token is present and the scopes match.
        /// </summary>
        /// <param name="cancel">A <see cref="CancellationToken"/> to cancel the wait for users to authorize on their browsers</param>
        /// <exception cref="OperationCanceledException">Thrown if the wait is canceled</exception>
        public override async Task Authorize(CancellationToken cancel = default)
        {
            OsuTokenResponse tokenResponse;
            if (RefreshToken.IsNullOrEmpty() || !requiredScopes.IsSubsetOf(AuthorizedScopes))
            {
                var taskCompletionSource = new TaskCompletionSource<AuthorizationCodeResponse>();

                EmbedIOAuthServer _server = new EmbedIOAuthServer(new Uri("http://localhost:5001/callback"), 5001);
                await _server.Start();

                _server.AuthorizationCodeReceived += (_, response) =>
                {
                    taskCompletionSource.SetResult(response);
                    return Task.CompletedTask;
                };

                var request = new LoginRequest(new Uri("https://osu.ppy.sh/oauth/authorize"), _server.BaseUri, ClientId, LoginRequest.ResponseType.Code)
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

                RestClient osuAuthClient = new RestClient("https://osu.ppy.sh/");

                osuAuthClient.AddHandler("application/json", () => new JsonDeserializer());

                var tokenRequest = new RestRequest("oauth/token/", Method.POST);
                tokenRequest.AddParameter("client_id", ClientId);
                tokenRequest.AddParameter("client_secret", ClientSecret);
                tokenRequest.AddParameter("code", response.Code);
                tokenRequest.AddParameter("grant_type", "authorization_code");
                tokenRequest.AddParameter("redirect_uri", "http://localhost:5001/callback");

                var codeResponse = await osuAuthClient.ExecuteAsync<OsuTokenResponse>(tokenRequest);
                tokenResponse = codeResponse.Data;
            }
            else
            {

                RestClient osuAuthClient = new RestClient("https://osu.ppy.sh/");

                osuAuthClient.AddHandler("application/json", () => new JsonDeserializer());

                var tokenRequest = new RestRequest("oauth/token/", Method.POST);
                tokenRequest.AddParameter("client_id", ClientId);
                tokenRequest.AddParameter("client_secret", ClientSecret);
                tokenRequest.AddParameter("refresh_token", RefreshToken);
                tokenRequest.AddParameter("grant_type", "refresh_token");

                var codeResponse = await osuAuthClient.ExecuteAsync<OsuTokenResponse>(tokenRequest);
                tokenResponse = codeResponse.Data;
            }

            RefreshToken = tokenResponse.refresh_token;
            AccessToken = tokenResponse.access_token;
            AuthorizedScopes = new List<string>(requiredScopes);

            osu = new RestClient("https://osu.ppy.sh/");
            osu.AddDefaultHeader("Authorization", "Bearer " + tokenResponse.access_token);

            RaiseConfigUpdated(EventArgs.Empty);
        }

        public override async Task Unauthorize(CancellationToken cancel = default)
        {
            RefreshToken = null;
            AccessToken = null;
            AuthorizedScopes.Clear();
            RaiseConfigUpdated(EventArgs.Empty);
        }

        public async Task<List<SimpleUser>> GetFriends()
        {
            var request = new RestRequest("api/v2/friends", Method.GET);

            return (await osu.ExecuteAsync<List<SimpleUser>>(request)).Data;
        }

        public async Task<FullUser> GetUser(string userId)
        {
            var request = new RestRequest("api/v2/users/{id}", Method.GET);
            request.AddUrlSegment("id", userId);

            return (await osu.ExecuteAsync<FullUser>(request)).Data;
        }
    }
}
