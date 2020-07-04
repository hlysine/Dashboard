using Dashboard.Config;
using Dashboard.ServiceProviders;
using Dashboard.Tools;
using Google.Apis.Calendar.v3;
using SpotifyAPI.Web;
using System;
using System.Collections.Generic;
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
        }

        public override void OnInitialize()
        {
            Calendar.RequireScopes(new[] {
                CalendarService.Scope.CalendarReadonly
            });
        }
    }
}
