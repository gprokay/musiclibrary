namespace MusicLibrary.Settings
{
    public class AppSettings
    {
        public string TokenSecret { get; set; }

        public string GoogleClientId { get; set; }

        public string GoogleClientSecret { get; set; }

        public string AzureFileStoreConnectionString { get; set; }

        public string AzureFileShareReference { get; set; }
    }
}
