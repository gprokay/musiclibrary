using GP.Tools.DriveService;
using GP.Tools.DriveService.AzureFileStorage;
using GP.Tools.DriveService.FileSystem;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.File;
using System;

namespace MusicLibrary.Services.DriveService
{
    public class DriveServiceFactory : IDriveServiceFactory
    {
        private readonly string storeageConnectionString;
        private readonly string shareReference;

        public DriveServiceFactory(string storeageConnectionString, string shareReference)
        {
            this.storeageConnectionString = storeageConnectionString;
            this.shareReference = shareReference;
        }

        public IDriveService GetService(string driveType, string root)
        {
            switch (driveType)
            {
                case FileSystemDriveService.DriveTypeName:
                    return new FileSystemDriveService(root);
                case AzureStorageDriveService.DriveTypeName:
                    CloudStorageAccount storageAccount = CloudStorageAccount.Parse(storeageConnectionString);
                    CloudFileClient fileClient = storageAccount.CreateCloudFileClient();
                    CloudFileShare share = fileClient.GetShareReference(shareReference);
                    return new AzureStorageDriveService(share);
                default:
                    throw new NotSupportedException();
            }
        }
    }
}
