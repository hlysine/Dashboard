namespace Dashboard.Config
{
    public class ColorScheme
    {
        public string PrimaryHue { get; set; } = "blue";
        public string AccentHue { get; set; } = "lightblue";
        public Theme Theme { get; set; } = Theme.Light;
    }

    public enum Theme
    {
        Dark,
        Light
    }
}
