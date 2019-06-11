namespace GP.Tools.DriveService
{
    using GP.Tools.DriveService;

    public class DriveFolder : DriveItem, IFolder
    {
        public DriveFolder(string id, string name, string root, string driveType) : base(id, name, root, driveType)
        {
        }
    }
}
