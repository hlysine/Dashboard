using Dashboard.Components;

namespace Dashboard.Views.Components
{
    /// <summary>
    /// Interaction logic for SpotifyView.xaml
    /// </summary>
    public partial class SpotifyView : SpotifyViewBase
    {
        public SpotifyView(SpotifyComponent component = null) : base(component)
        {
            InitializeComponent();
            Load();
        }
    }

    public abstract class SpotifyViewBase : DashboardView<SpotifyComponent>
    {
        protected SpotifyViewBase(SpotifyComponent component) : base(component)
        {

        }

        protected SpotifyViewBase() : this(null)
        {

        }
    }
}
