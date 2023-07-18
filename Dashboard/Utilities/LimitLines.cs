using System;
using System.Windows;
using System.Windows.Controls;

namespace Dashboard.Utilities
{
    public class LimitLines
    {
        public static readonly DependencyProperty MaxLinesProperty =
            DependencyProperty.RegisterAttached(
                "MaxLines",
                typeof(int),
                typeof(LimitLines),
                new PropertyMetadata(default(int), OnMaxLinesPropertyChangedCallback));

        public static void SetMaxLines(DependencyObject element, int value)
        {
            element.SetValue(MaxLinesProperty, value);
        }

        public static int GetMaxLines(DependencyObject element)
        {
            return (int)element.GetValue(MaxLinesProperty);
        }

        private static void OnMaxLinesPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var element = d as TextBlock;
            element.MaxHeight = getLineHeight(element) * GetMaxLines(element) + element.Padding.Top + element.Padding.Bottom;
        }

        public static readonly DependencyProperty MinLinesProperty =
            DependencyProperty.RegisterAttached(
                "MinLines",
                typeof(int),
                typeof(LimitLines),
                new PropertyMetadata(default(int), OnMinLinesPropertyChangedCallback));

        public static void SetMinLines(DependencyObject element, int value)
        {
            element.SetValue(MinLinesProperty, value);
        }

        public static int GetMinLines(DependencyObject element)
        {
            return (int)element.GetValue(MinLinesProperty);
        }

        private static void OnMinLinesPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var element = d as TextBlock;
            element.MinHeight = getLineHeight(element) * GetMinLines(element) + element.Padding.Top + element.Padding.Bottom;
        }

        private static double getLineHeight(TextBlock textBlock)
        {
            var lineHeight = textBlock.LineHeight;
            if (double.IsNaN(lineHeight))
                lineHeight = Math.Ceiling(textBlock.FontSize * textBlock.FontFamily.LineSpacing);
            return lineHeight;
        }
    }
}
