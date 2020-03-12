using Blazor.Extensions.Storage;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Blazor.Hosting;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection;
using MusicLibrary.FrontEnd.Blazor.Services;
using System.Threading.Tasks;

namespace MusicLibrary.FrontEnd.Blazor
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");
            builder.Services.AddSingleton<AuthenticationStateProvider, MusicLibraryAuthenticationStateProvider>();
            builder.Services.AddSingleton<IAuthorizationPolicyProvider, AuthorizationPolicyProvider>();
            builder.Services.AddSingleton<IAuthorizationService, AuthorizationService>();
            builder.Services.AddSingleton<PlayService, PlayService>();
            builder.Services.AddSingleton<TrackClientService, TrackClientService>();
            builder.Services.AddStorage();
            await builder.Build().RunAsync();
        }
    }
}
