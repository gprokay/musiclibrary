namespace GP.Tools.DriveService
{
    using GP.Tools.DriveService;

    public class DriveFile : DriveItem, IFile
    {
        public long Size { get; }

        public DriveFile(string id, string name, string root, string driveType, long size) : base(id, name, root, driveType)
        {
            Size = size;
        }
    }
}
