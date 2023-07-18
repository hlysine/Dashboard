using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Dashboard.Utilities
{
    public class Marquee
    {
        public static readonly DependencyProperty IsEnabledProperty =
            DependencyProperty.RegisterAttached(
                "IsEnabled",
                typeof(bool),
                typeof(Marquee),
                new(default(bool), OnIsEnabledPropertyChangedCallback));

        public static void SetIsEnabled(DependencyObject element, bool value)
        {
            element.SetValue(IsEnabledProperty, value);
        }

        public static bool GetIsEnabled(DependencyObject element)
        {
            return (bool)element.GetValue(IsEnabledProperty);
        }

        private static void OnIsEnabledPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var element = d as FrameworkElement;
            UpdateAnimation(element);
            if (GetIsEnabled(element))
                element.SizeChanged += Element_SizeChanged;
        }

        private static void Element_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var element = sender as FrameworkElement;
            UpdateAnimation(element);
        }

        private static void UpdateAnimation(FrameworkElement element)
        {
            var parent = (FrameworkElement)element.Parent;
            if (GetIsEnabled(element) && parent.ActualWidth <= element.ActualWidth)
            {
                var transform = new TranslateTransform(0, 0);
                element.RenderTransform = transform;


                var marquee1 = new DoubleAnimation()
                {
                    From = 0,
                    To = -element.ActualWidth,
                    Duration = TimeSpan.FromSeconds(5),
                    BeginTime = TimeSpan.FromSeconds(3),
                    EasingFunction = new SineEase() { EasingMode = EasingMode.EaseIn }
                };

                Storyboard.SetTarget(marquee1, element);
                Storyboard.SetTargetProperty(marquee1, new("(UIElement.RenderTransform).(TranslateTransform.X)"));


                var marquee2 = new DoubleAnimation()
                {
                    From = parent.ActualWidth,
                    To = 0,
                    Duration = TimeSpan.FromSeconds(3),
                    BeginTime = TimeSpan.FromSeconds(8),
                    EasingFunction = new SineEase() { EasingMode = EasingMode.EaseOut }
                };

                Storyboard.SetTarget(marquee2, element);
                Storyboard.SetTargetProperty(marquee2, new("(UIElement.RenderTransform).(TranslateTransform.X)"));

                var sb = new Storyboard();
                sb.Children.Add(marquee1);
                sb.Children.Add(marquee2);
                sb.RepeatBehavior = RepeatBehavior.Forever;

                sb.Begin(element, true);
            }
            else
            {
                if (element.RenderTransform != null)
                {
                    if (!element.RenderTransform.IsSealed && !element.RenderTransform.IsFrozen)
                        element.RenderTransform.BeginAnimation(TranslateTransform.XProperty, null);
                    if (element.RenderTransform is TranslateTransform)
                        ((TranslateTransform)element.RenderTransform).X = 0;
                }
            }
        }
    }
}
