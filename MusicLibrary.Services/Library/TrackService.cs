using AutoMapper;
using MusicLibrary.DataAccess.Context;
using MusicLibrary.DataAccess.QueryHelper;
using MusicLibrary.DataAccess.Repositories;
using MusicLibrary.Model;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;

namespace MusicLibrary.Services.Library
{
    public class TrackService
    {
        private readonly MusicLibraryContext context;
        private readonly IMapper mapper;

        public TrackService(MusicLibraryContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public Task ToggleLibrary(int trackId, bool isInLibrary, IPrincipal principal)
        {
            if (isInLibrary)
            {
                return context.TrackRepository.AddTrackToUserLibrary(trackId, principal.GetUserId());
            }
            else
            {
                return context.TrackRepository.RemoveTrackToUserLibrary(trackId, principal.GetUserId());
            }
        }

        public async Task<Track> GetById(int id, IPrincipal principal)
        {
            var track = await context.TrackRepository.GetById(id);

            var album = await context.AlbumRepository.GetById(track.AlbumId);

            var idAsList = new List<int> { id };
            var artists = await context.ArtistRepository.GetArtistsByTrackIdList(idAsList);
            var genres = await context.GenreRepository.GetGenresByTrackIdList(idAsList);

            var userTrackIds = new HashSet<int>(await context.TrackRepository.GetUserTracksByTrackIdList(idAsList, principal.GetUserId()));

            return new Track()
            {
                Id = track.Id,
                Album = mapper.Map<Album>(album),
                Artists = mapper.Map<List<Artist>>(artists[id]),
                Genres = mapper.Map<List<Genre>>(genres[id]),
                DiscNumber = track.DiscNumber,
                Duration = track.Duration.TotalSeconds,
                SavedToLibrary = userTrackIds.Contains(track.Id),
                Title = track.Title,
                TrackNumber = track.TrackNumber
            };
        }

        public async Task<ListResult<Track>> SearchTracks(TrackFilter filter, Page<TrackOrderColumn> page, IPrincipal principal)
        {
            var filterDo = mapper.Map<TrackFilterDo>(filter);
            var pageDo = mapper.Map<PagedQuery<TrackOrderColumnDo>>(page);

            if (filter != null && filter.PlayedByMe)
            {
                filterDo.PlayedByUserId = principal.GetUserId();
            }

            if (filter != null && filter.IsInMyLibrary)
            {
                filterDo.IsInLibraryForUserId = principal.GetUserId();
            }

            var tracks = (await context.TrackRepository.SearchTracks(filterDo, pageDo)).ToList();

            var albumIds = tracks.Select(t => t.AlbumId).Distinct().ToList();
            var trackIds = tracks.Select(t => t.Id).Distinct().ToList();

            var albums = (await context.AlbumRepository.GetByIdList(albumIds)).ToDictionary(a => a.Id);
            var artists = await context.ArtistRepository.GetArtistsByTrackIdList(trackIds);
            var genres = await context.GenreRepository.GetGenresByTrackIdList(trackIds);
            var userTrackIds = new HashSet<int>(await context.TrackRepository.GetUserTracksByTrackIdList(trackIds, principal.GetUserId()));

            var count = await context.TrackRepository.CountTracks(filterDo);

            var items = tracks
                .Select(t =>
                        new Track
                        {
                            Id = t.Id,
                            Album = mapper.Map<Album>(albums[t.AlbumId]),
                            Artists = mapper.Map<List<Artist>>(artists[t.Id]),
                            Genres = mapper.Map<List<Genre>>(genres[t.Id]),
                            DiscNumber = t.DiscNumber,
                            Duration = t.Duration.TotalSeconds,
                            SavedToLibrary = userTrackIds.Contains(t.Id),
                            Title = t.Title,
                            TrackNumber = t.TrackNumber
                        })
                .ToList();

            return new ListResult<Track>
            {
                Items = items,
                Count = count
            };
        }
    }
}
