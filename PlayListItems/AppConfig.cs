namespace ConsolePlayListItems
{
    /// <summary>
    /// Options class used for binding app configuration using options pattern.
    /// https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/options?view=aspnetcore-5.0
    /// </summary>
    public class AppConfig
    {
        // Section
        public const string Google = "Google";

        // Bound property
        public string YouTubeApiKey { get; set; }
    }
}