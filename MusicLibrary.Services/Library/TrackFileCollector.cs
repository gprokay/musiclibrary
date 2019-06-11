using GP.Tools.DriveService;
using GP.Tools.DriveService.FileSystem;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MusicLibrary.Services.Library
{
    public class TrackFileCollector : ITrackFileCollector
    {
        private readonly IDriveServiceFactory driveServiceFactory;

        public TrackFileCollector(IDriveServiceFactory driveServiceFactory)
        {
            this.driveServiceFactory = driveServiceFactory;
        }

        public async Task<IEnumerable<IFile>> FindTracksAsync(string startPath, CancellationToken? cancellationToken = null, IProgress<int> searchProgress = null)
        {
            var service = driveServiceFactory.GetService(FileSystemDriveService.DriveTypeName, string.Empty);
            var startFolder = await service.GetFolderWithIdAysnc(startPath);
            var queue = new Queue<IFolder>();
            queue.Enqueue(startFolder);

            var result = new List<IFile>();
            while (queue.Count > 0)
            {
                if (cancellationToken.HasValue && cancellationToken.Value.IsCancellationRequested)
                {
                    return result;
                }

                var current = queue.Dequeue();
                try
                {
                    var children = await service.GetChildrenAsync(current);
                    foreach (var child in children)
                    {
                        if (cancellationToken.HasValue && cancellationToken.Value.IsCancellationRequested)
                        {
                            return result;
                        }

                        if (child is IFile file && (file.Name.EndsWith("mp3") || file.Name.EndsWith("flac")))
                        {
                            result.Add(file);
                            searchProgress?.Report(result.Count);
                        }

                        if (child is IFolder folder)
                        {
                            queue.Enqueue(folder);
                        }
                    }
                }
                catch (UnauthorizedAccessException)
                {
                }
            }

            return result;
        }
    }
}
