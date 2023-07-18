using System;
using System.Runtime.InteropServices;

namespace Dashboard.Utilities;

public static class Native
{
    internal enum AccentFlagsType
    {
        Window = 0,
        Popup,
    }

    internal enum AccentState
    {
        ACCENT_DISABLED = 0,
        ACCENT_ENABLE_GRADIENT = 1,
        ACCENT_ENABLE_TRANSPARENTGRADIENT = 2,
        ACCENT_ENABLE_BLURBEHIND = 3,
        ACCENT_ENABLE_ACRYLICBLURBEHIND = 4,
        ACCENT_INVALID_STATE = 5
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct AccentPolicy
    {
        public AccentState AccentState;
        public int AccentFlags;
        public uint GradientColor;
        public int AnimationId;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct WindowCompositionAttributeData
    {
        public WindowCompositionAttribute Attribute;
        public IntPtr Data;
        public int SizeOfData;
    }

    internal enum WindowCompositionAttribute
    {
        // ...
        WCA_ACCENT_POLICY = 19,
        // ...
    }

    [DllImport("user32.dll")]
    internal static extern int SetWindowCompositionAttribute(IntPtr hwnd, ref WindowCompositionAttributeData data);

    // Registers a hot key with Windows.
    [DllImport("user32.dll")]
    internal static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);
    // Unregisters the hot key with Windows.
    [DllImport("user32.dll")]
    internal static extern bool UnregisterHotKey(IntPtr hWnd, int id);

    [Flags]
    internal enum ExtendedWindowStyles
    {
        // ...
        WS_EX_TOOLWINDOW = 0x00000080,
        // ...
    }

    internal enum GetWindowLongFields
    {
        // ...
        GWL_EXSTYLE = (-20),
        // ...
    }

    [DllImport("user32.dll")]
    internal static extern IntPtr GetWindowLong(IntPtr hWnd, int nIndex);

    internal static IntPtr SetWindowLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong)
    {
        var error = 0;
        var result = IntPtr.Zero;
        // Win32 SetWindowLong doesn't clear error on success
        SetLastError(0);

        if (IntPtr.Size == 4)
        {
            // use SetWindowLong
            var tempResult = IntSetWindowLong(hWnd, nIndex, IntPtrToInt32(dwNewLong));
            error = Marshal.GetLastWin32Error();
            result = new IntPtr(tempResult);
        }
        else
        {
            // use SetWindowLongPtr
            result = IntSetWindowLongPtr(hWnd, nIndex, dwNewLong);
            error = Marshal.GetLastWin32Error();
        }

        if ((result == IntPtr.Zero) && (error != 0))
        {
            throw new System.ComponentModel.Win32Exception(error);
        }

        return result;
    }

    [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr", SetLastError = true)]
    internal static extern IntPtr IntSetWindowLongPtr(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

    [DllImport("user32.dll", EntryPoint = "SetWindowLong", SetLastError = true)]
    internal static extern Int32 IntSetWindowLong(IntPtr hWnd, int nIndex, Int32 dwNewLong);

    internal static int IntPtrToInt32(IntPtr intPtr)
    {
        return unchecked((int)intPtr.ToInt64());
    }

    [DllImport("kernel32.dll", EntryPoint = "SetLastError")]
    internal static extern void SetLastError(int dwErrorCode);
}