using Dashboard.Config;
using Dashboard.Models;
using Dashboard.ServiceProviders;
using Dashboard.Utilities;
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
    public class OsuController : DashboardController
    {
        [RequireService]
        public OsuService Osu { get; set; }

        private bool authorized = false;

        public bool Authorized
        {
            get => authorized;
            set => SetAndNotify(ref authorized, value);
        }

        public override TimeSpan ForegroundRefreshRate => TimeSpan.FromSeconds(30);

        private List<OsuUser> friends = new List<OsuUser>();

        public List<OsuUser> Friends
        {
            get => friends;
            set => SetAndNotify(ref friends, value);
        }

        public OsuController()
        {
        }

        public override async void OnInitializationComplete()
        {
            if (Osu.CanAuthorize)
            {
                if (!Osu.IsAuthorized)
                    await Osu.Authorize();
                Authorized = true;
                var fds = await Osu.GetFriends();
                Friends.AddRange(fds.OrderByDescending(x => x.last_visit).Select(x => new OsuUser(x, Osu)));
                NotifyChanged(nameof(Friends));
                Loaded = true;
            }
        }

        public override void OnInitialize()
        {
            Osu.RequireScopes(new[] {
                "identify",
                "friends.read",
                "public"
            });
        }

        public override void OnRefresh()
        {
        }
    }
}
