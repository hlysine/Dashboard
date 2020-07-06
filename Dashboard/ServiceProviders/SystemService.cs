using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Dashboard.ServiceProviders
{
    public class SystemService : ServiceProvider
    {
        public override bool CanAuthorize => true;
        public override bool IsAuthorized => true;

        public string Run(string prompt)
        {
            try
            {
                var uriStr = prompt.ToString().Replace("&", "^&");
                Process.Start(new ProcessStartInfo($"cmd", $"/c start {uriStr}") { WindowStyle = ProcessWindowStyle.Hidden, UseShellExecute = true });
                return null;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}
