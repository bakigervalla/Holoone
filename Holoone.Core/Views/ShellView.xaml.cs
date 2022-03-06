using Caliburn.Micro;
using HolooneNavis.Models;
using System.Threading;
using System.Threading.Tasks;
using WPFSpark;

namespace HolooneNavis.Views
{
    /// <summary>
    /// Interaction logic for ShellView.xaml
    /// </summary>
    public partial class ShellView : SparkWindow, IHandle<ViewState>
    {
 
        public ShellView()
        {
            InitializeComponent();
            this.Loaded += ShellView_Loaded;

            IEventAggregator eventAggregator = IoC.Get<IEventAggregator>();
            eventAggregator.SubscribeOnUIThread(this);
        }

        public Task HandleAsync(ViewState viewState, CancellationToken cancellationToken)
        {
            switch (viewState)
            {
                case ViewState.Minimize:
                    this.WindowState = System.Windows.WindowState.Minimized;
                    return null;
                case ViewState.Normal:
                    this.WindowState = System.Windows.WindowState.Normal;
                    return null;
                case ViewState.Close:
                    this.Close();
                    return null;
                default:
                    this.WindowState = System.Windows.WindowState.Normal;
                    return null;
            }

        }

        private void ShellView_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            System.Windows.Forms.Integration.ElementHost.EnableModelessKeyboardInterop(this);
        }
    }
}
