using System;
using System.Collections.Generic;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.Web.Syndication;

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

        private void RecentPosts_Loading(object sender, object e)
        {
            var menuFlyout = RecentPosts.Flyout as MenuFlyout;

            if (menuFlyout == null)
                return;

            var menuFlyoutItems = menuFlyout.Items;

            if (menuFlyoutItems == null)
                return;

            //Check if we already loaded this list
            if (menuFlyoutItems.Count > 0)
                return;

            var menuItems = new MenuFlyoutItem[10];
            for (var i = 0; i < 10; i++)
            {
                var item = new MenuFlyoutItem { Text = "Loading recent posts..." };

                if (i > 0)
                    item.Visibility = Visibility.Collapsed;

                menuItems[i] = item;
                menuFlyoutItems.Add(item);
            }

            menuItems[0].Text = "Loading Latest Posts...";

            ReadLatestPosts(menuItems);
        }

        private async void ReadLatestPosts(MenuFlyoutItem[] menuItems)
        {
            var feedClient = new SyndicationClient();

            try
            {
                var feedData = await feedClient.RetrieveFeedAsync(new Uri("http://ytechie.com/rss"));

                var itemNumber = 0;
                foreach (var feedItem in feedData.Items)
                {
                    if (itemNumber < 10)
                    {
                        menuItems[itemNumber].Text = feedItem.Title.Text;
                        menuItems[itemNumber].Visibility = Visibility.Visible;

                        //We have to run this on the UI thread:
                        menuItems[itemNumber].Click += (sender, args) => Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                        {
                            var url = feedItem.Id;
                            WebViewControl.Navigate(new Uri(url));
                        });
                    }
                    itemNumber++;
                }
            }
            catch (Exception)
            {
                menuItems[0].Text = "Error Reading Feed!";
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            if(WebViewControl.CanGoBack)
                WebViewControl.GoBack();
        }

        private void Forward_Click(object sender, RoutedEventArgs e)
        {
            if(WebViewControl.CanGoForward)
                WebViewControl.GoForward();
        }
    }
}
