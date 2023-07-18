using Dashboard.Config;
using Dashboard.ViewModels;
using Dashboard.Services;
using Dashboard.Utilities;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dashboard.Components;

public class GoogleCalendarComponent : AutoRefreshComponent
{
    public override string DefaultName => "Google Calendar";

    [RequireService(nameof(GoogleAccountId))]
    public GoogleCalendarService Calendar { get; set; }

    [PersistentConfig]
    public string GoogleAccountId { get; set; }

    private List<GoogleCalendarEvent> events = new();
    public List<GoogleCalendarEvent> Events
    {
        get => events;
        set => SetAndNotify(ref events, value);
    }

    Colors colors;

    public GoogleCalendarComponent()
    {
    }

    private async Task LoadCalendar()
    {
        var events = await Calendar.GetAllEvents();
        Events.Clear();
        List<GoogleCalendarEvent> tempEvents = new();
        foreach (var calendar in events.Keys)
        {
            events[calendar].Items.ForEach(x => tempEvents.Add(new GoogleCalendarEvent(calendar, x, colors)));
        }
        Events.AddRange(tempEvents.OrderBy(x => x.Start));
        NotifyChanged(nameof(Events));
    }

    protected override async void OnInitializationComplete()
    {
        if (Calendar.CanAuthorize)
        {
            if (!Calendar.IsAuthorized)
                await Calendar.Authorize();
            colors = await Calendar.GetColors();
            await LoadCalendar();
            StartAutoRefresh();
        }
        Loaded = true;
    }

    protected override void OnInitialize()
    {
        Calendar.RequireScopes(new[] {
            CalendarService.Scope.CalendarReadonly
        });
    }

    protected override async void OnRefresh()
    {
        await LoadCalendar();
    }
}