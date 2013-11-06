using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.UI.Notifications;
using Windows.Web.Syndication;

//Source: http://msdn.microsoft.com/en-us/library/windows/apps/jj991805.aspx

namespace YTechie
{
    public sealed class BlogFeedBackgroundTask : IBackgroundTask
    {
        private const string FeedUrl = @"http://ytechie.com/rss";

        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            // Get a deferral, to prevent the task from closing prematurely 
            // while asynchronous code is still running.
            var deferral = taskInstance.GetDeferral();

            // Download the feed.
            var feed = await GetBlogFeed();

            // Update the live tile with the feed items.
            UpdateTile(feed);

            // Inform the system that the task is finished.
            deferral.Complete();
        }

        private static async Task<SyndicationFeed> GetBlogFeed()
        {
            SyndicationFeed feed = null;

            try
            {
                // Create a syndication client that downloads the feed.  
                var client = new SyndicationClient {BypassCacheOnRetrieve = true};
                //client.SetRequestHeader(customHeaderName, customHeaderValue);

                // Download the feed. 
                feed = await client.RetrieveFeedAsync(new Uri(FeedUrl));
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }

            return feed;
        }

        private static void UpdateTile(SyndicationFeed feed)
        {
            // Create a tile update manager for the specified syndication feed.
            var updater = TileUpdateManager.CreateTileUpdaterForApplication();
            updater.EnableNotificationQueue(true);
            updater.Clear();

            // Keep track of the number feed items that get tile notifications. 
            var itemCount = 0;

            // Create a tile notification for each feed item.
            foreach (var item in feed.Items)
            {
                var tileXml = TileUpdateManager.GetTemplateContent(TileTemplateType.TileWideText03);

                var title = item.Title;
                string titleText = title.Text;
                tileXml.GetElementsByTagName("text")[0].InnerText = titleText;

                // Create a new tile notification. 
                updater.Update(new TileNotification(tileXml));

                // Don't create more than 5 notifications.
                if (itemCount++ > 5) break;
            }
        }
    }
}