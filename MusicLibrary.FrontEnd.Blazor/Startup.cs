using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Blazor.Extensions.Storage;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Builder;
using Microsoft.Extensions.DependencyInjection;
using MusicLibrary.FrontEnd.Blazor.Services;

namespace MusicLibrary.FrontEnd.Blazor
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<AuthenticationStateProvider, MusicLibraryAuthenticationStateProvider>();
            services.AddSingleton<IAuthorizationPolicyProvider, X>();
            services.AddSingleton<IAuthorizationService, Y>();
            services.AddSingleton<PlayService, PlayService>();
            services.AddSingleton<TrackClientService, TrackClientService>();
            services.AddStorage();
        }

        public void Configure(IComponentsApplicationBuilder app)
        {
            app.AddComponent<App>("app");
        }
    }

    public class Y : IAuthorizationService
    {
        public Task<AuthorizationResult> AuthorizeAsync(ClaimsPrincipal user, object resource, IEnumerable<IAuthorizationRequirement> requirements)
        {
            return Task.FromResult(user.Identity.IsAuthenticated ? AuthorizationResult.Success() : AuthorizationResult.Failed());
        }

        public Task<AuthorizationResult> AuthorizeAsync(ClaimsPrincipal user, object resource, string policyName)
        {
            return Task.FromResult(user.Identity.IsAuthenticated ? AuthorizationResult.Success() : AuthorizationResult.Failed());
        }
    }

    public class DefaultRequirment : IAuthorizationRequirement
    {
    }

    public class X : IAuthorizationPolicyProvider
    {
        public Task<AuthorizationPolicy> GetDefaultPolicyAsync()
        {
            return Task.FromResult(new AuthorizationPolicy(new IAuthorizationRequirement[] { new DefaultRequirment() }, new string[0]));
        }

        public Task<AuthorizationPolicy> GetFallbackPolicyAsync()
        {
            return Task.FromResult(new AuthorizationPolicy(new IAuthorizationRequirement[] { new DefaultRequirment() }, new string[0]));
        }

        public Task<AuthorizationPolicy> GetPolicyAsync(string policyName)
        {
            return Task.FromResult(new AuthorizationPolicy(new IAuthorizationRequirement[] { new DefaultRequirment() }, new string[0]));
        }
    }
}
