using Dashboard.Config;
using Dashboard.Models;
using Dashboard.ServiceProviders;
using Dashboard.Utilities;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
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
    public class GoogleCalendarController : DashboardController
    {
        [RequireService]
        public GoogleCalendarService Calendar { get; set; }

        private List<GoogleCalendarEvent> events = new List<GoogleCalendarEvent>();
        public List<GoogleCalendarEvent> Events
        {
            get => events;
            set => SetAndNotify(ref events, value);
        }

        private bool authorized = false;
        Colors colors;

        public bool Authorized
        {
            get => authorized;
            set => SetAndNotify(ref authorized, value);
        }

        public GoogleCalendarController()
        {
        }

        private async Task LoadCalendar()
        {
            var events = await Calendar.GetAllEvents();
            Events.Clear();
            List<GoogleCalendarEvent> tempEvents = new List<GoogleCalendarEvent>();
            foreach (var calendar in events.Keys)
            {
                events[calendar].Items.ForEach(x => tempEvents.Add(new GoogleCalendarEvent(calendar, x, colors)));
            }
            Events.AddRange(tempEvents.OrderBy(x => x.Start));
            NotifyChanged(nameof(Events));
        }

        public override async void OnInitializationComplete()
        {
            if (Calendar.CanAuthorize)
            {
                if (!Calendar.IsAuthorized)
                    await Calendar.Authorize();
                Authorized = true;
            }
            colors = await Calendar.GetColors();
            await LoadCalendar();
            Loaded = true;
        }

        public override void OnInitialize()
        {
            Calendar.RequireScopes(new[] {
                CalendarService.Scope.CalendarReadonly
            });
        }

        public override async void OnRefresh()
        {
            await LoadCalendar();
        }
    }
}
