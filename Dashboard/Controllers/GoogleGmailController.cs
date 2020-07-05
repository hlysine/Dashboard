using Dashboard.Config;
using Dashboard.Models;
using Dashboard.ServiceProviders;
using Dashboard.Tools;
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

        private ObservableCollection<GoogleGmailThread> threads = new ObservableCollection<GoogleGmailThread>();
        public ObservableCollection<GoogleGmailThread> Threads
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

        public override async void OnInitializationComplete()
        {
            if (Gmail.CanAuthorize)
            {
                if (!Gmail.IsAuthorized)
                    await Gmail.Authorize();
                Authorized = true;
            }
            var th = await Gmail.GetThreads();
            th.Threads.ForEach(x => Threads.Add(new GoogleGmailThread(x, Gmail)));
        }

        public override void OnInitialize()
        {
            Gmail.RequireScopes(new[] {
                GmailService.Scope.GmailReadonly
            });
        }
    }
}
