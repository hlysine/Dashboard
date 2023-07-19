using Dashboard.Config;
using Dashboard.ViewModels;
using Dashboard.Services;
using Dashboard.Utilities;
using Google.Apis.Tasks.v1;
using System.Collections.Generic;
using System.Linq;
using Google.Apis.Tasks.v1.Data;
using Task = System.Threading.Tasks.Task;

namespace Dashboard.Components;

public class GoogleTasksComponent : AutoRefreshComponent
{
    protected override string DefaultName => "Google Tasks";

    [RequireService(nameof(GoogleAccountId))]
    public GoogleTasksService Tasks { get; set; }

    [PersistentConfig]
    public string GoogleAccountId { get; set; }

    private List<GoogleTasksTask> currentTaskList = new();

    public List<GoogleTasksTask> CurrentTaskList
    {
        get => currentTaskList;
        set => SetAndNotify(ref currentTaskList, value);
    }

    private readonly Dictionary<TaskList, List<GoogleTasksTask>> allTasks = new();

    private async Task loadTasks()
    {
        Dictionary<TaskList, Tasks> tasks = await Tasks.GetAllTasks();
        allTasks.Clear();
        foreach (TaskList taskList in tasks.Keys)
        {
            var convertedTasks = new List<GoogleTasksTask>();
            var tmp = new List<GoogleTasksTask>();
            allTasks.Add(taskList, convertedTasks);
            tasks[taskList].Items?.ForEach(x => tmp.Add(new GoogleTasksTask(x)));
            List<IGrouping<string, GoogleTasksTask>> groups = tmp.GroupBy(x => x.ParentId).ToList();
            convertedTasks.AddRange(
                groups.Where(x => x.Key == null)
                      .SelectMany(x => x)
                      .OrderBy(x => x.Position)
            );
            groups.Where(x => x.Key != null)
                  .ForEach(
                      x =>
                          convertedTasks.InsertRange(
                              convertedTasks.FindIndex(y => y.Id == x.Key) + 1,
                              x.OrderBy(y => y.Position)
                          )
                  );
        }

        CurrentTaskList = allTasks.Values.First();
        NotifyChanged(nameof(CurrentTaskList));
    }

    protected override async void OnInitializeSelf()
    {
        if (Tasks.CanAuthorize)
        {
            if (!Tasks.IsAuthorized)
                await Tasks.Authorize();
            await loadTasks();
            StartAutoRefresh();
        }

        Loaded = true;
    }

    protected override void OnInitializeDependencies()
    {
        Tasks.RequireScopes(
            new[]
            {
                TasksService.Scope.TasksReadonly,
            }
        );
    }

    protected override async void OnRefresh()
    {
        await loadTasks();
    }
}
