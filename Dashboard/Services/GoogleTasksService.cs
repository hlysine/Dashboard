using Dashboard.Config;
using Dashboard.Utilities;
using Google.Apis.Services;
using Google.Apis.Tasks.v1;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Tasks.v1.Data;
using Task = System.Threading.Tasks.Task;

namespace Dashboard.Services;

public class GoogleTasksService : AuthCodeService
{
    [RequireService(nameof(Id))]
    private GoogleService Google { get; set; }

    // Comes from GoogleService

    public override string ClientId => Google?.ClientId;

    public override string ClientSecret => Google?.ClientSecret;

    public override List<string> AuthorizedScopes => Google?.AuthorizedScopes ?? new List<string>() /* Prevents null ref when deserializing */;

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
    protected new List<string> RequiredScopes;

    private TasksService tasks;

    /// <summary>
    /// Start the authorization code flow or request for an access token if a refresh token is present and the scopes match.
    /// </summary>
    /// <param name="cancel">A <see cref="CancellationToken"/> to cancel the wait for users to authorize on their browsers</param>
    /// <exception cref="OperationCanceledException">Thrown if the wait is canceled</exception>
    public override async Task Authorize(CancellationToken cancel = default)
    {
        if (!Google.IsAuthorized)
            await Google.Authorize(cancel);
        tasks = new TasksService(
            new BaseClientService.Initializer
            {
                HttpClientInitializer = Google.GetCredential(),
                ApplicationName = Helper.GetProductName(),
            }
        );
    }

    /// <summary>
    /// Note: unauthorizing one Google service causes all services to be unauthorized and the user has to authorize other services again.
    /// </summary>
    /// <param name="cancel">A <see cref="CancellationToken"/> to cancel the operation (may not be cancelable)</param>
    public override async Task Unauthorize(CancellationToken cancel = default)
    {
        await Google.Unauthorize(cancel);
    }

    public async Task<TaskLists> GetAllTaskLists()
    {
        TasklistsResource.ListRequest request = tasks.Tasklists.List();

        return await request.ExecuteAsync();
    }

    /// <summary>
    /// Get all tasks in a task list.
    /// </summary>
    /// <param name="taskListId">ID of the task list.</param>
    /// <param name="maxResults">Number of tasks to return.</param>
    /// <returns></returns>
    public async Task<Tasks> GetTasks(string taskListId, int maxResults = 100)
    {
        // Define parameters of request.
        TasksResource.ListRequest request = tasks.Tasks.List(taskListId);
        request.ShowCompleted = true;
        request.ShowHidden = true;
        request.MaxResults = maxResults;

        // List tasks.
        return await request.ExecuteAsync();
    }

    /// <summary>
    /// Get all tasks in all task lists.
    /// </summary>
    /// <param name="maxResults">Number of tasks to return for each task list.</param>
    public async Task<Dictionary<TaskList, Tasks>> GetAllTasks(int maxResults = 100)
    {
        TaskLists taskLists = await GetAllTaskLists();

        var ret = new Dictionary<TaskList, Tasks>();
        foreach (TaskList taskList in taskLists.Items)
        {
            Tasks downloadedTasks = await GetTasks(taskList.Id, maxResults);
            ret.Add(taskList, downloadedTasks);
        }

        return ret;
    }
}
