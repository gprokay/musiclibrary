using GP.Tools.DriveService;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MusicLibrary.Services.Library
{
    public class TaglibTrackInfoCollector : ITrackInfoCollector
    {
        private readonly IDriveServiceFactory _serviceFactory;

        public TaglibTrackInfoCollector(IDriveServiceFactory serviceFactory)
        {
            _serviceFactory = serviceFactory;
        }

        public async Task<List<TrackInfoResult>> GetTrackInfosAsync(IEnumerable<IFile> files, CancellationToken? cancellationToken = null, IProgress<int> progress = null)
        {
            var result = new List<TrackInfoResult>();

            foreach (var file in files)
            {
                if (cancellationToken.HasValue && cancellationToken.Value.IsCancellationRequested)
                {
                    break;
                }

                var service = _serviceFactory.GetService(file.DriveType, file.Root);
                var fileAbstraction = new StreamedFileAbstraction(file.Name, await service.GetFileContentStreamAsync(file));
                try
                {
                    var tagFile = global::TagLib.File.Create(fileAbstraction);

                    var album = tagFile.Tag.Album;
                    if (string.IsNullOrEmpty(album))
                    {
                        var folderId = service.GetParentId(file.Id);
                        var folder = await service.GetFolderWithIdAysnc(folderId);
                        album = folder.Name;
                    }

                    var title = tagFile.Tag.Title;
                    if (string.IsNullOrEmpty(title))
                    {
                        title = file.Name;
                    }

                    var info = new TrackInfo
                    {
                        Id = file.Id,
                        Album = album,
                        AlbumArtists = tagFile.Tag.AlbumArtists,
                        Artists = tagFile.Tag.Performers,
                        DiscNumber = (int)tagFile.Tag.Disc,
                        TrackNumber = (int)tagFile.Tag.Track,
                        Genres = tagFile.Tag.Genres,
                        Title = title,
                        Duration = tagFile.Properties.Duration
                    };

                    result.Add(new TrackInfoResult(info, file));
                    progress?.Report(result.Count);
                }
                catch
                {
                }
            }

            return result;
        }
    }
}
