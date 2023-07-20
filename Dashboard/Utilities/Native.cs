using System;
using System.Runtime.InteropServices;

namespace Dashboard.Utilities;

internal static partial class Native
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
        ACCENT_INVALID_STATE = 5,
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

    [LibraryImport("user32.dll")]
    internal static partial int SetWindowCompositionAttribute(IntPtr hwnd, ref WindowCompositionAttributeData data);

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
        GWL_EXSTYLE = -20,
        // ...
    }

    [LibraryImport("user32.dll", EntryPoint = "GetWindowLongW")]
    internal static partial IntPtr GetWindowLong32(IntPtr hWnd, int nIndex);

    [LibraryImport("user32.dll", EntryPoint = "GetWindowLongPtrW")]
    internal static partial IntPtr GetWindowLong64(IntPtr hWnd, int nIndex);

    internal static IntPtr GetWindowLong(IntPtr hWnd, int nIndex)
    {
        var error = 0;
        IntPtr result;
        // Win32 SetWindowLong doesn't clear error on success
        SetLastError(0);

        if (IntPtr.Size == 4)
        {
            // use GetWindowLong
            IntPtr tempResult = GetWindowLong32(hWnd, nIndex);
            error = Marshal.GetLastWin32Error();
            result = new IntPtr(tempResult);
        }
        else
        {
            // use GetWindowLongPtr
            result = GetWindowLong64(hWnd, nIndex);
            error = Marshal.GetLastWin32Error();
        }

        if (result == IntPtr.Zero && error != 0)
        {
            throw new System.ComponentModel.Win32Exception(error);
        }

        return result;
    }


    [LibraryImport("user32.dll", EntryPoint = "SetWindowLongW", SetLastError = true)]
    internal static partial IntPtr SetWindowLong32(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

    [LibraryImport("user32.dll", EntryPoint = "SetWindowLongPtrW", SetLastError = true)]
    internal static partial IntPtr SetWindowLong64(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

    internal static IntPtr SetWindowLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong)
    {
        var error = 0;
        IntPtr result;
        // Win32 SetWindowLong doesn't clear error on success
        SetLastError(0);

        if (IntPtr.Size == 4)
        {
            // use SetWindowLong
            IntPtr tempResult = SetWindowLong32(hWnd, nIndex, IntPtrToInt32(dwNewLong));
            error = Marshal.GetLastWin32Error();
            result = new IntPtr(tempResult);
        }
        else
        {
            // use SetWindowLongPtr
            result = SetWindowLong64(hWnd, nIndex, dwNewLong);
            error = Marshal.GetLastWin32Error();
        }

        if (result == IntPtr.Zero && error != 0)
        {
            throw new System.ComponentModel.Win32Exception(error);
        }

        return result;
    }

    internal static int IntPtrToInt32(IntPtr intPtr)
    {
        return unchecked((int)intPtr.ToInt64());
    }

    [LibraryImport("kernel32.dll", EntryPoint = "SetLastError")]
    internal static partial void SetLastError(int dwErrorCode);
}
