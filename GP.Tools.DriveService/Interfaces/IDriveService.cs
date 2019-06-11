namespace GP.Tools.DriveService
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;

    public interface IDriveService
    {
        string Root { get; }

        string Delimiter { get; }

        string DriveType { get; }

        Task<IFolder> GetRootAsync();

        Task<IReadOnlyCollection<IDriveItem>> GetChildrenAsync(IFolder folder);

        Task<IFile> CopyToAsync(IFolder folder, Stream fileContent, string fileName, CancellationToken? token = null, IProgress<FileCopyProgress> progress = null);

        Task<IFolder> CreateFolderAsync(IFolder folder, string name);

        Task<IDriveItem> GetItemByFileIdAysnc(string path);

        Task<IFolder> GetFolderWithIdAysnc(string id);

        Task<IFolder> CreateFolderAsync(string path);

        string GetParentId(string path);

        Task<byte[]> GetContentAsync(IFile file, long offset, long lenght);

        Task<Stream> GetFileContentStreamAsync(IFile file);

        string GetExtension(string fileId);
    }
}
