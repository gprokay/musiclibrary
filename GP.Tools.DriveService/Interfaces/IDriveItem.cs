namespace GP.Tools.DriveService
{
    public interface IDriveItem
    {
        string DriveType { get; }

        string Root { get; }

        string Id { get; }

        string Name { get; }
    }
}