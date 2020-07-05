using Dashboard.Config;
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
    public class SpotifyController : DashboardController
    {
        [RequireService]
        public SpotifyService Spotify { get; set; }

        private bool authorized = false;

        public bool Authorized
        {
            get => authorized;
            set => SetAndNotify(ref authorized, value);
        }

        private FullTrack currentTrack;

        public FullTrack CurrentTrack
        {
            get => currentTrack;
            set => SetAndNotify(ref currentTrack, value, new[] {
                nameof(CurrentTrackArtists),
                nameof(CurrentTrackAlbum),
                nameof(CurrentTrackName),
                nameof(CurrentTrackImageUrl),
                nameof(HasTrack)
            });
        }

        public string CurrentTrackArtists
        {
            get => string.Join(", ", CurrentTrack?.Artists.Select(x => x.Name) ?? new string[] { });
        }

        public string CurrentTrackAlbum
        {
            get => CurrentTrack?.Album.Name;
        }

        public string CurrentTrackName
        {
            get => CurrentTrack?.Name;
        }

        public string CurrentTrackImageUrl
        {
            get => CurrentTrack?.Album.Images.FirstOrDefault()?.Url;
        }

        public bool HasTrack
        {
            get => CurrentTrack != null;
        }

        private bool isPlaying = false;

        public bool IsPlaying
        {
            get => isPlaying;
            set => SetAndNotify(ref isPlaying, value);
        }

        public override TimeSpan ForegroundRefreshRate => TimeSpan.FromSeconds(10);

        private RelayCommand playPauseCommand;

        public ICommand PlayPauseCommand
        {
            get
            {
                return playPauseCommand ?? (playPauseCommand = new RelayCommand(
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
                        }
                        catch (Exception _)
                        {
                            // User clicked too frequently, or no active device
                        }
                        // In case something goes wrong, schedule a check
                        _ = Task.Delay(500).ContinueWith(_ => UpdateCurrentlyPlaying());
                    },
                    // can execute
                    () =>
                    {
                        // TODO: can't play when there's no active device?
                        return HasTrack;
                    }
                ));
            }
        }

        public SpotifyController()
        {
        }

        public override async void OnInitializationComplete()
        {
            if (Spotify.CanAuthorize)
            {
                if (!Spotify.IsAuthorized)
                    await Spotify.Authorize();
                Authorized = true;
                UpdateCurrentlyPlaying();
                Loaded = true;
            }
        }

        public override void OnInitialize()
        {
            Spotify.RequireScopes(new[] {
                Scopes.UserLibraryRead,
                Scopes.UserReadCurrentlyPlaying,
                Scopes.UserReadPlaybackPosition,
                Scopes.UserReadPlaybackState,
                Scopes.UserReadRecentlyPlayed,
                Scopes.UserModifyPlaybackState
            });
        }

        public override void OnRefresh()
        {
            UpdateCurrentlyPlaying();
        }

        private async void UpdateCurrentlyPlaying()
        {
            var currentlyPlaying = await Spotify.GetCurrentlyPlaying();
            if (currentlyPlaying?.Item?.Type == ItemType.Track)
            {
                CurrentTrack = (FullTrack)currentlyPlaying.Item;
                IsPlaying = currentlyPlaying.IsPlaying;
                _ = Task.Delay(CurrentTrack.DurationMs - currentlyPlaying.ProgressMs.GetValueOrDefault() + 100).ContinueWith(_ => UpdateCurrentlyPlaying());
            }
        }
    }
}
