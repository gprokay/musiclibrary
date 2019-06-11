using Microsoft.WindowsAzure.Storage.File;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace GP.Tools.DriveService.AzureFileStorage
{
    public class AzureStorageDriveService : IAzureStorageDriveService
    {
        public const string DriveTypeName = "azure";

        private readonly CloudFileShare fileShare;

        public string Root { get; }

        public string Delimiter => "/";

        public string DriveType => DriveTypeName;

        public AzureStorageDriveService(CloudFileShare fileShare)
        {
            this.fileShare = fileShare;
            Root = fileShare.Name;
        }

        public Task<IFolder> GetRootAsync()
        {
            return Task<IFolder>.Factory.StartNew(() =>
            {
                var root = fileShare.GetRootDirectoryReference();
                return new DriveFolder(root.Uri.ToString(), root.Name, Root, DriveType);
            });
        }

        public Task<IReadOnlyCollection<IDriveItem>> GetChildrenAsync(IFolder folder)
        {
            throw new NotImplementedException();
            //var uri = new Uri(folder.Id);
            //var azureFolder = new CloudFileDirectory(uri, this._fileShare.ServiceClient.Credentials);
            //await azureFolder.FetchAttributesAsync();
            //var result = new List<IDriveItem>();
            //foreach (var item in (await azureFolder.ListFilesAndDirectoriesSegmentedAsync(null)))
            //{
            //    try
            //    {
            //        var file = new CloudFile(item.Uri, this._fileShare.ServiceClient.Credentials);
            //        if (await file.ExistsAsync())
            //        {
            //            await file.FetchAttributesAsync();
            //            result.Add(new DriveFile(file.Uri.ToString(), file.Name, this.Root, this.DriveType, file.Properties.Length));
            //            continue;
            //        }
            //    }
            //    catch (StorageException)
            //    {
            //    }

            //    try
            //    {
            //        var dir = new CloudFileDirectory(item.Uri, this._fileShare.ServiceClient.Credentials);
            //        if (await dir.ExistsAsync())
            //        {
            //            await dir.FetchAttributesAsync();
            //            result.Add(new DriveFolder(dir.Uri.ToString(), dir.Name, this.Root, this.DriveType));
            //        }
            //    }
            //    catch (StorageException)
            //    {
            //    }
            //}

            //return result;
        }

        public async Task<IFile> CopyToAsync(IFolder folder, Stream fileContent, string fileName, CancellationToken? token = null, IProgress<FileCopyProgress> progress = null)
        {
            var destinationDir = new CloudFileDirectory(new Uri(folder.Id), fileShare.ServiceClient.Credentials);
            var file = destinationDir.GetFileReference(fileName);
            await file.UploadFromStreamAsync(fileContent);
            return new DriveFile(file.Uri.ToString(), file.Name, Root, DriveType, fileContent.Length);
        }

        public async Task<IFolder> CreateFolderAsync(IFolder folder, string name)
        {
            var dir = new CloudFileDirectory(new Uri(folder.Id), fileShare.ServiceClient.Credentials);
            var newFolder = dir.GetDirectoryReference(name);
            if (!await newFolder.ExistsAsync())
            {
                await newFolder.CreateAsync();
            }
            else
            {
                await newFolder.FetchAttributesAsync();
            }

            return new DriveFolder(newFolder.Uri.ToString(), newFolder.Name, Root, DriveType);
        }

        public async Task<IDriveItem> GetItemByFileIdAysnc(string path)
        {
            var file = new CloudFile(new Uri(path), fileShare.ServiceClient.Credentials);
            if (await file.ExistsAsync())
            {
                await file.FetchAttributesAsync();
                return new DriveFile(file.Uri.ToString(), file.Name, Root, DriveType, file.Properties.Length);
            }

            var dir = new CloudFileDirectory(new Uri(path), fileShare.ServiceClient.Credentials);
            if (await dir.ExistsAsync())
            {
                return new DriveFolder(dir.Uri.ToString(), dir.Name, Root, DriveType);
            }

            return null;
            //var split = new Queue<string>(path.Split(new[] { Delimiter }, StringSplitOptions.RemoveEmptyEntries));

            //var folder = this._fileShare.GetRootDirectoryReference();
            //while (split.Count > 1)
            //{
            //    var current = split.Dequeue();
            //    folder = folder.GetDirectoryReference(current);
            //}

            //var name = split.Dequeue();
            //var file = folder.GetFileReference(name);
            //if (file.Exists())
            //{
            //    await file.FetchAttributesAsync();
            //    return new DriveFile(file.Uri.ToString(), file.Name, Root, DriveType, file.Properties.Length);
            //}

            //var dir = folder.GetDirectoryReference(name);
            //if (dir.Exists())
            //{
            //    return new DriveFolder(dir.Uri.ToString(), dir.Name, Root, DriveType);
            //}

            //return null;
        }

        public async Task<IFolder> GetFolderWithIdAysnc(string id)
        {
            var folder = new CloudFileDirectory(new Uri(id), fileShare.ServiceClient.Credentials);
            if (await folder.ExistsAsync())
            {
                return new DriveFolder(id, folder.Name, Root, DriveType);
            }

            return null;
        }

        public async Task<IFolder> CreateFolderAsync(string path)
        {
            var split = new Queue<string>(path.Split(new[] { Delimiter }, StringSplitOptions.RemoveEmptyEntries));

            var folder = fileShare.GetRootDirectoryReference();

            while (split.Count > 0)
            {
                var current = split.Dequeue();
                folder = folder.GetDirectoryReference(current);
                if (!await folder.ExistsAsync())
                {
                    await folder.CreateAsync();
                }
            }

            return new DriveFolder(folder.Uri.ToString(), folder.Name, Root, DriveType);
        }

        public string GetParentId(string path)
        {
            return null;
        }

        public async Task<Stream> GetFileContentStreamAsync(IFile file)
        {
            var cloudFile = new CloudFile(new Uri(file.Id), fileShare.ServiceClient.Credentials);
            Stream memoryStream = new MemoryStream();
            await cloudFile.DownloadToStreamAsync(memoryStream);
            memoryStream.Seek(0, SeekOrigin.Begin);
            return memoryStream;
        }

        public async Task<byte[]> GetContentAsync(IFile file, long offset, long lenght)
        {
            var azureFile = new CloudFile(new Uri(file.Id), fileShare.ServiceClient.Credentials);
            using (var memoryStream = new MemoryStream())
            {
                await azureFile.DownloadRangeToStreamAsync(memoryStream, offset, lenght);
                return memoryStream.ToArray();
            }
        }

        public string GetExtension(string file)
        {
            throw new NotImplementedException();
        }
    }
}

