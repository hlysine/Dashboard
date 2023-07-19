using Dashboard.Services;
using Dashboard.Utilities;
using Google.Apis.Gmail.v1.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Dashboard.ViewModels;

public partial class GoogleGmailThread : NotifyPropertyChanged
{
    private Thread thread;
    private readonly GoogleGmailService gmail;
    private readonly Profile profile;

    // default is false, set 1 for true.
    private int fetchedThread;

    private bool FetchedThread
    {
        get => System.Threading.Interlocked.CompareExchange(ref fetchedThread, 1, 1) == 1;
        set
        {
            if (value) System.Threading.Interlocked.CompareExchange(ref fetchedThread, 1, 0);
            else System.Threading.Interlocked.CompareExchange(ref fetchedThread, 0, 1);
        }
    }

    public string Snippet => getMessages()?.Last().Snippet ?? thread.Snippet;

    public bool Unread => (getMessages()?.Last().LabelIds.Contains("UNREAD")).GetValueOrDefault();

    public string Subject => getMessages()?.Last().Payload.Headers.FirstOrDefault(x => x.Name == "Subject")?.Value;

    public int MessageCount => (getMessages()?.Count).GetValueOrDefault();

    public bool MultipleMessages => (getMessages()?.Count).GetValueOrDefault() > 1;

    [GeneratedRegex("^\"?(.*?)\"?(?: <[^<>]*>)?$")]
    private static partial Regex EmailFromFieldRegex();

    public string From => EmailFromFieldRegex().Match(getMessages()?.Last().Payload.Headers.FirstOrDefault(x => x.Name == "From")?.Value ?? "").Groups[1].Value;

    public bool Important => (getMessages()?.Last().LabelIds.Contains("IMPORTANT")).GetValueOrDefault();

    public bool Starred => (getMessages()?.Last().LabelIds.Contains("STARRED")).GetValueOrDefault();

    public DateTime Date
    {
        get
        {
            long? dateTimeMs = getMessages()?.Last().InternalDate;

            return dateTimeMs == null ? default : DateTimeOffset.FromUnixTimeMilliseconds(dateTimeMs.GetValueOrDefault()).DateTime.ToLocalTime();
        }
    }

    private List<Message> getMessages()
    {
        if (thread.Messages != null)
            return thread.Messages.ToList();
        if (FetchedThread)
            return null;

        FetchedThread = true;
        Task.Run(
            () =>
            {
                Thread th = gmail.GetThread(thread.Id).Result;
                thread = th;
                NotifyChanged(
                    new[]
                    {
                        nameof(Unread),
                        nameof(Subject),
                        nameof(MessageCount),
                        nameof(MultipleMessages),
                        nameof(From),
                        nameof(Important),
                        nameof(Starred),
                        nameof(Date),
                    }
                );
            }
        );

        return null;
    }

    private RelayCommand openCommand;

    public ICommand OpenCommand => openCommand ??= new RelayCommand(
        // execute
        () =>
        {
            Helper.OpenUri(new Uri($"https://mail.google.com/mail?authuser={profile.EmailAddress}#{((getMessages()?.Last().LabelIds.Contains("INBOX")).GetValueOrDefault() ? "inbox" : "all")}/{thread.Id}"));
        },
        // can execute
        () => true
    );

    public GoogleGmailThread(Thread thread, GoogleGmailService gmail, Profile profile) => (this.thread, this.gmail, this.profile) = (thread, gmail, profile);
}
