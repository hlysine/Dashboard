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
    protected override string DefaultName => "Google Calendar";

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

    private Colors colors;

    private async Task loadCalendar()
    {
        Dictionary<CalendarListEntry, Events> downloadedEvents = await Calendar.GetAllEvents();
        Events.Clear();
        List<GoogleCalendarEvent> tempEvents = new();
        foreach (CalendarListEntry calendar in downloadedEvents.Keys)
        {
            downloadedEvents[calendar].Items.ForEach(x => tempEvents.Add(new GoogleCalendarEvent(calendar, x, colors)));
        }

        Events.AddRange(tempEvents.OrderBy(x => x.Start));
        NotifyChanged(nameof(Events));
    }

    protected override async void OnInitializeSelf()
    {
        if (Calendar.CanAuthorize)
        {
            if (!Calendar.IsAuthorized)
                await Calendar.Authorize();
            colors = await Calendar.GetColors();
            await loadCalendar();
            StartAutoRefresh();
        }

        Loaded = true;
    }

    protected override void OnInitializeDependencies()
    {
        Calendar.RequireScopes(
            new[]
            {
                CalendarService.Scope.CalendarReadonly,
            }
        );
    }

    protected override async void OnRefresh()
    {
        await loadCalendar();
    }
}
