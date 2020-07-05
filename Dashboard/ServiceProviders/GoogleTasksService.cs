using Dashboard.Config;
using Dashboard.Tools;
using Google.Apis.Services;
using Google.Apis.Tasks.v1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace Dashboard.ServiceProviders
{
    public class GoogleTasksService : AuthCodeServiceProvider
    {
        [RequireService]
        private GoogleService Google { get; set; }

        // comes from GoogleService

        public override string ClientId { get => Google?.ClientId; }

        public override string ClientSecret { get => Google?.ClientSecret; }

        public override List<string> AuthorizedScopes { get => Google?.AuthorizedScopes ?? new List<string>(); /* To avoid null reference when deserializing XML */ }

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


        public override bool IsAuthorized => tasks != null;

        [Obsolete("RequiredScopes is saved in GoogleService", true)]
        protected new List<string> requiredScopes;

        private TasksService tasks;

        /// <summary>
        /// Start the authroization code flow or request for an access token if a refresh token is present and the scopes match.
        /// </summary>
        /// <param name="cancel">A <see cref="CancellationToken"/> to cancel the wait for users to authorize on their browsers</param>
        /// <exception cref="OperationCanceledException">Thrown if the wait is canceled</exception>
        public override async Task Authorize(CancellationToken cancel = default)
        {
            if (!Google.IsAuthorized)
                await Google.Authorize(cancel);
            tasks = new TasksService(new BaseClientService.Initializer()
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

        public async Task<Google.Apis.Tasks.v1.Data.TaskLists> GetAllTasklists()
        {
            TasklistsResource.ListRequest request = tasks.Tasklists.List();

            return await request.ExecuteAsync();
        }

        /// <summary>
        /// Get all tasks in a tasklist.
        /// </summary>
        /// <param name="calendarId">ID of the tasklist.</param>
        /// <param name="maxResults">Number of tasks to return.</param>
        /// <returns></returns>
        public async Task<Google.Apis.Tasks.v1.Data.Tasks> GetTasks(string tasklistId, int maxResults = 100)
        {
            // Define parameters of request.
            TasksResource.ListRequest request = tasks.Tasks.List(tasklistId);
            request.ShowCompleted = true;
            request.ShowHidden = true;
            request.MaxResults = maxResults;

            // List tasks.
            return await request.ExecuteAsync();
        }

        /// <summary>
        /// Get all tasks in all tasklists.
        /// </summary>
        /// <param name="maxResults">Number of tasks to return for each tasklist.</param>
        public async Task<Dictionary<Google.Apis.Tasks.v1.Data.TaskList, Google.Apis.Tasks.v1.Data.Tasks>> GetAllTasks( int maxResults = 100)
        {
            var tasklists = await GetAllTasklists();

            var ret = new Dictionary<Google.Apis.Tasks.v1.Data.TaskList, Google.Apis.Tasks.v1.Data.Tasks>();
            foreach (var tasklist in tasklists.Items)
            {
                var tasks = await GetTasks(tasklist.Id, maxResults);
                ret.Add(tasklist, tasks);
            }

            return ret;
        }
    }
}
