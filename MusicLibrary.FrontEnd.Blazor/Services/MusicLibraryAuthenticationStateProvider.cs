using Blazor.Extensions.Storage;
using Blazor.Extensions.Storage.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MusicLibrary.Model;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MusicLibrary.FrontEnd.Blazor.Services
{
    public class MusicLibraryAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly HttpClient httpClient;
        private readonly ILocalStorage localStorage;

        private string token;
        private User user;
        private ClaimsPrincipal principal;

        public MusicLibraryAuthenticationStateProvider(HttpClient httpClient, ILocalStorage localStorage)
        {
            this.httpClient = httpClient;
            this.localStorage = localStorage;
        }

        public async Task SetToken(string token)
        {
            this.token = token;
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            await localStorage.SetItem("token", token);
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }

        public async Task ClearToken()
        {
            token = null;
            user = null;
            principal = null;
            httpClient.DefaultRequestHeaders.Authorization = null;
            await localStorage.RemoveItem("token");
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            if (string.IsNullOrEmpty(token))
            {
                token = await localStorage.GetItem<string>("token");
            }

            if (string.IsNullOrEmpty(token))
            {
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(new Claim[0])));
            }
            else
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            if (user == null)
            {
                try
                {
                    user = await httpClient.GetJsonAsync<User>("user/me");
                    principal = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, user.UserName) }, "jwt"));
                }
                catch (HttpRequestException)
                {
                    principal = new ClaimsPrincipal(new ClaimsIdentity(new Claim[0]));
                }
            }

            return new AuthenticationState(principal); 
        }
    }
}
