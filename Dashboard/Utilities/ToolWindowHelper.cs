using System;
using System.Windows;
using System.Windows.Interop;
using static Dashboard.Utilities.Native;

namespace Dashboard.Utilities
{
    public static class ToolWindowHelper
    {
        public static void SetToolWindow(Window window)
        {
            WindowInteropHelper wndHelper = new(window);

            var exStyle = (int)GetWindowLong(wndHelper.Handle, (int)GetWindowLongFields.GWL_EXSTYLE);

            exStyle |= (int)ExtendedWindowStyles.WS_EX_TOOLWINDOW;
            SetWindowLong(wndHelper.Handle, (int)GetWindowLongFields.GWL_EXSTYLE, (IntPtr)exStyle);
        }
    }
}
