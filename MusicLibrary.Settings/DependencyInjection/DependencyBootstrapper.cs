using AutoMapper;
using GP.Tools.DriveService;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MusicLibrary.DataAccess.Context;
using MusicLibrary.Services;
using MusicLibrary.Services.DriveService;
using MusicLibrary.Services.Library;
using System;

namespace MusicLibrary.Settings
{
    public static class DependencyBootstrapper
    {
        public static IConfiguration AddSecrets(IConfiguration configuration)
        {
            return new ConfigurationBuilder().AddConfiguration(configuration).AddUserSecrets("8e52a36e-46e8-4f82-a749-fc2153eea045").Build();
        }

        public static void ConfigureDependencies(IServiceCollection services, IConfiguration configuration)
        {
            var appSettings = configuration.GetSection("AppSettings").Get<AppSettings>();

            services.AddSingleton<MusicLibraryContext, MusicLibraryContext>(provider => new MusicLibraryContext(configuration.GetConnectionString("library")));
            services.AddSingleton<IDriveServiceFactory, DriveServiceFactory>(provider => new DriveServiceFactory(appSettings.AzureFileStoreConnectionString, appSettings.AzureFileShareReference));
            services.AddSingleton<ITrackFileCollector, TrackFileCollector>();
            services.AddSingleton<ITrackInfoCollector, TaglibTrackInfoCollector>();
            services.AddSingleton<LibraryService, LibraryService>();
            services.AddSingleton<TrackService, TrackService>();
            services.AddSingleton<TrackStreamService, TrackStreamService>(provider => new TrackStreamService(provider.GetService<MusicLibraryContext>(), provider.GetService<IDriveServiceFactory>(), appSettings.TokenSecret));
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
        }
    }
}
