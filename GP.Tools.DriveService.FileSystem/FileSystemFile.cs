namespace GP.Tools.DriveService.FileSystem
{
    using GP.Tools.DriveService;
    using System.IO;

    public class FileSystemFile : DriveFile
    {
        public FileSystemFile(FileInfo file)
            : base(file.FullName.ToLowerInvariant(), file.Name.ToLowerInvariant(), file.FullName.ToLowerInvariant().Substring(0, 2), FileSystemDriveService.DriveTypeName, file.Length)
        {
        }
    }
}
