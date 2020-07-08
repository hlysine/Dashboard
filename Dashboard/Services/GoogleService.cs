using Dashboard.Config;
using Dashboard.Utilities;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Dashboard.Services
{
    public class GoogleService : AuthCodeService
    {
        // Properties that are not used since the Google client libraries handle the credentials themselves
        [Obsolete("This property is not used", true)]
        public new string AccessToken { get; set; }
        [Obsolete("This property is not used", true)]
        public new string RefreshToken { get; set; }

        public override bool IsAuthorized => credential != null;

        private readonly string credPath = "google-tokens".ToAbsolutePath();

        private readonly SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);

        private UserCredential credential;

        /// <summary>
        /// Start the authroization code flow or request for an access token if a refresh token is present and the scopes match.
        /// </summary>
        /// <param name="cancel">A <see cref="CancellationToken"/> to cancel the wait for users to authorize on their browsers</param>
        /// <exception cref="OperationCanceledException">Thrown if the wait is canceled</exception>
        public override async Task Authorize(CancellationToken cancel = default)
        {
            // Use a semaphore to avoid opening multiple consent screens when multiple Google services are authenticating
            await semaphore.WaitAsync();
            try
            {
                if (!requiredScopes.IsSubsetOf(AuthorizedScopes))
                {
                    await Unauthorize();
                }

                // TODO: use custom DataStore

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
                AuthorizedScopes = requiredScopes;
                RaiseConfigUpdated(EventArgs.Empty);
            }
            finally
            {
                semaphore.Release();
            }
        }

        public override async Task Unauthorize(CancellationToken cancel = default)
        {
            //TODO: delete saved token.json
            if (Directory.Exists(credPath))
                Directory.Delete(credPath, true);
            AuthorizedScopes.Clear();
            RaiseConfigUpdated(EventArgs.Empty);
        }

        public UserCredential GetCredential()
        {
            return credential;
        }
    }
}
