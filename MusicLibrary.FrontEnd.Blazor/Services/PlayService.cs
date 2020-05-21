using Microsoft.AspNetCore.Components;
using MusicLibrary.Model;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace MusicLibrary.FrontEnd.Blazor.Services
{
    public class PlayTrackEventArgs
    {
        public Track Track { get; set; }

        public bool HasNextTrack { get; set; }

        public bool HasPreviousTrack { get; set; }
    }

    public class PlayService
    {
        private readonly object lockObject = new object();

        private readonly HttpClient httpClient;

        private int currentlyPlayingIndex = -1;
        private int[] trackIds = new int[0];

        public event EventHandler<PlayTrackEventArgs> OnPlayTrack;

        public PlayService(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public void SetTracks(int[] trackIds)
        {
            lock (lockObject)
            {
                this.trackIds = trackIds;
                currentlyPlayingIndex = -1;
            }
        }

        public void AddTracks(int[] trackIds)
        {
            lock (lockObject)
            {
                this.trackIds = this.trackIds.Concat(trackIds).ToArray();
            }
        }

        public Task PlayNext()
        {
            var trackId = 0;

            if (trackIds.Length == 0) return Task.CompletedTask;

            lock (lockObject)
            {
                currentlyPlayingIndex++;
                if (currentlyPlayingIndex >= trackIds.Length)
                {
                    currentlyPlayingIndex = 0;
                }

                trackId = trackIds[currentlyPlayingIndex];
            }

            return GetTrackAndTriggerEvent(trackId);
        }

        public Task PlayPrevious()
        {
            var trackId = 0;

            if (trackIds.Length == 0) return Task.CompletedTask;
            lock (lockObject)
            {
                currentlyPlayingIndex--;
                if (currentlyPlayingIndex < 0)
                {
                    currentlyPlayingIndex = trackIds.Length - 1;
                }

                trackId = trackIds[currentlyPlayingIndex];
            }

            return GetTrackAndTriggerEvent(trackId);
        }

        public Task PlayTrack(int trackId)
        {
            lock (lockObject)
            {
                var index = Array.FindIndex(trackIds, t => t == trackId);
                if (index < 0)
                {
                    trackIds = new[] { trackId };
                    index = 0;
                }

                currentlyPlayingIndex = index;
            }

            return GetTrackAndTriggerEvent(trackId);
        }

        public async Task GetTrackAndTriggerEvent(int trackId)
        {
            var track = await httpClient.GetJsonAsync<Track>("track/" + trackId);
            OnPlayTrack?.Invoke(this, new PlayTrackEventArgs { Track = track, HasNextTrack = currentlyPlayingIndex < trackIds.Length - 1, HasPreviousTrack = currentlyPlayingIndex > 0 });
        }
    }
}
