using System;
using Windows.System;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace YTechie
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            Loaded += OnLoaded;
            InitializeComponent();
        }

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            WebViewControl.NavigationStarting += WebViewControlOnNavigationStarting;
            WebViewControl.NavigationCompleted += WebViewControlOnNavigationCompleted;
        }

        private void WebViewControlOnNavigationCompleted(WebView sender, WebViewNavigationCompletedEventArgs args)
        {
            BusyIndicator.Visibility = Visibility.Collapsed;
            BusyIndicator.IsActive = false;
        }

        private async void WebViewControlOnNavigationStarting(WebView sender, WebViewNavigationStartingEventArgs args)
        {
            if (args.Uri.Host.EndsWith("ytechie.com", StringComparison.CurrentCultureIgnoreCase))
            {
                BusyIndicator.Visibility = Visibility.Visible;
                BusyIndicator.IsActive = true;
                return;
            }

            args.Cancel = true;
            await Launcher.LaunchUriAsync(args.Uri, new LauncherOptions {DesiredRemainingView = ViewSizePreference.UseHalf});
        }

        private void RefreshButton_OnClick(object sender, RoutedEventArgs e)
        {
            WebViewControl.Refresh();
        }

        private void HomeButton_OnClick(object sender, RoutedEventArgs e)
        {
            WebViewControl.Navigate(new Uri("http://www.ytechie.com"));
        }
    }
}
