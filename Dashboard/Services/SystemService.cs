using System;
using System.Diagnostics;

namespace Dashboard.Services;

public class SystemService : Service
{
    public override bool CanAuthorize => true;
    public override bool IsAuthorized => true;

    public string Run(string prompt)
    {
        try
        {
            string uriStr = prompt.Replace("&", "^&");
            Process.Start(new ProcessStartInfo($"cmd", $"/c start {uriStr}") { WindowStyle = ProcessWindowStyle.Hidden, UseShellExecute = true });
            return null;
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }
}
