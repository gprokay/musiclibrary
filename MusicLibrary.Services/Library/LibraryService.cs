using GP.Tools.DriveService;
using GP.Tools.DriveService.FileSystem;
using MusicLibrary.DataAccess.Context;
using MusicLibrary.DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MusicLibrary.Services.Library
{

    public class LibraryService
    {
        private readonly ITrackFileCollector fileCollector;
        private readonly IDriveServiceFactory driveServiceFactory;
        private readonly MusicLibraryContext context;
        private readonly ITrackInfoCollector infoCollector;

        public LibraryService(ITrackFileCollector fileCollector, IDriveServiceFactory driveServiceFactory, MusicLibraryContext context, ITrackInfoCollector infoCollector)
        {
            this.fileCollector = fileCollector;
            this.driveServiceFactory = driveServiceFactory;
            this.context = context;
            this.infoCollector = infoCollector;
        }

        public async Task SaveAsync(List<TrackInfoResult> trackInfoResults)
        {
            var libraryData = new MusicLibraryData(driveServiceFactory);
            await libraryData.BuildDataAsync(trackInfoResults);
            await Save(libraryData);
        }

        public async Task<List<TrackInfoResult>> SearchNewFiles(string folder)
        {
            var files = await fileCollector.FindTracksAsync(folder);
            var existingFiles = await context.MusicFileRepository.GetMusicFileIds(MusicLibraryData.MachineId, FileSystemDriveService.DriveTypeName);
            var existingFileIds = new HashSet<string>(existingFiles.Select(f => f.ToLowerInvariant()));
            var newFiles = files.Where(f => !existingFileIds.Contains(f.Id)).ToList();
            return await infoCollector.GetTrackInfosAsync(newFiles);
        }

        public async Task CopyTo(MusicFileDo sourceFile, IFolder targetFolder)
        {
            var sourceFileService = driveServiceFactory.GetService(sourceFile.DriveType, sourceFile.Root);
            var targetDriveService = driveServiceFactory.GetService(targetFolder.DriveType, targetFolder.Root);

            var fs = new DriveFile(sourceFile.FileId, sourceFile.Name, sourceFile.Root, sourceFile.DriveType, sourceFile.Length);
            var sourceStream = await sourceFileService.GetFileContentStreamAsync(fs);

            var name = Guid.NewGuid() + sourceFileService.GetExtension(sourceFile.FileId);

            var targetFile = await targetDriveService.CopyToAsync(targetFolder, sourceStream, name);

            var targetMusicFile = new MusicFileDo
            {
                TrackId = sourceFile.TrackId,
                FileId = targetFile.Id,
                DriveType = targetFile.DriveType,
                Length = targetFile.Size,
                Name = targetFile.Name,
                Root = targetFile.Root,
                MachineId = MusicLibraryData.MachineId
            };

            await context.MusicFileRepository.Save(new List<MusicFileDo> { targetMusicFile });
        }

        private async Task Save(MusicLibraryData data)
        {
            using (var uow = new UnitOfWork())
            {
                await context.AlbumRepository.Save(data.Albums.ToList());
                await context.ArtistRepository.Save(data.Artists.ToList());
                await context.GenreRepository.Save(data.Genres.ToList());
                await context.TrackRepository.Save(data.Tracks.ToList());
                await context.TrackGenreRepository.Save(data.TrackGenres.ToList());
                await context.AlbumArtistRepository.Save(data.AlbumArtists.ToList());
                await context.TrackArtistRepository.Save(data.TrackArtists.ToList());
                await context.MusicFileRepository.Save(data.MusicFiles.ToList());
                await context.TrackFileRepostiory.Save(data.TrackFiles.ToList());
                uow.Complete();
            }
        }
    }
}
