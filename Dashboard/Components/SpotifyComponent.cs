using Dashboard.Config;
using Dashboard.Services;
using Dashboard.Utilities;
using SpotifyAPI.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Dashboard.Components;

public class SpotifyComponent : AutoRefreshComponent
{
    protected override string DefaultName => "Spotify";

    [RequireService(nameof(SpotifyAccountId))]
    public SpotifyService Spotify { get; set; }

    [PersistentConfig]
    public string SpotifyAccountId { get; set; }

    private FullTrack currentTrack;

    private FullTrack scheduledCheck;

    public FullTrack CurrentTrack
    {
        get => currentTrack;
        set => SetAndNotify(
            ref currentTrack, value, new[]
            {
                nameof(CurrentTrackArtists),
                nameof(CurrentTrackAlbum),
                nameof(CurrentTrackName),
                nameof(CurrentTrackImageUrl),
                nameof(HasTrack),
                nameof(CurrentTrackDuration),
            }
        );
    }

    public TimeSpan CurrentTrackDuration => TimeSpan.FromMilliseconds((CurrentTrack?.DurationMs).GetValueOrDefault());

    public string CurrentTrackArtists => string.Join(", ", CurrentTrack?.Artists.Select(x => x.Name) ?? Array.Empty<string>());

    public string CurrentTrackAlbum => CurrentTrack?.Album.Name;

    public string CurrentTrackName => CurrentTrack?.Name;

    public string CurrentTrackImageUrl => CurrentTrack?.Album.Images.FirstOrDefault()?.Url;

    public bool HasTrack => CurrentTrack != null;

    private bool isPlaying;

    public bool IsPlaying
    {
        get => isPlaying;
        set => SetAndNotify(ref isPlaying, value);
    }

    private bool savedTrack;

    public bool SavedTrack
    {
        get => savedTrack;
        set
        {
            if (savedTrack != value)
            {
                SetAndNotify(ref savedTrack, value);
                updateSavedTrack();
            }
            else
            {
                SetAndNotify(ref savedTrack, value);
            }
        }
    }

    private DateTime startTime;
    private DateTime pauseTime;

    public TimeSpan PlaybackProgress
    {
        get
        {
            if (IsPlaying)
                pauseTime = DateTime.Now;
            TimeSpan progress = pauseTime - startTime;

            return (CurrentTrackDuration > progress) ? progress : CurrentTrackDuration;
        }
    }

    private RelayCommand playPauseCommand;

    public ICommand PlayPauseCommand => playPauseCommand ??= new RelayCommand(
        // execute
        async () =>
        {
            try
            {
                if (isPlaying)
                {
                    IsPlaying = !await Spotify.PausePlayback();
                }
                else
                {
                    IsPlaying = await Spotify.ResumePlayback();
                }

                pauseTime = DateTime.Now;
            }
            catch (APIException)
            {
                // User clicked too frequently, or no active device
            }

            // In case something goes wrong, schedule a check
            _ = Task.Delay(500).ContinueWith(_ => updateCurrentlyPlaying());
        },
        // can execute
        () => HasTrack
    );

    private RelayCommand radioCommand;

    public ICommand RadioCommand => radioCommand ??= new RelayCommand(
        // execute
        async () =>
        {
            // build radio list
            RecommendationsResponse recommendations = await Spotify.GetRecommendations(new[] { CurrentTrack.Id }, 100);
            List<string> playlist = recommendations.Tracks.Select(x => x.Uri).ToList();
            playlist.RemoveAll(x => x == CurrentTrack.Uri);
            playlist.Insert(0, CurrentTrack.Uri);

            // set shuffle and repeat
            try
            {
                await Spotify.SetShuffle(false);
            }
            catch (APIException)
            {
                // May fail if user is playing a radio
            }

            try
            {
                await Spotify.SetRepeat(PlayerSetRepeatRequest.State.Off);
            }
            catch (APIException)
            {
            }

            // play the list
            try
            {
                await Spotify.StartPlayback(playlist);
            }
            catch (APIException)
            {
            }

            // schedule a check after some time, allowing the track to start playing
            _ = Task.Delay(500).ContinueWith(_ => updateCurrentlyPlaying());
        },
        // can execute
        () => HasTrack
    );

    public override TimeSpan ForegroundRefreshRate => TimeSpan.FromMilliseconds(100);

    protected override async void OnInitializeSelf()
    {
        if (Spotify.CanAuthorize)
        {
            if (!Spotify.IsAuthorized)
                await Spotify.Authorize();
            updateCurrentlyPlaying();
            StartAutoRefresh();
        }

        Loaded = true;
    }

    protected override void OnInitializeDependencies()
    {
        Spotify.RequireScopes(
            new[]
            {
                Scopes.UserLibraryRead,
                Scopes.UserLibraryModify,
                Scopes.UserReadCurrentlyPlaying,
                Scopes.UserReadPlaybackPosition,
                Scopes.UserReadPlaybackState,
                Scopes.UserReadRecentlyPlayed,
                Scopes.UserModifyPlaybackState,
            }
        );
    }

    protected override void OnForegroundChanged()
    {
        base.OnForegroundChanged();
        if (Foreground)
            updateCurrentlyPlaying();
    }

    private int tick;

    protected override void OnRefresh()
    {
        NotifyChanged(nameof(PlaybackProgress));
        tick++;

        if (tick < 100)
            return;

        updateCurrentlyPlaying();
        tick = 0;
    }

    private async void updateCurrentlyPlaying()
    {
        CurrentlyPlaying currentlyPlaying = await Spotify.GetCurrentlyPlaying();

        if (currentlyPlaying?.Item.Type != ItemType.Track)
            return;

        CurrentTrack = (FullTrack)currentlyPlaying.Item;
        IsPlaying = currentlyPlaying.IsPlaying;
        pauseTime = DateTime.Now;
        startTime = pauseTime - TimeSpan.FromMilliseconds(currentlyPlaying.ProgressMs.GetValueOrDefault());
        savedTrack = (await Spotify.IsInLibrary(new[] { currentTrack.Id })).FirstOrDefault();
        NotifyChanged(nameof(SavedTrack));

        if (scheduledCheck == currentTrack)
            return;

        _ = Task.Delay(CurrentTrack.DurationMs - currentlyPlaying.ProgressMs.GetValueOrDefault() + 100).ContinueWith(_ => updateCurrentlyPlaying());
        scheduledCheck = currentTrack;
    }

    private async void updateSavedTrack()
    {
        if (SavedTrack)
        {
            await Spotify.AddToLibrary(new[] { CurrentTrack.Id });
        }
        else
        {
            await Spotify.RemoveFromLibrary(new[] { CurrentTrack.Id });
        }

        // In case something goes wrong, schedule a check
        _ = Task.Delay(500).ContinueWith(_ => updateCurrentlyPlaying());
    }
}
