using Dashboard.Config;
using Dashboard.Models;
using Dashboard.ServiceProviders;
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

namespace Dashboard.Controllers
{
    public class GoogleTasksController : DashboardController
    {
        [RequireService]
        public GoogleTasksService Tasks { get; set; }

        private List<GoogleTasksTask> currentTasklist = new List<GoogleTasksTask>();
        public List<GoogleTasksTask> CurrentTasklist
        {
            get => currentTasklist;
            set => SetAndNotify(ref currentTasklist, value);
        }

        private Dictionary<Google.Apis.Tasks.v1.Data.TaskList, List<GoogleTasksTask>> allTasks = new Dictionary<Google.Apis.Tasks.v1.Data.TaskList, List<GoogleTasksTask>>();

        private bool authorized = false;

        public bool Authorized
        {
            get => authorized;
            set => SetAndNotify(ref authorized, value);
        }

        public GoogleTasksController()
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

        public override async void OnInitializationComplete()
        {
            if (Tasks.CanAuthorize)
            {
                if (!Tasks.IsAuthorized)
                    await Tasks.Authorize();
                Authorized = true;
            }
            await LoadTasks();
            Loaded = true;
        }

        public override void OnInitialize()
        {
            Tasks.RequireScopes(new[] {
                TasksService.Scope.TasksReadonly
            });
        }

        public override async void OnRefresh()
        {
            await LoadTasks();
        }
    }
}
