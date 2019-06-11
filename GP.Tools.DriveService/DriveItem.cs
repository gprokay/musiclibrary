namespace GP.Tools.DriveService
{
    using GP.Tools.DriveService;

    public abstract class DriveItem : IDriveItem
    {
        public string DriveType { get; }

        public string Root { get; }

        public string Id { get; }

        public string Name { get; }

        protected DriveItem(string id, string name, string root, string driveType)
        {
            Id = id;
            Name = name;
            Root = root;
            DriveType = driveType;
        }
    }
}
