using Dashboard.Utilities;
using Google.Apis.Tasks.v1.Data;

namespace Dashboard.ViewModels
{
    public class GoogleTasksTask
    {
        private Task task;

        public string Title { get => task.Title; }

        public bool Completed { get => !task.Completed.IsNullOrEmpty(); }

        public bool TopLevel { get => task.Parent.IsNullOrEmpty(); }

        public string Id { get => task.Id; }

        public string ParentId { get => task.Parent; }

        public string Position { get => task.Position; }

        public GoogleTasksTask(Task _task) => task = _task;
    }
}
