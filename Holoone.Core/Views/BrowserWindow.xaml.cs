using System;
using System.Windows;

namespace HolooneNavis.Views
{
    /// <summary>
    /// Interaction logic for BrowserWindow.xaml
    /// </summary>
    public partial class BrowserWindow : Window
    {
        //const int POLL_DELAY = 250;
        //WebBrowser _webBrowser;

        //string LCP_URL;
        //public OAuthState State { get; }

        public BrowserWindow(string regionUrl)
        {
            InitializeComponent();
            //return;
            //LCP_URL = regionUrl;

            //State = new OAuthState();
            //DataContext = this;

            InitializeAsync(regionUrl);

            //var thread = new Thread(() => NavigateWebView(regionUrl));
            //thread.Start();
        }

        private async void NavigateWebView(string Url)
        {
            //CoreWebView2EnvironmentOptions opt = new CoreWebView2EnvironmentOptions();

            //opt.AdditionalBrowserArguments = "--enable-logging --log-file=C:\\temp\\tests\\WebView1.log";

            //CoreWebView2Environment env = await CoreWebView2Environment.CreateAsync(null, "WebViewFolder1", opt);

            //await Wb.EnsureCoreWebView2Async(env);

            //CoreWebView2EnvironmentOptions opt2 = new CoreWebView2EnvironmentOptions();

            //opt2.AdditionalBrowserArguments = "--enable-logging --log-file=C:\\temp\\tests\\WebView2.log";

            //CoreWebView2Environment env2 = await CoreWebView2Environment.CreateAsync(null, "WebViewFolder2", opt2);

            //await Wb.EnsureCoreWebView2Async(env2);

            //await Dispatcher.BeginInvoke(new Action(() =>
            //{
            //    Wb.Source = new Uri(Url);
            //}));
        }

        private bool _firstLoad = true;

        private async void InitializeAsync(string url)
        {
            //var env = await CoreWebView2Environment.CreateAsync(
            //    userDataFolder: Path.Combine(Path.GetTempPath(), "HolooneNavis"),
            //    options: new CoreWebView2EnvironmentOptions(allowSingleSignOnUsingOSPrimaryAccount: true));
            //await Wb.EnsureCoreWebView2Async(env);

            //Wb.NavigationCompleted += WebView_NavigationCompleted;

            //Wb.Source = new Uri(url);
        }

        //private void WebView_NavigationCompleted(object sender, CoreWebView2NavigationCompletedEventArgs e)
        //{
        //    if (!_firstLoad)
        //        return;

        //    _firstLoad = false;
        //    Wb.Visibility = Visibility.Visible;
        //}

        //private void YourPage_OnUnloaded(object sender, RoutedEventArgs e)
        //{
        //    _firstLoad = true;
        //    Wb.Dispose();
        //}

        //private async void HandleRedirect()
        //{
        //    State.Token = null;

        //    var request = OAuthRequest.BuildLoopbackRequest();
        //    var listener = new HttpListener();
        //    listener.Prefixes.Add(request.RedirectUri);
        //    listener.Start();

        //    // note: add a reference to System.Windows.Presentation and a 'using System.Windows.Threading' for this to compile
        //    await Dispatcher.BeginInvoke(new Action(() =>
        //    {
        //        // Wb.Navigate(LCP_URL); // request.AuthorizationRequestUri);
        //        Wb.Source = new Uri(LCP_URL);
        //    }));

        //    // here, we'll wait for redirection from our hosted webbrowser
        //    var context = await listener.GetContextAsync();

        //    // browser has navigated to our small http servern answer anything here
        //    string html = string.Format("<html><body></body></html>");
        //    var buffer = Encoding.UTF8.GetBytes(html);
        //    context.Response.ContentLength64 = buffer.Length;
        //    var stream = context.Response.OutputStream;
        //    var responseTask = stream.WriteAsync(buffer, 0, buffer.Length).ContinueWith((task) =>
        //    {
        //        stream.Close();
        //        listener.Stop();
        //    });

        //    string error = context.Request.QueryString["error"];
        //    if (error != null)
        //        return;

        //    string state = context.Request.QueryString["state"];
        //    if (state != request.State)
        //        return;

        //    string code = context.Request.QueryString["code"];
        //    State.Token = request.ExchangeCodeForAccessToken(code);
        //}
    }
}
