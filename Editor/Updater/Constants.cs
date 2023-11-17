namespace Editor.Updater
{

    internal static class Constants
    {

        private const string ApiUrlFormat = "https://api.github.com/repos/{0}/{1}/";

        public const string Tags = ApiUrlFormat + "tags";

        public const string Compare = ApiUrlFormat + "compare/v{2}...v{3}";

        public const string GitHubUsername = "Axwabo";

        public const string Repository = "slocExporter";

        public const string RateLimit = "You have been rate limited by the GitHub API.\nTry again at {0}";

        public const string CheckFailed = "Failed to check for sloc updates";

        public const string ReceiveFailed = "sloc update failed";

        public const string ReceivingChanges = "Receiving changes...";

        public const string DownloadingFiles = "Downloading files...";

        public const string ArchiveFileName = "slocUpdateArchive.zip";

        public const string CheckingForUpdates = "Checking for updates...";

    }

    public delegate void ProgressUpdater(string message, float progress);

}
