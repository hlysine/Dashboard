﻿using Dashboard.Config;
using Dashboard.ViewModels;
using Dashboard.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dashboard.Components;

public class OsuComponent : AutoRefreshComponent
{
    public override string DefaultName => "osu!";

    [RequireService(nameof(OsuAccountId))]
    public OsuService Osu { get; set; }

    [PersistentConfig]
    public string OsuAccountId { get; set; }

    public override TimeSpan ForegroundRefreshRate => TimeSpan.FromSeconds(30);

    private List<OsuUser> friends = new();

    public List<OsuUser> Friends
    {
        get => friends;
        set => SetAndNotify(ref friends, value);
    }

    public OsuComponent()
    {
    }

    private async Task LoadFriends()
    {
        List<CompactUser> fds = await Osu.GetFriends();
        if (fds == null)
        {
            await Task.Delay(500);
            fds = await Osu.GetFriends();
        }

        Friends.Clear();
        if (fds != null)
            Friends.AddRange(fds.OrderByDescending(x => x.LastVisit).Select(x => new OsuUser(x, Osu)));
        NotifyChanged(nameof(Friends));
    }

    protected override async void OnInitializeSelf()
    {
        if (Osu.CanAuthorize)
        {
            if (!Osu.IsAuthorized)
                await Osu.Authorize();
            await LoadFriends();
            StartAutoRefresh();
        }

        Loaded = true;
    }

    protected override void OnInitializeDependencies()
    {
        Osu.RequireScopes(new[]
        {
            "identify",
            "friends.read",
            "public",
        });
    }

    protected override async void OnRefresh()
    {
        await LoadFriends();
    }
}