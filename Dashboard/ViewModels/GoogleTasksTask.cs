using Dashboard.Utilities;
using Google.Apis.Tasks.v1.Data;

namespace Dashboard.ViewModels;

public class GoogleTasksTask
{
    private Task task;

    public string Title => task.Title;

    public bool Completed => !task.Completed.IsNullOrEmpty();

    public bool TopLevel => task.Parent.IsNullOrEmpty();

    public string Id => task.Id;

    public string ParentId => task.Parent;

    public string Position => task.Position;

    public GoogleTasksTask(Task _task) => task = _task;
}