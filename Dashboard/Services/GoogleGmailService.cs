using Dashboard.Config;
using Dashboard.Utilities;
using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using Google.Apis.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace Dashboard.Services
{
    public class GoogleGmailService : AuthCodeService
    {
        [RequireService(nameof(Id))]
        private GoogleService Google { get; set; }

        // Comes from GoogleService

        public override string ClientId { get => Google?.ClientId; }

        public override string ClientSecret { get => Google?.ClientSecret; }

        public override List<string> AuthorizedScopes { get => Google?.AuthorizedScopes ?? new List<string>()  /* Prevents null ref when deserializing */; }

        /// <summary>
        /// Set the list of scopes required. To be called before <see cref="Authorize(CancellationToken)"/>.
        /// </summary>
        /// <param name="scopes">The list of scopes</param>
        public override void RequireScopes(string[] scopes)
        {
            Google?.RequireScopes(scopes);
        }

        // Properties that are not used since the Google client libraries handle the credentials themselves

        [Obsolete("This property is not used", true)]
        public new string AccessToken { get; set; }

        [Obsolete("This property is not used", true)]
        public new string RefreshToken { get; set; }


        public override bool IsAuthorized => gmail != null;

        [Obsolete("RequiredScopes is saved in GoogleService", true)]
        protected new List<string> requiredScopes;

        private GmailService gmail;

        /// <summary>
        /// Start the authroization code flow or request for an access token if a refresh token is present and the scopes match.
        /// </summary>
        /// <param name="cancel">A <see cref="CancellationToken"/> to cancel the wait for users to authorize on their browsers</param>
        /// <exception cref="OperationCanceledException">Thrown if the wait is canceled</exception>
        public override async Task Authorize(CancellationToken cancel = default)
        {
            if (!Google.IsAuthorized)
                await Google.Authorize(cancel);
            gmail = new GmailService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = Google.GetCredential(),
                ApplicationName = Helper.GetProductName(),
            });
        }

        /// <summary>
        /// Note: unauthorizing one Google service causes all services to be unauthorized and the user has to authorize other services again.
        /// </summary>
        /// <param name="cancel">A <see cref="CancellationToken"/> to cancel the operation (may not be cancelable)</param>
        public override async Task Unauthorize(CancellationToken cancel = default)
        {
            await Google.Unauthorize();
        }

        public async Task<Profile> GetProfile()
        {
            UsersResource.GetProfileRequest request = gmail.Users.GetProfile("me");

            return await request.ExecuteAsync();
        }

        public async Task<ListThreadsResponse> GetThreads()
        {
            UsersResource.ThreadsResource.ListRequest request = gmail.Users.Threads.List("me");

            return await request.ExecuteAsync();
        }

        /// <summary>
        /// Get details of a thread
        /// </summary>
        /// <param name="threadId">ID of the thread.</param>
        /// <returns></returns>
        public async Task<Google.Apis.Gmail.v1.Data.Thread> GetThread(string threadId)
        {
            UsersResource.ThreadsResource.GetRequest request = gmail.Users.Threads.Get("me", threadId);
            request.Format = UsersResource.ThreadsResource.GetRequest.FormatEnum.Metadata;

            return await request.ExecuteAsync();
        }
    }
}
