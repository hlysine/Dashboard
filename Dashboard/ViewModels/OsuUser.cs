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
            get => sUser.username;
        }

        public int Rank
        {
            get => getUser()?.statistics?.rank.global ?? sUser.current_mode_rank;
        }

        public bool Online
        {
            get => getUser()?.is_online ?? sUser.is_online;
        }

        public DateTime LastOnline
        {
            get => getUser()?.last_visit.ToLocalTime() ?? sUser.last_visit.ToLocalTime();
        }

        public double? PP
        {
            get => getUser()?.statistics?.pp;
        }

        public double? Accuracy
        {
            get => getUser()?.statistics?.hit_accuracy;
        }

        public string AvatarUrl
        {
            get
            {
                string url = getUser()?.avatar_url ?? sUser.avatar_url;
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
                        var th = osu.GetUser(sUser.id.ToString()).Result;
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
                        Helper.OpenUri(new Uri($"https://osu.ppy.sh/users/{getUser()?.id ?? sUser.id}"));
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

    public class SimpleUser
    {
        public string avatar_url { get; set; }
        public string country_code { get; set; }
        public string default_group { get; set; }
        public int id { get; set; }
        public bool is_active { get; set; }
        public bool is_bot { get; set; }
        public bool is_online { get; set; }
        public bool is_supporter { get; set; }
        public DateTime last_visit { get; set; }
        public bool pm_friends_only { get; set; }
        public object profile_colour { get; set; }
        public string username { get; set; }
        public Country country { get; set; }
        public Cover cover { get; set; }
        public int current_mode_rank { get; set; }
        public List<object> groups { get; set; }
        public int support_level { get; set; }
    }

    public class Kudosu
    {
        public int total { get; set; }
        public int available { get; set; }
    }

    public class Country
    {
        public string code { get; set; }
        public string name { get; set; }
    }

    public class Cover
    {
        public object custom_url { get; set; }
        public string url { get; set; }
        public string id { get; set; }
    }

    public class MonthlyPlaycount
    {
        public string start_date { get; set; }
        public int count { get; set; }
    }

    public class Page
    {
        public string html { get; set; }
        public string raw { get; set; }
    }

    public class Level
    {
        public int current { get; set; }
        public int progress { get; set; }
    }

    public class GradeCounts
    {
        public int ss { get; set; }
        public int ssh { get; set; }
        public int s { get; set; }
        public int sh { get; set; }
        public int a { get; set; }
    }

    public class Rank
    {
        public int global { get; set; }
        public int country { get; set; }
    }

    public class Statistics
    {
        public Level level { get; set; }
        public double pp { get; set; }
        public int pp_rank { get; set; }
        public int ranked_score { get; set; }
        public double hit_accuracy { get; set; }
        public int play_count { get; set; }
        public int play_time { get; set; }
        public int total_score { get; set; }
        public int total_hits { get; set; }
        public int maximum_combo { get; set; }
        public int replays_watched_by_others { get; set; }
        public bool is_ranked { get; set; }
        public GradeCounts grade_counts { get; set; }
        public Rank rank { get; set; }
    }

    public class UserAchievement
    {
        public DateTime achieved_at { get; set; }
        public int achievement_id { get; set; }
    }

    public class RankHistory
    {
        public string mode { get; set; }
        public List<int> data { get; set; }
    }

    public class FullUser
    {
        public string avatar_url { get; set; }
        public string country_code { get; set; }
        public string default_group { get; set; }
        public int id { get; set; }
        public bool is_active { get; set; }
        public bool is_bot { get; set; }
        public bool is_online { get; set; }
        public bool is_supporter { get; set; }
        public DateTime last_visit { get; set; }
        public bool pm_friends_only { get; set; }
        public object profile_colour { get; set; }
        public string username { get; set; }
        public string cover_url { get; set; }
        public object discord { get; set; }
        public bool has_supported { get; set; }
        public object interests { get; set; }
        public DateTime join_date { get; set; }
        public Kudosu kudosu { get; set; }
        public object lastfm { get; set; }
        public object location { get; set; }
        public int max_blocks { get; set; }
        public int max_friends { get; set; }
        public object occupation { get; set; }
        public string playmode { get; set; }
        public object playstyle { get; set; }
        public int post_count { get; set; }
        public List<string> profile_order { get; set; }
        public object skype { get; set; }
        public object title { get; set; }
        public object twitter { get; set; }
        public object website { get; set; }
        public Country country { get; set; }
        public Cover cover { get; set; }
        public List<object> account_history { get; set; }
        public List<object> active_tournament_banner { get; set; }
        public List<object> badges { get; set; }
        public int favourite_beatmapset_count { get; set; }
        public int follower_count { get; set; }
        public int graveyard_beatmapset_count { get; set; }
        public List<object> groups { get; set; }
        public int loved_beatmapset_count { get; set; }
        public List<MonthlyPlaycount> monthly_playcounts { get; set; }
        public Page page { get; set; }
        public List<object> previous_usernames { get; set; }
        public int ranked_and_approved_beatmapset_count { get; set; }
        public List<object> replays_watched_counts { get; set; }
        public int scores_first_count { get; set; }
        public Statistics statistics { get; set; }
        public int support_level { get; set; }
        public int unranked_beatmapset_count { get; set; }
        public List<UserAchievement> user_achievements { get; set; }
        public RankHistory rankHistory { get; set; }
    }
}
