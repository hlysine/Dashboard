using Dashboard.Utilities;
using Google.Apis.Calendar.v3.Data;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Windows.Media;

namespace Dashboard.ViewModels
{
    public class GoogleCalendarEvent
    {
        private Event @event;
        private CalendarListEntry calendar;
        private Google.Apis.Calendar.v3.Data.Colors colors;

        public string Name { get => @event.Summary; }

        public DateTime Start { get => @event.Start.GetDateTime(); }

        public string DateTimeString
        {
            get
            {
                string ret = "";
                DateTime start = @event.Start.GetDateTime();
                DateTime end = @event.End.GetDateTime();
                if (@event.Start.DateTime == null)
                { // All day event
                    ret += start.ToReadableDateString();
                    if (!@event.EndTimeUnspecified.GetValueOrDefault())
                    {
                        if (end - start > TimeSpan.FromDays(1))
                        {
                            ret += " - ";
                            ret += end.ToReadableDateString();
                        }
                    }
                }
                else
                {
                    if (start != default)
                    {
                        ret += start.ToReadableDateString() + " " + start.ToString("h:mm tt");
                        if (!@event.EndTimeUnspecified.GetValueOrDefault() && end != default)
                        {
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
                    }
                    else
                    {
                        return "";
                    }
                }
                return ret;
            }
        }

        public string CalendarName { get => calendar.SummaryOverride ?? calendar.Summary; }

        public bool PrimaryCalendar { get => calendar.Primary.GetValueOrDefault(); }

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

        public ICommand OpenCommand
        {
            get
            {
                return openCommand ?? (openCommand = new RelayCommand(
                    // execute
                    () =>
                    {
                        Helper.OpenUri(new Uri($"https://calendar.google.com/calendar/r/agenda/{@event.Start.GetDateTime().Year}/{@event.Start.GetDateTime().Month}/{@event.Start.GetDateTime().Day}"));
                    },
                    // can execute
                    () =>
                    {
                        return true;
                    }
                ));
            }
        }

        public GoogleCalendarEvent(CalendarListEntry _calendar, Event _event, Google.Apis.Calendar.v3.Data.Colors _colors)
        {
            @event = _event;
            calendar = _calendar;
            colors = _colors;
        }
    }
}
