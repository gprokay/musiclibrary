namespace GP.Tools.DriveService
{
    public interface IDriveServiceFactory
    {
        IDriveService GetService(string driveType, string root);
    }
}
