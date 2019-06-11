namespace GP.Tools.DriveService.FileSystem
{
    using System.IO;
    using GP.Tools.DriveService;

    public class FileSystemFolder : DriveFolder
    {
        public FileSystemFolder(DirectoryInfo dir)
            : base(dir.FullName, dir.Name, dir.FullName.Substring(0, 2), FileSystemDriveService.DriveTypeName)
        {
        }

        public FileSystemFolder(string id, string name, string root)
            : base(id, name, root, FileSystemDriveService.DriveTypeName)
        {
        }
    }
}
