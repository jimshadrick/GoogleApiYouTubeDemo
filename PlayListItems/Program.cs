using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Microsoft.Extensions.Configuration;
using System;
using System.Globalization;
using System.Threading.Tasks;

namespace ConsolePlayListItems
{
    internal class ChannelUploads
    {
        const string channelUser = "IAmTimCorey";

        [STAThread]
        static void Main(string[] args)
        {
            // Load configuration options to retrieve api key.
            var cfg = InitOptions<AppConfig>();

            Console.WriteLine($"YouTube Data API: Channel Uploads for {channelUser}");
            Console.WriteLine("============================");

            try
            {
                new ChannelUploads().Run(cfg.YouTubeApiKey).Wait();
            }
            catch (AggregateException ex)
            {
                foreach (var e in ex.InnerExceptions)
                {
                    Console.WriteLine("Error: " + e.Message);
                }
            }

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        public async Task Run(string apiKey)
        {
            var youtubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                ApiKey = apiKey,
                ApplicationName = this.GetType().ToString()
            });

            var channelsListRequest = youtubeService.Channels.List("contentDetails");

            channelsListRequest.ForUsername = channelUser;

            // Retrieve the contentDetails part of the channel resource for the specific user's channel.
            var channelsListResponse = await channelsListRequest.ExecuteAsync();

            foreach (var channel in channelsListResponse.Items)
            {
                // From the API response, extract the playlist ID that identifies the list
                // of videos uploaded to the user's channel.
                var uploadsListId = channel.ContentDetails.RelatedPlaylists.Uploads;

                Console.WriteLine($"Videos in list {uploadsListId}");

                var nextPageToken = "";
                while (nextPageToken != null)
                {
                    var playlistItemsListRequest = youtubeService.PlaylistItems.List("snippet");
                    playlistItemsListRequest.PlaylistId = uploadsListId;
                    playlistItemsListRequest.MaxResults = 50;
                    playlistItemsListRequest.PageToken = nextPageToken;

                    // Retrieve the list of videos uploaded to the authenticated user's channel.
                    var playlistItemsListResponse = await playlistItemsListRequest.ExecuteAsync();

                    foreach (var playlistItem in playlistItemsListResponse.Items)
                    {
                        //TODO: Populate these values in an array/list that can be sorted by publish date.                    
                        // Print information about each video.
                        DateTime publishDate = DateTime.Parse(playlistItem.Snippet.PublishedAt, CultureInfo.InvariantCulture); //ISO 8601 format
                        Console.WriteLine($"{playlistItem.Snippet.Title}, {playlistItem.Snippet.ResourceId.VideoId}, " +
                            $"{publishDate.ToString("d")}");
                    }
                    nextPageToken = playlistItemsListResponse.NextPageToken;
                }
            }
        }

        // Binds the configurations to options defined in our app config class.
        private static T InitOptions<T>()
            where T : new()
        {
            var config = InitConfig();
            return config.Get<T>();
        }

        // Retrieve configurations from the various sources.
        private static IConfigurationRoot InitConfig()
        {
            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var builder = new ConfigurationBuilder()
                .AddJsonFile($"appsettings.json", true, true)
                .AddJsonFile($"appsettings.{env}.json", true, true)
                .AddEnvironmentVariables();

            return builder.Build();
        }

    }
}
