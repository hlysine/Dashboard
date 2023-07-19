using Dashboard.Config;
using Dashboard.Utilities;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Dashboard.Services;

public class GoogleCalendarService : AuthCodeService
{
    [RequireService(nameof(Id))]
    private GoogleService Google { get; set; }

    // Comes from GoogleService

    public override string ClientId => Google?.ClientId;

    public override string ClientSecret => Google?.ClientSecret;

    public override List<string> AuthorizedScopes => Google?.AuthorizedScopes ?? new List<string>() /* Prevents null ref when deserializing */;

    /// <summary>
    /// Set the list of scopes required. To be called before <see cref="Authorize(CancellationToken)"/>.
    /// </summary>
    /// <param name="scopes">The list of scopes</param>
    public override void RequireScopes(string[] scopes)
    {
        Google?.RequireScopes(scopes);
    }

    // Properties that are not used since the Google client libraries handle the credentials themselves

    [Obsolete("This property is not used", true)]
    public new string AccessToken { get; set; }

    [Obsolete("This property is not used", true)]
    public new string RefreshToken { get; set; }


    public override bool IsAuthorized => calendar != null;

    [Obsolete("RequiredScopes is saved in GoogleService", true)]
    protected new List<string> RequiredScopes;

    private CalendarService calendar;

    /// <summary>
    /// Start the authorization code flow or request for an access token if a refresh token is present and the scopes match.
    /// </summary>
    /// <param name="cancel">A <see cref="CancellationToken"/> to cancel the wait for users to authorize on their browsers</param>
    /// <exception cref="OperationCanceledException">Thrown if the wait is canceled</exception>
    public override async Task Authorize(CancellationToken cancel = default)
    {
        if (!Google.IsAuthorized)
            await Google.Authorize(cancel);
        calendar = new CalendarService(new BaseClientService.Initializer
        {
            HttpClientInitializer = Google.GetCredential(),
            ApplicationName = Helper.GetProductName(),
        });
    }

    /// <summary>
    /// Note: unauthorizing one Google service causes all services to be unauthorized and the user has to authorize other services again.
    /// </summary>
    /// <param name="cancel">A <see cref="CancellationToken"/> to cancel the operation (may not be cancelable)</param>
    public override async Task Unauthorize(CancellationToken cancel = default)
    {
        await Google.Unauthorize(cancel);
    }

    public async Task<CalendarList> GetAllCalendars()
    {
        CalendarListResource.ListRequest request = calendar.CalendarList.List();

        return await request.ExecuteAsync();
    }

    /// <summary>
    /// Get all events in a calendar.
    /// </summary>
    /// <param name="calendarId">ID of the calendar. Use <code>primary</code> for the primary calendar.</param>
    /// <param name="timeMin">The time of the earliest event to be returned. Defaults to <see cref="DateTime.Now"/>.</param>
    /// <param name="maxResults">Number of events to return.</param>
    /// <returns></returns>
    public async Task<Events> GetEvents(string calendarId, DateTime timeMin = default, int maxResults = 100)
    {
        // Define parameters of request.
        EventsResource.ListRequest request = calendar.Events.List(calendarId);
        request.TimeMin = timeMin == default ? DateTime.Now : timeMin;
        request.ShowDeleted = false;
        request.SingleEvents = true;
        request.MaxResults = maxResults;
        request.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;

        // List events.
        return await request.ExecuteAsync();
    }

    /// <summary>
    /// Get all events in all calendars.
    /// </summary>
    /// <param name="timeMin">The time of the earliest event to be returned. Defaults to <see cref="DateTime.Now"/>.</param>
    /// <param name="maxResults">Number of events to return for each calendar.</param>
    public async Task<Dictionary<CalendarListEntry, Events>> GetAllEvents(DateTime timeMin = default, int maxResults = 100)
    {
        CalendarList calendars = await GetAllCalendars();

        var ret = new Dictionary<CalendarListEntry, Events>();
        foreach (CalendarListEntry downloadedCalendar in calendars.Items)
        {
            Events events = await GetEvents(downloadedCalendar.Id, timeMin, maxResults);
            ret.Add(downloadedCalendar, events);
        }

        return ret;
    }

    /// <summary>
    /// Get colors used in calendars and events
    /// </summary>
    /// <returns></returns>
    public async Task<Colors> GetColors()
    {
        ColorsResource.GetRequest request = calendar.Colors.Get();

        return await request.ExecuteAsync();
    }
}
