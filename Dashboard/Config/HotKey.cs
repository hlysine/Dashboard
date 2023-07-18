using System.Windows.Input;

namespace Dashboard.Config;

public class HotKey
{
    public Key Key { get; set; }
    public ModifierKeys ModifierKeys { get; set; }

    public static implicit operator KeyGesture(HotKey hotKey) => new(hotKey.Key, hotKey.ModifierKeys);
}