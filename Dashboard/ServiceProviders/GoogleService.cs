using Dashboard.Config;
using Dashboard.Tools;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Util.Store;
using SpotifyAPI.Web;
using SpotifyAPI.Web.Auth;
using Swan.Parsers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Dashboard.ServiceProviders
{
    public class GoogleService : AuthCodeServiceProvider
    {
        // Properties that are not used since the Google client libraries handle the credentials themselves
        [Obsolete("This property is not used", true)]
        public new string AccessToken { get; set; }
        [Obsolete("This property is not used", true)]
        public new string RefreshToken { get; set; }

        public override bool IsAuthorized => credential != null;

        private readonly string credPath = "token.json".ToAbsolutePath();

        private UserCredential credential;

        /// <summary>
        /// Start the authroization code flow or request for an access token if a refresh token is present and the scopes match.
        /// </summary>
        /// <param name="cancel">A <see cref="CancellationToken"/> to cancel the wait for users to authorize on their browsers</param>
        /// <exception cref="OperationCanceledException">Thrown if the wait is canceled</exception>
        public override async Task Authorize(CancellationToken cancel = default)
        {
            // The file token.json stores the user's access and refresh tokens, and is created
            // automatically when the authorization flow completes for the first time.
            credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                new ClientSecrets
                {
                    ClientId = ClientId,
                    ClientSecret = ClientSecret
                },
                requiredScopes,
                "user",
                cancel,
                new FileDataStore(credPath, true));
            RaiseConfigUpdated(EventArgs.Empty);
        }

        public override async Task Unauthorize(CancellationToken cancel = default)
        {
            //TODO: delete saved token.json
            if (File.Exists(credPath))
                File.Delete(credPath);
            RaiseConfigUpdated(EventArgs.Empty);
        }

        public UserCredential GetCredential()
        {
            return credential;
        }
    }
}
