using MusicLibrary.Services.Library;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MusicLibrary.LoaderApp.Workflows
{
    public class SaveNewFilesToDatabaseWorkflow
    {
        private readonly LibraryService libraryService;

        public SaveNewFilesToDatabaseWorkflow(LibraryService libraryService)
        {
            this.libraryService = libraryService;
        }

        public async Task Execute(string startFolder)
        {
            Console.WriteLine($"Saving new files in: '{startFolder}'");
            Console.Write("Searching files ... ");
            var result = await libraryService.SearchNewFiles(startFolder);
            Console.WriteLine($"{result.Count} files found");

            var groups = result
                .Select(ti => new { Info = ti, Folder = Path.GetDirectoryName(ti.File.Id) })
                .GroupBy(ti => ti.Folder, ti => ti.Info);

            foreach (var group in groups)
            {
                var tracks = group.ToList();
                Console.Write($"Saving {tracks.Count} tracks in '{group.Key}' ... ");
                await libraryService.SaveAsync(tracks);
                Console.WriteLine($"done");
            }

            Console.WriteLine("Save compete.");
        }
    }
}
