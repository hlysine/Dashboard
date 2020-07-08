using Dashboard.Components;

namespace Dashboard.Views
{
    /// <summary>
    /// Interaction logic for GoogleGmailView.xaml
    /// </summary>
    public partial class GoogleGmailView : GoogleGmailViewBase
    {
        public GoogleGmailView(GoogleGmailComponent component = null) : base(component)
        {
            InitializeComponent();
            Load();
        }
    }

    public abstract class GoogleGmailViewBase : DashboardView<GoogleGmailComponent>
    {
        protected GoogleGmailViewBase(GoogleGmailComponent component) : base(component)
        {

        }

        protected GoogleGmailViewBase() : this(null)
        {

        }
    }
}
