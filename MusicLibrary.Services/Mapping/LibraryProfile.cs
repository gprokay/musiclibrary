using AutoMapper;
using MusicLibrary.DataAccess.Entities;
using MusicLibrary.DataAccess.QueryHelper;
using MusicLibrary.DataAccess.Repositories;
using MusicLibrary.Model;

namespace MusicLibrary.Services.Mapping
{
    public class LibraryProfile : Profile
    {
        public LibraryProfile()
        {
            CreateMap<AlbumDo, Album>();
            CreateMap<ArtistDo, Artist>();
            CreateMap<GenreDo, Genre>();

            CreateMap<TrackOrderColumn, TrackOrderColumnDo>();
            CreateMap<OrderDirection, OrderByDirection>();

            CreateMap<OrderModel<TrackOrderColumn>, OrderBy<TrackOrderColumnDo>>();
            CreateMap<Page<TrackOrderColumn>, PagedQuery<TrackOrderColumnDo>>();

            CreateMap<TrackFilter, TrackFilterDo>()
                .ForMember(f => f.PlayedByUserId, o => o.Ignore())
                .ForMember(f => f.IsInLibraryForUserId, o => o.Ignore());
        }
    }
}
