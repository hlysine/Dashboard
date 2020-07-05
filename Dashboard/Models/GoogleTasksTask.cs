using Dashboard.Utilities;
using Google.Apis.Tasks.v1.Data;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace Dashboard.Models
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
