namespace GP.Tools.DriveService
{
    public interface IFile : IDriveItem
    {
        long Size { get; }
    }
}