using GP.Tools.DriveService;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MusicLibrary.DataAccess.Context;
using MusicLibrary.LoaderApp.Workflows;
using MusicLibrary.Services.Library;
using MusicLibrary.Settings;
using System.Threading.Tasks;

namespace MusicLibrary.LoaderApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            IConfiguration config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", true, true)
                .Build();

            config = DependencyBootstrapper.AddSecrets(config);

            var services = new ServiceCollection();
            DependencyBootstrapper.ConfigureDependencies(services, config);

            var provider = services.BuildServiceProvider();

            var context = provider.GetService<MusicLibraryContext>();

            var libMan = provider.GetService<LibraryService>();
            var saveWorkflow = new SaveNewFilesToDatabaseWorkflow(libMan);

            foreach (var path in args)
            {
                await saveWorkflow.Execute(path);
            }

            var uploadWorkflow = new UploadFilesToAzureWorkflow(context, provider.GetService<IDriveServiceFactory>(), libMan);
            await uploadWorkflow.Execute();

        }
    }
}
