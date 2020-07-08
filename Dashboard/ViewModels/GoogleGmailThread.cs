using Dashboard.Services;
using Dashboard.Utilities;
using Google.Apis.Gmail.v1.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;

namespace Dashboard.ViewModels
{
    public class GoogleGmailThread : NotifyPropertyChanged
    {
        private Thread thread;
        private GoogleGmailService gmail;
        private Profile profile;

        // default is false, set 1 for true.
        private int fetchedThread = 0;

        private bool FetchedThread
        {
            get { return System.Threading.Interlocked.CompareExchange(ref fetchedThread, 1, 1) == 1; }
            set
            {
                if (value) System.Threading.Interlocked.CompareExchange(ref fetchedThread, 1, 0);
                else System.Threading.Interlocked.CompareExchange(ref fetchedThread, 0, 1);
            }
        }

        public string Snippet { get => getMessages()?.Last().Snippet ?? thread.Snippet; }

        public bool Unread { get => (getMessages()?.Last().LabelIds.Contains("UNREAD")).GetValueOrDefault(); }

        public string Subject { get => getMessages()?.Last().Payload.Headers.FirstOrDefault(x => x.Name == "Subject")?.Value; }

        public int MessageCount { get => (getMessages()?.Count).GetValueOrDefault(); }

        public bool MultipleMessages { get => (getMessages()?.Count).GetValueOrDefault() > 1; }

        public string From
        {
            get => Regex.Match(getMessages()?.Last().Payload.Headers.FirstOrDefault(x => x.Name == "From")?.Value ?? "", @"^""?(.*?)""?(?: <[^<>]*>)?$").Groups[1].Value;
        }

        public bool Important { get => (getMessages()?.Last().LabelIds.Contains("IMPORTANT")).GetValueOrDefault(); }

        public bool Starred { get => (getMessages()?.Last().LabelIds.Contains("STARRED")).GetValueOrDefault(); }

        public DateTime Date
        {
            get
            {
                var dateTimeMs = getMessages()?.Last().InternalDate;
                return dateTimeMs == null ? default : DateTimeOffset.FromUnixTimeMilliseconds(dateTimeMs.GetValueOrDefault()).DateTime.ToLocalTime();
            }
        }

        private List<Message> getMessages()
        {
            if (thread.Messages == null)
            {
                if (!FetchedThread)
                {
                    FetchedThread = true;
                    Task.Run(() =>
                    {
                        var th = gmail.GetThread(thread.Id).Result;
                        thread = th;
                        NotifyChanged(new[] {
                            nameof(Unread),
                            nameof(Subject),
                            nameof(MessageCount),
                            nameof(MultipleMessages),
                            nameof(From),
                            nameof(Important),
                            nameof(Starred),
                            nameof(Date),
                        });
                    });
                }
                return null;
            }
            else
            {
                return thread.Messages.ToList();
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
                        Helper.OpenUri(new Uri($"https://mail.google.com/mail?authuser={profile.EmailAddress}#{((getMessages()?.Last().LabelIds.Contains("INBOX")).GetValueOrDefault() ? "inbox" : "all")}/{thread.Id}"));
                    },
                    // can execute
                    () =>
                    {
                        return true;
                    }
                ));
            }
        }

        public GoogleGmailThread(Thread _thread, GoogleGmailService _gmail, Profile _profile) => (thread, gmail, profile) = (_thread, _gmail, _profile);
    }
}
