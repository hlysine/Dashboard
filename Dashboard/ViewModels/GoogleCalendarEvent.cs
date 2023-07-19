using Dashboard.Utilities;
using Google.Apis.Calendar.v3.Data;
using System;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media;

namespace Dashboard.ViewModels;

public class GoogleCalendarEvent
{
    private readonly Event @event;
    private readonly CalendarListEntry calendar;
    private readonly Google.Apis.Calendar.v3.Data.Colors colors;

    public string Name => @event.Summary;

    public DateTime Start => @event.Start.GetDateTime();

    public string DateTimeString
    {
        get
        {
            var ret = "";
            DateTime start = @event.Start.GetDateTime();
            DateTime end = @event.End.GetDateTime();
            if (@event.Start.DateTimeRaw == null)
            {
                // All day event
                ret += start.ToReadableDateString();

                if (@event.EndTimeUnspecified.GetValueOrDefault())
                    return ret;

                if (end - start <= TimeSpan.FromDays(1))
                    return ret;

                ret += " - ";
                ret += end.ToReadableDateString();
            }
            else
            {
                if (start != default)
                {
                    ret += start.ToReadableDateString() + " " + start.ToString("h:mm tt");

                    if (@event.EndTimeUnspecified.GetValueOrDefault() || end == default)
                        return ret;

                    ret += " - ";
                    if (end.Date == start.Date)
                    {
                        ret += end.ToString("h:mm tt");
                    }
                    else
                    {
                        ret += end.ToReadableDateString() + " " + end.ToString("h:mm tt");
                    }
                }
                else
                {
                    return "";
                }
            }

            return ret;
        }
    }

    public string CalendarName => calendar.SummaryOverride ?? calendar.Summary;

    public bool PrimaryCalendar => calendar.Primary.GetValueOrDefault();

    public Color EventColor
    {
        get
        {
            if (@event.ColorId.IsNullOrEmpty())
            {
                return (Color)ColorConverter.ConvertFromString(colors.Event__.First().Value.Background);
            }
            else
            {
                return (Color)ColorConverter.ConvertFromString(colors.Event__[@event.ColorId].Background);
            }
        }
    }

    private RelayCommand openCommand;

    public ICommand OpenCommand => openCommand ??= new RelayCommand(
        // execute
        () =>
        {
            Helper.OpenUri(new Uri($"https://calendar.google.com/calendar/r/agenda/{@event.Start.GetDateTime().Year}/{@event.Start.GetDateTime().Month}/{@event.Start.GetDateTime().Day}"));
        },
        // can execute
        () => true
    );

    public GoogleCalendarEvent(CalendarListEntry calendar, Event @event, Google.Apis.Calendar.v3.Data.Colors colors)
    {
        this.@event = @event;
        this.calendar = calendar;
        this.colors = colors;
    }
}
