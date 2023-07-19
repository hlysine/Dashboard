using Dashboard.Services;
using Dashboard.Utilities;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Dashboard.ViewModels;

public class OsuUser : NotifyPropertyChanged
{
    private readonly CompactUser compactUser;
    private User fullUser;
    private readonly OsuService osu;

    // default is false, set 1 for true.
    private int fetchedUser;

    private bool FetchedUser
    {
        get => System.Threading.Interlocked.CompareExchange(ref fetchedUser, 1, 1) == 1;
        set
        {
            if (value) System.Threading.Interlocked.CompareExchange(ref fetchedUser, 1, 0);
            else System.Threading.Interlocked.CompareExchange(ref fetchedUser, 0, 1);
        }
    }

    public string Username => compactUser.Username;

    public long? Rank => getUser()?.RankHistory?.Data.Last() ?? compactUser.Statistics.GlobalRank;

    public bool Online => getUser()?.IsOnline ?? compactUser.IsOnline;

    public DateTime? LastOnline => getUser()?.LastVisit?.ToLocalTime() ?? compactUser.LastVisit?.ToLocalTime();

    public double? PP => getUser()?.Statistics?.PP;

    public double? Accuracy => getUser()?.Statistics?.HitAccuracy;

    public string AvatarUrl
    {
        get
        {
            string url = getUser()?.AvatarUrl ?? compactUser.AvatarUrl;

            return Uri.TryCreate(new Uri("https://osu.ppy.sh/"), url, out Uri res) ? res.ToString() : url;
        }
    }

    private User getUser()
    {
        if (fullUser != null)
            return fullUser;
        if (FetchedUser)
            return null;

        FetchedUser = true;
        Task.Run(
            () =>
            {
                User th = osu.GetUser(compactUser.Id.ToString()).Result;
                fullUser = th;
                NotifyChanged(
                    new[]
                    {
                        nameof(Rank),
                        nameof(Online),
                        nameof(LastOnline),
                        nameof(PP),
                        nameof(Accuracy),
                        nameof(AvatarUrl),
                    }
                );
            }
        );

        return null;
    }

    private RelayCommand openCommand;

    public ICommand OpenCommand => openCommand ??= new RelayCommand(
        // execute
        () =>
        {
            Helper.OpenUri(new Uri($"https://osu.ppy.sh/users/{getUser()?.Id ?? compactUser.Id}"));
        },
        // can execute
        () => true
    );

    public OsuUser(CompactUser compactUser, OsuService osu) => (this.compactUser, this.osu) = (compactUser, osu);
}
