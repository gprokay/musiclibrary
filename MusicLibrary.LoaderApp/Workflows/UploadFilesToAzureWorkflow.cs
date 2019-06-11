using GP.Tools.DriveService;
using MusicLibrary.DataAccess.Context;
using MusicLibrary.Services.Library;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MusicLibrary.LoaderApp.Workflows
{
    public class UploadFilesToAzureWorkflow
    {
        private readonly MusicLibraryContext context;
        private readonly IDriveServiceFactory driveServiceFactory;
        private readonly LibraryService libraryService;

        public UploadFilesToAzureWorkflow(MusicLibraryContext context, IDriveServiceFactory driveServiceFactory, LibraryService libraryService)
        {
            this.context = context;
            this.driveServiceFactory = driveServiceFactory;
            this.libraryService = libraryService;
        }

        public async Task Execute()
        {
            Console.WriteLine("Uploading to Azure...");
            Console.Write("Searcing files to upload ... ");
            var filesToUpload = (await context.MusicFileRepository.GetFileSystemOnlyFiles(MusicLibraryData.MachineId)).ToList();

            Console.WriteLine($"{filesToUpload.Count} files found");
            var service = driveServiceFactory.GetService("azure", "lib");
            var targetFolder = await service.GetRootAsync();

            foreach (var sourceFile in filesToUpload)
            {
                Console.Write($"Uploading '{sourceFile.FileId}' ... ");
                await libraryService.CopyTo(sourceFile, targetFolder);
                Console.WriteLine($"done");
            }

            Console.WriteLine("Upload complete.");
        }
    }
}
