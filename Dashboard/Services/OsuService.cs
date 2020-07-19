using Dashboard.Config;
using Dashboard.ViewModels;
using Dashboard.Utilities;
using Dashboard.Utilities.Auth.Models;
using RestSharp;
using RestSharp.Serialization.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RestSharp.Serializers.NewtonsoftJson;

namespace Dashboard.Services
{
    public class OsuService : AuthCodeService
    {
        private IRestClient osu;

        public override bool IsAuthorized => osu != null;

        /// <summary>
        /// Start the authroization code flow or request for an access token if a refresh token is present and the scopes match.
        /// </summary>
        /// <param name="cancel">A <see cref="CancellationToken"/> to cancel the wait for users to authorize on their browsers</param>
        /// <exception cref="OperationCanceledException">Thrown if the wait is canceled</exception>
        public override async Task Authorize(CancellationToken cancel = default)
        {
            OsuTokenResponse tokenResponse;
            if (RefreshToken.IsNullOrEmpty() || !requiredScopes.IsSubsetOf(AuthorizedScopes))
            {
                var taskCompletionSource = new TaskCompletionSource<AuthorizationCodeResponse>();

                EmbedIOAuthServer _server = new EmbedIOAuthServer(new Uri("http://localhost:5001/callback"), 5001);
                await _server.Start();

                _server.AuthorizationCodeReceived += (_, response) =>
                {
                    taskCompletionSource.SetResult(response);
                    return Task.CompletedTask;
                };

                var request = new LoginRequest(new Uri("https://osu.ppy.sh/oauth/authorize"), _server.BaseUri, ClientId, LoginRequest.ResponseType.Code)
                {
                    Scope = requiredScopes
                };
                Helper.OpenUri(request.ToUri());

                while (!taskCompletionSource.Task.IsCompleted)
                {
                    cancel.ThrowIfCancellationRequested();
                    await Task.Delay(500);
                }

                await _server.Stop();

                var response = taskCompletionSource.Task.Result;

                IRestClient osuAuthClient = new RestClient("https://osu.ppy.sh/").UseNewtonsoftJson();

                osuAuthClient.AddHandler("application/json", () => new JsonDeserializer());

                var tokenRequest = new RestRequest("oauth/token/", Method.POST);
                tokenRequest.AddParameter("client_id", ClientId);
                tokenRequest.AddParameter("client_secret", ClientSecret);
                tokenRequest.AddParameter("code", response.Code);
                tokenRequest.AddParameter("grant_type", "authorization_code");
                tokenRequest.AddParameter("redirect_uri", "http://localhost:5001/callback");

                var codeResponse = await osuAuthClient.ExecuteAsync<OsuTokenResponse>(tokenRequest);
                tokenResponse = codeResponse.Data;
            }
            else
            {

                IRestClient osuAuthClient = new RestClient("https://osu.ppy.sh/").UseNewtonsoftJson();

                osuAuthClient.AddHandler("application/json", () => new JsonDeserializer());

                var tokenRequest = new RestRequest("oauth/token/", Method.POST);
                tokenRequest.AddParameter("client_id", ClientId);
                tokenRequest.AddParameter("client_secret", ClientSecret);
                tokenRequest.AddParameter("refresh_token", RefreshToken);
                tokenRequest.AddParameter("grant_type", "refresh_token");

                var codeResponse = await osuAuthClient.ExecuteAsync<OsuTokenResponse>(tokenRequest);
                tokenResponse = codeResponse.Data;
            }

            RefreshToken = tokenResponse.refresh_token;
            AccessToken = tokenResponse.access_token;
            AuthorizedScopes = new List<string>(requiredScopes);

            osu = new RestClient("https://osu.ppy.sh/").UseNewtonsoftJson();
            osu.AddDefaultHeader("Authorization", "Bearer " + tokenResponse.access_token);

            RaiseConfigUpdated(EventArgs.Empty);
        }

        public override async Task Unauthorize(CancellationToken cancel = default)
        {
            RefreshToken = null;
            AccessToken = null;
            AuthorizedScopes.Clear();
            RaiseConfigUpdated(EventArgs.Empty);
        }

        public async Task<List<SimpleUser>> GetFriends()
        {
            var request = new RestRequest("api/v2/friends", Method.GET);

            return (await osu.ExecuteAsync<List<SimpleUser>>(request)).Data;
        }

        public async Task<FullUser> GetUser(string userId)
        {
            var request = new RestRequest("api/v2/users/{id}", Method.GET);
            request.AddUrlSegment("id", userId);

            var response = await osu.ExecuteAsync<FullUser>(request);
            return response.Data;
        }
    }

    public class SimpleUser
    {

        [JsonProperty("avatar_url")]
        public string AvatarUrl { get; set; }

        [JsonProperty("country_code")]
        public string CountryCode { get; set; }

        [JsonProperty("default_group")]
        public string DefaultGroup { get; set; }

        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("is_active")]
        public bool IsActive { get; set; }

        [JsonProperty("is_bot")]
        public bool IsBot { get; set; }

        [JsonProperty("is_online")]
        public bool IsOnline { get; set; }

        [JsonProperty("is_supporter")]
        public bool IsSupporter { get; set; }

        [JsonProperty("last_visit")]
        public DateTime LastVisit { get; set; }

        [JsonProperty("pm_friends_only")]
        public bool PmFriendsOnly { get; set; }

        [JsonProperty("profile_colour")]
        public object ProfileColour { get; set; }

        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("country")]
        public Country Country { get; set; }

        [JsonProperty("cover")]
        public Cover Cover { get; set; }

        [JsonProperty("current_mode_rank")]
        public long CurrentModeRank { get; set; }

        [JsonProperty("groups")]
        public List<object> Groups { get; set; }

        [JsonProperty("support_level")]
        public int SupportLevel { get; set; }

    }

    public class Kudosu
    {

        [JsonProperty("total")]
        public long Total { get; set; }

        [JsonProperty("available")]
        public long Available { get; set; }

    }

    public class Country
    {

        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

    }

    public class Cover
    {

        [JsonProperty("custom_url")]
        public object CustomUrl { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

    }

    public class MonthlyPlaycount
    {

        [JsonProperty("start_date")]
        public string StartDate { get; set; }

        [JsonProperty("count")]
        public long Count { get; set; }

    }

    public class Page
    {

        [JsonProperty("html")]
        public string Html { get; set; }

        [JsonProperty("raw")]
        public string Raw { get; set; }

    }

    public class Level
    {

        [JsonProperty("current")]
        public long Current { get; set; }

        [JsonProperty("progress")]
        public long Progress { get; set; }

    }

    public class GradeCounts
    {

        [JsonProperty("ss")]
        public long Ss { get; set; }

        [JsonProperty("ssh")]
        public long Ssh { get; set; }

        [JsonProperty("s")]
        public long S { get; set; }

        [JsonProperty("sh")]
        public long Sh { get; set; }

        [JsonProperty("a")]
        public long A { get; set; }

    }

    public class Rank
    {

        [JsonProperty("global")]
        public long Global { get; set; }

        [JsonProperty("country")]
        public long Country { get; set; }

    }

    public class Statistics
    {

        [JsonProperty("level")]
        public Level Level { get; set; }

        [JsonProperty("pp")]
        public double PP { get; set; }

        [JsonProperty("pp_rank")]
        public long PpRank { get; set; }

        [JsonProperty("ranked_score")]
        public long RankedScore { get; set; }

        [JsonProperty("hit_accuracy")]
        public double HitAccuracy { get; set; }

        [JsonProperty("play_count")]
        public long PlayCount { get; set; }

        [JsonProperty("play_time")]
        public long PlayTime { get; set; }

        [JsonProperty("total_score")]
        public long TotalScore { get; set; }

        [JsonProperty("total_hits")]
        public long TotalHits { get; set; }

        [JsonProperty("maximum_combo")]
        public long MaximumCombo { get; set; }

        [JsonProperty("replays_watched_by_others")]
        public long ReplaysWatchedByOthers { get; set; }

        [JsonProperty("is_ranked")]
        public bool IsRanked { get; set; }

        [JsonProperty("grade_counts")]
        public GradeCounts GradeCounts { get; set; }

        [JsonProperty("rank")]
        public Rank Rank { get; set; }

    }

    public class UserAchievement
    {

        [JsonProperty("achieved_at")]
        public DateTime AchievedAt { get; set; }

        [JsonProperty("achievement_id")]
        public long AchievementId { get; set; }

    }

    public class RankHistory
    {

        [JsonProperty("mode")]
        public string Mode { get; set; }

        [JsonProperty("data")]
        public List<int> Data { get; set; }

    }

    public class FullUser
    {

        [JsonProperty("avatar_url")]
        public string AvatarUrl { get; set; }

        [JsonProperty("country_code")]
        public string CountryCode { get; set; }

        [JsonProperty("default_group")]
        public string DefaultGroup { get; set; }

        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("is_active")]
        public bool IsActive { get; set; }

        [JsonProperty("is_bot")]
        public bool IsBot { get; set; }

        [JsonProperty("is_online")]
        public bool IsOnline { get; set; }

        [JsonProperty("is_supporter")]
        public bool IsSupporter { get; set; }

        [JsonProperty("last_visit")]
        public DateTime LastVisit { get; set; }

        [JsonProperty("pm_friends_only")]
        public bool PmFriendsOnly { get; set; }

        [JsonProperty("profile_colour")]
        public object ProfileColour { get; set; }

        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("cover_url")]
        public string CoverUrl { get; set; }

        [JsonProperty("discord")]
        public object Discord { get; set; }

        [JsonProperty("has_supported")]
        public bool HasSupported { get; set; }

        [JsonProperty("interests")]
        public object Interests { get; set; }

        [JsonProperty("join_date")]
        public DateTime JoinDate { get; set; }

        [JsonProperty("kudosu")]
        public Kudosu Kudosu { get; set; }

        [JsonProperty("lastfm")]
        public object Lastfm { get; set; }

        [JsonProperty("location")]
        public object Location { get; set; }

        [JsonProperty("max_blocks")]
        public long MaxBlocks { get; set; }

        [JsonProperty("max_friends")]
        public long MaxFriends { get; set; }

        [JsonProperty("occupation")]
        public object Occupation { get; set; }

        [JsonProperty("playmode")]
        public string Playmode { get; set; }

        [JsonProperty("playstyle")]
        public object Playstyle { get; set; }

        [JsonProperty("post_count")]
        public long PostCount { get; set; }

        [JsonProperty("profile_order")]
        public List<string> ProfileOrder { get; set; }

        [JsonProperty("skype")]
        public object Skype { get; set; }

        [JsonProperty("title")]
        public object Title { get; set; }

        [JsonProperty("twitter")]
        public object Twitter { get; set; }

        [JsonProperty("website")]
        public object Website { get; set; }

        [JsonProperty("country")]
        public Country Country { get; set; }

        [JsonProperty("cover")]
        public Cover Cover { get; set; }

        [JsonProperty("account_history")]
        public List<object> AccountHistory { get; set; }

        [JsonProperty("active_tournament_banner")]
        public List<object> ActiveTournamentBanner { get; set; }

        [JsonProperty("badges")]
        public List<object> Badges { get; set; }

        [JsonProperty("favourite_beatmapset_count")]
        public long FavouriteBeatmapsetCount { get; set; }

        [JsonProperty("follower_count")]
        public long FollowerCount { get; set; }

        [JsonProperty("graveyard_beatmapset_count")]
        public long GraveyardBeatmapsetCount { get; set; }

        [JsonProperty("groups")]
        public List<object> Groups { get; set; }

        [JsonProperty("loved_beatmapset_count")]
        public long LovedBeatmapsetCount { get; set; }

        [JsonProperty("monthly_playcounts")]
        public List<MonthlyPlaycount> MonthlyPlaycounts { get; set; }

        [JsonProperty("page")]
        public Page Page { get; set; }

        [JsonProperty("previous_usernames")]
        public List<object> PreviousUsernames { get; set; }

        [JsonProperty("ranked_and_approved_beatmapset_count")]
        public long RankedAndApprovedBeatmapsetCount { get; set; }

        [JsonProperty("replays_watched_counts")]
        public List<object> ReplaysWatchedCounts { get; set; }

        [JsonProperty("scores_first_count")]
        public long ScoresFirstCount { get; set; }

        [JsonProperty("statistics")]
        public Statistics Statistics { get; set; }

        [JsonProperty("support_level")]
        public long SupportLevel { get; set; }

        [JsonProperty("unranked_beatmapset_count")]
        public long UnrankedBeatmapsetCount { get; set; }

        [JsonProperty("user_achievements")]
        public List<UserAchievement> UserAchievements { get; set; }

        [JsonProperty("rankHistory")]
        public RankHistory RankHistory { get; set; }

    }
}
