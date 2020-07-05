using Dashboard.Config;
using Dashboard.Models;
using Dashboard.ServiceProviders;
using Dashboard.Utilities;
using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Dashboard.Controllers
{
    public class GoogleGmailController : DashboardController
    {
        [RequireService]
        public GoogleGmailService Gmail { get; set; }

        private List<GoogleGmailThread> threads = new List<GoogleGmailThread>();
        public List<GoogleGmailThread> Threads
        {
            get => threads;
            set => SetAndNotify(ref threads, value);
        }

        private bool authorized = false;

        public bool Authorized
        {
            get => authorized;
            set => SetAndNotify(ref authorized, value);
        }

        public GoogleGmailController()
        {
        }

        private async Task LoadGmail()
        {
            var th = await Gmail.GetThreads();
            Threads.Clear();
            threads.AddRange(th.Threads.Select(x => new GoogleGmailThread(x, Gmail)));
            NotifyChanged(nameof(Threads));
        }

        public override async void OnInitializationComplete()
        {
            if (Gmail.CanAuthorize)
            {
                if (!Gmail.IsAuthorized)
                    await Gmail.Authorize();
                Authorized = true;
            }
            await LoadGmail();
            Loaded = true;
        }

        public override void OnInitialize()
        {
            Gmail.RequireScopes(new[] {
                GmailService.Scope.GmailReadonly
            });
        }

        public override async void OnRefresh()
        {
            await LoadGmail();
        }
    }
}
