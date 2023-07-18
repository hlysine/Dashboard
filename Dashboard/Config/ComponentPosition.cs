using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace Dashboard.Config
{
    public class ComponentPosition
    {
        public HorizontalAlignment HorizontalAlignment { get; set; } = HorizontalAlignment.Center;
        public VerticalAlignment VerticalAlignment { get; set; } = VerticalAlignment.Center;
        public Thickness Margin { get; set; } = new(5d);
        public double Width { get; set; } = 500d;
        public double Height { get; set; } = 300d;

        public ComponentPosition() { }

        public ComponentPosition(double width, double height, Thickness margin, HorizontalAlignment alignmentX, VerticalAlignment alignmentY)
            => (Width, Height, Margin, HorizontalAlignment, VerticalAlignment) = (width, height, margin, alignmentX, alignmentY);
    }
}
