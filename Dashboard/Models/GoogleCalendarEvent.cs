using Dashboard.Utilities;
using Google.Apis.Calendar.v3.Data;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace Dashboard.Models
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
                    if (start.Year == DateTime.Now.Year)
                        ret += start.ToString("MMM dd");
                    else
                        ret += start.ToString("MMM dd, yyyy");
                    if (!@event.EndTimeUnspecified.GetValueOrDefault())
                    {
                        if (end - start > TimeSpan.FromDays(1))
                        {
                            ret += " - ";
                            if (end.Year == DateTime.Now.Year)
                                ret += end.ToString("MMM dd");
                            else
                                ret += end.ToString("MMM dd, yyyy");
                        }
                    }
                }
                else
                {
                    if (start != default)
                    {
                        if (start.Year == DateTime.Now.Year)
                            ret += start.ToString("MMM dd h:mm tt");
                        else
                            ret += start.ToString("MMM dd, yyyy h:mm tt");
                        if (!@event.EndTimeUnspecified.GetValueOrDefault() && end != default)
                        {
                            ret += " - ";
                            if (end.Date == start.Date)
                            {
                                ret += end.ToString("h:mm tt");
                            }
                            else
                            {
                                if (end.Year == DateTime.Now.Year)
                                    ret += end.ToString("MMM dd h:mm tt");
                                else
                                    ret += end.ToString("MMM dd, yyyy h:mm tt");
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

        public GoogleCalendarEvent(CalendarListEntry _calendar, Event _event, Google.Apis.Calendar.v3.Data.Colors _colors)
        {
            @event = _event;
            calendar = _calendar;
            colors = _colors;
        }
    }
}
