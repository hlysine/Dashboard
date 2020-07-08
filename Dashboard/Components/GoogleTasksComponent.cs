using Dashboard.Config;
using Dashboard.ViewModels;
using Dashboard.Services;
using Dashboard.Utilities;
using Google.Apis.Tasks.v1;
using SpotifyAPI.Web;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Dashboard.Components
{
    public class GoogleTasksComponent : AutoRefreshComponent
    {
        [RequireService(nameof(GoogleAccountId))]
        public GoogleTasksService Tasks { get; set; }

        [PersistentConfig]
        public string GoogleAccountId { get; set; }

        private List<GoogleTasksTask> currentTasklist = new List<GoogleTasksTask>();
        public List<GoogleTasksTask> CurrentTasklist
        {
            get => currentTasklist;
            set => SetAndNotify(ref currentTasklist, value);
        }

        private Dictionary<Google.Apis.Tasks.v1.Data.TaskList, List<GoogleTasksTask>> allTasks = new Dictionary<Google.Apis.Tasks.v1.Data.TaskList, List<GoogleTasksTask>>();

        public GoogleTasksComponent()
        {
        }

        private async Task LoadTasks()
        {
            var tasks = await Tasks.GetAllTasks();
            allTasks.Clear();
            foreach (var tasklist in tasks.Keys)
            {
                var convertedTasks = new List<GoogleTasksTask>();
                var tmp = new List<GoogleTasksTask>();
                allTasks.Add(tasklist, convertedTasks);
                tasks[tasklist].Items?.ForEach(x => tmp.Add(new GoogleTasksTask(x)));
                var groups = tmp.GroupBy(x => x.ParentId);
                convertedTasks.AddRange(groups.Where(x => x.Key == null).SelectMany(x => x).OrderBy(x => x.Position));
                groups.Where(x => x.Key != null).ForEach(x => convertedTasks.InsertRange(convertedTasks.FindIndex(y => y.Id == x.Key) + 1, x.OrderBy(x => x.Position)));
            }
            CurrentTasklist = allTasks.Values.Last();
            NotifyChanged(nameof(CurrentTasklist));
        }

        protected override async void OnInitializationComplete()
        {
            if (Tasks.CanAuthorize)
            {
                if (!Tasks.IsAuthorized)
                    await Tasks.Authorize();
                await LoadTasks();
            }
            Loaded = true;
        }

        protected override void OnInitialize()
        {
            Tasks.RequireScopes(new[] {
                TasksService.Scope.TasksReadonly
            });
        }

        protected override async void OnRefresh()
        {
            await LoadTasks();
        }
    }
}
