using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using static Dashboard.Utilities.Native;

namespace Dashboard.Utilities;

public class WindowBlur
{
    public static readonly DependencyProperty IsEnabledProperty = DependencyProperty.RegisterAttached(
        "IsEnabled", typeof(bool), typeof(WindowBlur),
        new PropertyMetadata(false, OnIsEnabledChanged)
    );

    public static void SetIsEnabled(DependencyObject element, bool value)
    {
        element.SetValue(IsEnabledProperty, value);
    }

    public static bool GetIsEnabled(DependencyObject element)
    {
        return (bool)element.GetValue(IsEnabledProperty);
    }

    private static void OnIsEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not Window window)
            return;

        if (true.Equals(e.OldValue))
        {
            window.ClearValue(WindowBlurProperty);
        }

        if (true.Equals(e.NewValue))
        {
            var blur = new WindowBlur(GetBlurType(d));
            window.SetValue(WindowBlurProperty, blur);
        }
    }

    public static readonly DependencyProperty BlurTypeProperty = DependencyProperty.RegisterAttached(
        "BlurType", typeof(BlurType), typeof(WindowBlur),
        new PropertyMetadata(BlurType.Acrylic, OnBlurTypeChanged)
    );

    public static void SetBlurType(DependencyObject element, BlurType value)
    {
        element.SetValue(BlurTypeProperty, value);
    }

    public static BlurType GetBlurType(DependencyObject element)
    {
        return (BlurType)element.GetValue(BlurTypeProperty);
    }

    private static void OnBlurTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not Window window)
            return;

        if (!GetIsEnabled(d))
            return;

        var blur = new WindowBlur((BlurType)e.NewValue);
        window.SetValue(WindowBlurProperty, blur);
    }

    public enum BlurType
    {
        NoBlur,
        Blur,
        Acrylic,
    }

    public static readonly DependencyProperty WindowBlurProperty = DependencyProperty.RegisterAttached(
        "WindowBlur", typeof(WindowBlur), typeof(WindowBlur),
        new PropertyMetadata(null, OnWindowBlurChanged)
    );

    public static void SetWindowBlur(DependencyObject element, WindowBlur value)
    {
        element.SetValue(WindowBlurProperty, value);
    }

    public static WindowBlur GetWindowBlur(DependencyObject element)
    {
        return (WindowBlur)element.GetValue(WindowBlurProperty);
    }

    private static void OnWindowBlurChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not Window window)
            return;

        (e.OldValue as WindowBlur)?.detach();
        (e.NewValue as WindowBlur)?.attach(window);
    }

    private Window window;
    private readonly BlurType type;

    public WindowBlur(BlurType type) => this.type = type;

    private void attach(Window windowToAttach)
    {
        window = windowToAttach;
        var source = (HwndSource)PresentationSource.FromVisual(windowToAttach);
        if (source == null)
        {
            windowToAttach.SourceInitialized += OnSourceInitialized;
        }
        else
        {
            attachCore();
        }
    }

    private void detach()
    {
        try
        {
            detachCore();
        }
        finally
        {
            window = null;
        }
    }

    private void OnSourceInitialized(object sender, EventArgs e)
    {
        ((Window)sender).SourceInitialized -= OnSourceInitialized;
        attachCore();
    }

    private void attachCore()
    {
        enableBlur(window, type);
    }

    private void detachCore()
    {
        window.SourceInitialized -= OnSourceInitialized;
    }

    private static void enableBlur(Window window, BlurType type)
    {
        var windowHelper = new WindowInteropHelper(window);

        var accent = new AccentPolicy
        {
            AccentState = type switch
            {
                //var currentVersion = SystemInfo.Version.Value;
                //if (currentVersion >= VersionInfos.Windows10_1903)
                //{
                //    accent.AccentState = AccentState.ACCENT_ENABLE_BLURBEHIND;
                //}
                //else if (currentVersion >= VersionInfos.Windows10_1809)
                //{
                //    accent.AccentState = AccentState.ACCENT_ENABLE_ACRYLICBLURBEHIND;
                //}
                //else if (currentVersion >= VersionInfos.Windows10)
                //{
                //    accent.AccentState = AccentState.ACCENT_ENABLE_BLURBEHIND;
                //}
                //else
                //{
                //    accent.AccentState = AccentState.ACCENT_ENABLE_TRANSPARENTGRADIENT;
                //}
                BlurType.NoBlur => AccentState.ACCENT_DISABLED,
                BlurType.Blur => AccentState.ACCENT_ENABLE_BLURBEHIND,
                _ => AccentState.ACCENT_ENABLE_ACRYLICBLURBEHIND
            },
            AccentFlags = 2,
            GradientColor = 0x00ffffff,
        };

        int accentStructSize = Marshal.SizeOf(accent);

        IntPtr accentPtr = Marshal.AllocHGlobal(accentStructSize);
        Marshal.StructureToPtr(accent, accentPtr, false);

        var data = new WindowCompositionAttributeData
        {
            Attribute = WindowCompositionAttribute.WCA_ACCENT_POLICY,
            SizeOfData = accentStructSize,
            Data = accentPtr,
        };

        SetWindowCompositionAttribute(windowHelper.Handle, ref data);

        Marshal.FreeHGlobal(accentPtr);
    }
}
