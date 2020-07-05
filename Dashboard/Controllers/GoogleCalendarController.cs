using Dashboard.Config;
using Dashboard.Models;
using Dashboard.ServiceProviders;
using Dashboard.Tools;
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

        private ObservableCollection<GoogleCalendarEvent> events = new ObservableCollection<GoogleCalendarEvent>();
        public ObservableCollection<GoogleCalendarEvent> Events
        {
            get => events;
            set => SetAndNotify(ref events, value);
        }

        private bool authorized = false;

        public bool Authorized
        {
            get => authorized;
            set => SetAndNotify(ref authorized, value);
        }

        public GoogleCalendarController()
        {
        }

        public override async void OnInitializationComplete()
        {
            if (Calendar.CanAuthorize)
            {
                if (!Calendar.IsAuthorized)
                    await Calendar.Authorize();
                Authorized = true;
            }
            Colors colors = await Calendar.GetColors();
            var events = await Calendar.GetAllEvents();
            List<GoogleCalendarEvent> tempEvents = new List<GoogleCalendarEvent>();
            foreach (var calendar in events.Keys)
            {
                events[calendar].Items.ForEach(x => tempEvents.Add(new GoogleCalendarEvent(calendar, x, colors)));
            }
            tempEvents.OrderBy(x => x.Start).ForEach(x => Events.Add(x));
        }

        public override void OnInitialize()
        {
            Calendar.RequireScopes(new[] {
                CalendarService.Scope.CalendarReadonly
            });
        }
    }
}
