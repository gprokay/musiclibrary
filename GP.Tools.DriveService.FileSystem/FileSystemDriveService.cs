using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GP.Tools.DriveService.FileSystem
{
    public class FileSystemDriveService : IDriveService
    {
        public const string DriveTypeName = "filesystem";

        public string Root { get; }

        public string Delimiter => "\\";

        public string DriveType => DriveTypeName;

        public FileSystemDriveService(string root)
        {
            Root = root;
        }

        public Task<IFolder> GetRootAsync()
        {
            return Task<IFolder>.Factory.StartNew(() => new FileSystemFolder(Root, Root, Root));
        }

        public Task<IReadOnlyCollection<IDriveItem>> GetChildrenAsync(IFolder folder)
        {
            return Task<IReadOnlyCollection<IDriveItem>>.Factory.StartNew(
                () =>
                    {
                        return
                            Directory.GetDirectories(folder.Id)
                                .Select(d => new FileSystemFolder(new DirectoryInfo(d)))
                                .Union(
                                    Directory.GetFiles(folder.Id)
                                        .Select(f => new FileSystemFile(new FileInfo(f)))
                                        .Cast<IDriveItem>())
                                .ToList();
                    });
        }

        public async Task<IFile> CopyToAsync(
            IFolder folder,
            Stream fileContent,
            string fileName,
            CancellationToken? token = null,
            IProgress<FileCopyProgress> progress = null)
        {
            var state = new FileCopyProgress { TotalSize = fileContent.Length, Name = fileName };
            progress?.Report(state);
            var targetPath = Path.Combine(folder.Id, fileName);
            using (var stream = new FileStream(targetPath, FileMode.Create, FileAccess.Write, FileShare.Write))
            {
                var bufferSize = 1024 * 1024;
                var buffer = new byte[bufferSize];
                int count;
                while ((count = token.HasValue
                    ? await fileContent.ReadAsync(buffer, 0, buffer.Length, token.Value)
                    : await fileContent.ReadAsync(buffer, 0, buffer.Length)) != 0)
                {
                    if (token.HasValue)
                    {
                        await stream.WriteAsync(buffer, 0, count, token.Value);
                    }
                    else
                    {
                        await stream.WriteAsync(buffer, 0, count);
                    }

                    state.CopiedSize += count;
                    progress?.Report(state);
                }
            }

            return new DriveFile(targetPath, fileName, Root, DriveType, state.CopiedSize);
        }

        public Task<IFolder> CreateFolderAsync(IFolder folder, string name)
        {
            var path = Path.Combine(folder.Id, name);
            Directory.CreateDirectory(path);
            return Task.FromResult<IFolder>(new FileSystemFolder(new DirectoryInfo(path)));
        }

        public Task<IDriveItem> GetItemByFileIdAysnc(string path)
        {
            if (File.Exists(path))
            {
                return Task.FromResult<IDriveItem>(new FileSystemFile(new FileInfo(path)));
            }

            if (Directory.Exists(path))
            {
                return Task.FromResult<IDriveItem>(new FileSystemFolder(new DirectoryInfo(path)));
            }

            return Task.FromResult<IDriveItem>(null);
        }

        public Task<IFolder> GetFolderWithIdAysnc(string id)
        {
            return Task<IFolder>.Factory.StartNew(
                () => Directory.Exists(id)
                          ? new FileSystemFolder(new DirectoryInfo(id))
                          : null);

        }

        public Task<IFolder> CreateFolderAsync(string path)
        {
            Directory.CreateDirectory(path);
            return Task.FromResult<IFolder>(new FileSystemFolder(new DirectoryInfo(path)));
        }

        public string GetParentId(string path)
        {
            if (path == null)
            {
                throw new ArgumentNullException();
            }

            if (File.Exists(path))
            {
                return new FileInfo(path).Directory.FullName;
            }

            if (Directory.Exists(path))
            {
                return new DirectoryInfo(path).Parent?.FullName;
            }

            return path;
        }

        public Task<Stream> GetFileContentStreamAsync(IFile file)
        {
            return Task.FromResult<Stream>(new FileStream(file.Id, FileMode.Open, FileAccess.Read, FileShare.Read));
        }

        public async Task<byte[]> GetContentAsync(IFile file, long offset, long lenght)
        {
            using (var stream = await GetFileContentStreamAsync(file))
            {
                var buffer = new byte[lenght];
                stream.Seek(offset, SeekOrigin.Begin);
                var readLenght = await stream.ReadAsync(buffer, (int)0, (int)lenght);
                var output = new byte[readLenght];
                Array.Copy(buffer, output, readLenght);
                return output;
            }

        }

        public string GetExtension(string fileId)
        {
            return Path.GetExtension(fileId);
        }
    }
}
