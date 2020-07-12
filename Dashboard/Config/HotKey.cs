using Dashboard.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Dashboard.Config
{
    public class HotKey
    {
        public ModifierKeys ModifierKeys { get; set; } = ModifierKeys.Alt;
        public Keys Key { get; set; } = Keys.D;
    }
}
