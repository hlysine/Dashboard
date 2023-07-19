using Dashboard.Config;
using Dashboard.ViewModels;
using Dashboard.Services;
using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dashboard.Components;

public class GoogleGmailComponent : AutoRefreshComponent
{
    public override string DefaultName => "Gmail";

    [RequireService(nameof(GoogleAccountId))]
    public GoogleGmailService Gmail { get; set; }

    [PersistentConfig]
    public string GoogleAccountId { get; set; }

    private Profile profile;
    public Profile Profile
    {
        get => profile;
        set => SetAndNotify(ref profile, value);
    }

    private List<GoogleGmailThread> threads = new();
    public List<GoogleGmailThread> Threads
    {
        get => threads;
        set => SetAndNotify(ref threads, value);
    }

    public GoogleGmailComponent()
    {
    }

    private async Task LoadGmail()
    {
        Profile = await Gmail.GetProfile();
        var th = await Gmail.GetThreads();
        Threads.Clear();
        threads.AddRange(th.Threads.Select(x => new GoogleGmailThread(x, Gmail, Profile)));
        NotifyChanged(nameof(Threads));
    }

    protected override async void OnInitializationComplete()
    {
        if (Gmail.CanAuthorize)
        {
            if (!Gmail.IsAuthorized)
                await Gmail.Authorize();
            await LoadGmail();
            StartAutoRefresh();
        }
        Loaded = true;
    }

    protected override void OnInitialize()
    {
        Gmail.RequireScopes(new[] {
            GmailService.Scope.GmailReadonly,
        });
    }

    protected override async void OnRefresh()
    {
        await LoadGmail();
    }
}