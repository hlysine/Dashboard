using Dashboard.Components;

namespace Dashboard.Views
{
    /// <summary>
    /// Interaction logic for GoogleCalendarView.xaml
    /// </summary>
    public partial class GoogleCalendarView : GoogleCalendarViewBase
    {
        public GoogleCalendarView(GoogleCalendarComponent component = null) : base(component)
        {
            InitializeComponent();
            Load();
        }
    }

    public abstract class GoogleCalendarViewBase : DashboardView<GoogleCalendarComponent>
    {
        protected GoogleCalendarViewBase(GoogleCalendarComponent component) : base(component)
        {

        }

        protected GoogleCalendarViewBase() : this(null)
        {

        }
    }
}
