using Dashboard.Components;

namespace Dashboard.Views.Components
{
    /// <summary>
    /// Interaction logic for OsuView.xaml
    /// </summary>
    public partial class OsuView : OsuViewBase
    {
        public OsuView(OsuComponent component = null) : base(component)
        {
            InitializeComponent();
            Load();
        }
    }

    public abstract class OsuViewBase : DashboardView<OsuComponent>
    {
        protected OsuViewBase(OsuComponent component) : base(component)
        {

        }

        protected OsuViewBase() : this(null)
        {

        }
    }
}
