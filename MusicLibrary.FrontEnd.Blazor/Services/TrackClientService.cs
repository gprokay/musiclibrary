using Microsoft.AspNetCore.Components;
using MusicLibrary.Model;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace MusicLibrary.FrontEnd.Blazor.Services
{
    public class TrackLibraryToggledEventArgs
    {
        public int TrackId { get; set; }

        public bool Toggle { get; set; }
    }

    public class TrackClientService
    {
        private readonly HttpClient http;

        public event EventHandler<TrackLibraryToggledEventArgs> OnLibraryToggle;

        public TrackClientService(HttpClient http)
        {
            this.http = http;
        }

        public Task<ListResult<Track>> SearchTracks(string search)
        {
            return http.PostJsonAsync<ListResult<Track>>("track/search", new SearchTrackRequest
            {
                Filter = new TrackFilter
                {
                    Title = search
                },
                Page = new Page<TrackOrderColumn>
                {
                    Skip = 0,
                    Take = 10,
                    OrderBy = new OrderModel<TrackOrderColumn>
                    {
                        Column = TrackOrderColumn.Title,
                        Direction = OrderDirection.Descending
                    }
                }
            });
        }

        public Task<Track> GetTrack(int trackId)
        {
            return http.GetJsonAsync<Track>("track/" + trackId);
        }

        public async Task<string> GetStreamUrl(int trackId)
        {
            var token = await http.GetStringAsync("stream/token/" + trackId);
            var uriBuilder = new UriBuilder(http.BaseAddress);
            uriBuilder.Path = "stream/" + token;
            return uriBuilder.ToString();
        }

        public async Task ToggleTrackInLibrary(int trackId, bool toggle)
        {
            if (toggle)
            {
                await http.PostAsync("track/library/" + trackId, null);
            }
            else
            {
                await http.DeleteAsync("track/library/" + trackId);
            }

            OnLibraryToggle?.Invoke(this, new TrackLibraryToggledEventArgs { TrackId = trackId, Toggle = toggle });
        }
    }
}
