using Dashboard.Services;
using Dashboard.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Dashboard.ViewModels
{
    public class OsuUser : NotifyPropertyChanged
    {
        private SimpleUser sUser;
        private FullUser fUser;
        private OsuService osu;

        // default is false, set 1 for true.
        private int fetchedUser = 0;

        private bool FetchedUser
        {
            get { return System.Threading.Interlocked.CompareExchange(ref fetchedUser, 1, 1) == 1; }
            set
            {
                if (value) System.Threading.Interlocked.CompareExchange(ref fetchedUser, 1, 0);
                else System.Threading.Interlocked.CompareExchange(ref fetchedUser, 0, 1);
            }
        }

        public string Username
        {
            get => sUser.Username;
        }

        public long Rank
        {
            get => getUser()?.Statistics?.Rank.Global ?? sUser.CurrentModeRank;
        }

        public bool Online
        {
            get => getUser()?.IsOnline ?? sUser.IsOnline;
        }

        public DateTime LastOnline
        {
            get => getUser()?.LastVisit.ToLocalTime() ?? sUser.LastVisit.ToLocalTime();
        }

        public double? PP
        {
            get => getUser()?.Statistics?.PP;
        }

        public double? Accuracy
        {
            get => getUser()?.Statistics?.HitAccuracy;
        }

        public string AvatarUrl
        {
            get
            {
                string url = getUser()?.AvatarUrl ?? sUser.AvatarUrl;
                if (Uri.TryCreate(new Uri("https://osu.ppy.sh/"), url, out Uri res)) return res.ToString();
                else return url;
            }
        }

        private FullUser getUser()
        {
            if (fUser == null)
            {
                if (!FetchedUser)
                {
                    FetchedUser = true;
                    Task.Run(() =>
                    {
                        var th = osu.GetUser(sUser.Id.ToString()).Result;
                        fUser = th;
                        NotifyChanged(new[] {
                            nameof(Rank),
                            nameof(Online),
                            nameof(LastOnline),
                            nameof(PP),
                            nameof(Accuracy),
                            nameof(AvatarUrl),
                        });
                    });
                }
                return null;
            }
            else
            {
                return fUser;
            }
        }

        private RelayCommand openCommand;

        public ICommand OpenCommand
        {
            get
            {
                return openCommand ?? (openCommand = new RelayCommand(
                    // execute
                    () =>
                    {
                        Helper.OpenUri(new Uri($"https://osu.ppy.sh/users/{getUser()?.Id ?? sUser.Id}"));
                    },
                    // can execute
                    () =>
                    {
                        return true;
                    }
                ));
            }
        }

        public OsuUser(SimpleUser _sUser, OsuService _osu) => (sUser, osu) = (_sUser, _osu);
    }
}
