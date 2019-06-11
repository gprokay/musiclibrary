using GP.Tools.DriveService;
using MusicLibrary.DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Threading.Tasks;

namespace MusicLibrary.Services.Library
{
    public class MusicLibraryData
    {
        public static string MachineId = GetMachineId();

        private readonly IDriveServiceFactory _driveServiceFactory;

        private DateTimeOffset now = DateTimeOffset.UtcNow;

        private List<TrackDo> tracks;
        private List<Tuple<TrackDo, GenreDo>> trackGenres;
        private List<Tuple<TrackDo, AlbumDo>> albumTracks;
        private List<Tuple<TrackDo, ArtistDo>> trackArtists;
        private List<Tuple<AlbumDo, ArtistDo>> albumArtists;
        private List<Tuple<TrackDo, MusicFileDo>> trackFiles;
        private Dictionary<string, GenreDo> genres;
        private Dictionary<string, ArtistDo> artists;
        private Dictionary<string, AlbumDo> albumCollection;
        private Dictionary<string, AlbumDo> albumForTracks;
        private List<Tuple<TrackDo, MusicFileDo>> musicFiles;

        public IEnumerable<TrackDo> Tracks => albumTracks.Select(at => { at.Item1.AlbumId = at.Item2.Id; return at.Item1; });

        public IEnumerable<AlbumDo> Albums => albumCollection.Values;

        public IEnumerable<GenreDo> Genres => genres.Values;

        public IEnumerable<ArtistDo> Artists => artists.Values;

        public IEnumerable<TrackArtistDo> TrackArtists => trackArtists.Select(ta => new TrackArtistDo { ArtistId = ta.Item2.Id, TrackId = ta.Item1.Id });

        public IEnumerable<AlbumArtistDo> AlbumArtists => albumArtists.Select(aa => new AlbumArtistDo { AlbumId = aa.Item1.Id, ArtistId = aa.Item2.Id });

        public IEnumerable<TrackGenreDo> TrackGenres => trackGenres.Select(tg => new TrackGenreDo { GenreId = tg.Item2.Id, TrackId = tg.Item1.Id });

        public IEnumerable<MusicFileDo> MusicFiles => musicFiles.Select(mf => { mf.Item2.TrackId = mf.Item1.Id; return mf.Item2; });

        public IEnumerable<TrackFileDo> TrackFiles => trackFiles.Select(tf => new TrackFileDo { MusicFileId = tf.Item2.Id, TrackId = tf.Item1.Id });

        public MusicLibraryData(IDriveServiceFactory driveServiceFactory)
        {
            _driveServiceFactory = driveServiceFactory;
        }

        public async Task BuildDataAsync(ICollection<TrackInfoResult> trackInfoMusicFiles)
        {
            Initialize(trackInfoMusicFiles);
            await PrepareAlbums(trackInfoMusicFiles);
            CollectLibraryData(trackInfoMusicFiles);
            var variousArtists = GetOrAddArtist(new ArtistDo { Name = "Unkown Artist" });
            var unkownArtist = GetOrAddArtist(new ArtistDo { Name = "Various Artists" });
            HandleAlbumsWithoutArtist(unkownArtist, variousArtists);
        }

        private void HandleAlbumsWithoutArtist(ArtistDo unkownArtist, ArtistDo variousArtists)
        {
            var albumArtistsByAlbum = albumArtists.ToLookup(aa => aa.Item1);
            var albumTracksByAlbum = albumTracks.ToLookup(at => at.Item2);
            var trackArtistsByTrack = trackArtists.ToLookup(ta => ta.Item1);
            var albumsWithoutArtist = albumCollection.Values.Where(a => !albumArtistsByAlbum.Contains(a)).ToList();

            foreach (var album in albumsWithoutArtist)
            {
                var tracksOfAlbum = albumTracksByAlbum.Contains(album)
                    ? albumTracksByAlbum[album].Select(at => at.Item1)
                    : Enumerable.Empty<TrackDo>();

                var artistsOfAlbum = tracksOfAlbum
                    .SelectMany(t => trackArtistsByTrack.Contains(t)
                        ? trackArtistsByTrack[t].Select(ta => ta.Item2)
                        : Enumerable.Empty<ArtistDo>())
                    .Distinct()
                    .ToList();

                if (artistsOfAlbum.Count == 0)
                {
                    var albumArtist = new Tuple<AlbumDo, ArtistDo>(album, unkownArtist);
                    albumArtists.Add(albumArtist);
                }
                else if (artistsOfAlbum.Count <= 2)
                {
                    foreach (var artist in artistsOfAlbum)
                    {
                        var albumArtist = new Tuple<AlbumDo, ArtistDo>(album, artist);
                        albumArtists.Add(albumArtist);
                    }
                }
                else
                {
                    var albumArtist = new Tuple<AlbumDo, ArtistDo>(album, variousArtists);
                    albumArtists.Add(albumArtist);
                }
            }
        }

        private ArtistDo GetOrAddArtist(ArtistDo artist)
        {
            if (artists.ContainsKey(artist.Name))
            {
                artist = artists[artist.Name];
            }
            else
            {
                artists.Add(artist.Name, artist);
            }

            return artist;
        }

        private void CollectLibraryData(ICollection<TrackInfoResult> trackInfoMusicFiles)
        {
            foreach (var info in trackInfoMusicFiles)
            {
                var track = new TrackDo { Duration = info.TrackInfo.Duration, Title = info.TrackInfo.Title, DiscNumber = info.TrackInfo.DiscNumber, TrackNumber = info.TrackInfo.TrackNumber, CreatedOn = now };
                var singleTrackGenres = info.TrackInfo.Genres.Select(g => new Tuple<TrackDo, GenreDo>(track, genres[g]));
                var singleTrackArtists = info.TrackInfo.Artists.Select(a => new Tuple<TrackDo, ArtistDo>(track, artists[a]));
                var singleTrackAlbumArtists = info
                    .TrackInfo
                    .AlbumArtists
                    .Select(a => new Tuple<AlbumDo, ArtistDo>(albumForTracks[info.File.Id], artists[a]))
                    .Where(a => albumArtists.All(aa => aa.Item1.Title != a.Item1.Title && aa.Item2.Name != a.Item2.Name));
                var albumTrack = new Tuple<TrackDo, AlbumDo>(track, albumForTracks[info.File.Id]);

                tracks.Add(track);
                trackGenres.AddRange(singleTrackGenres);
                trackArtists.AddRange(singleTrackArtists);
                albumArtists.AddRange(singleTrackAlbumArtists);
                albumTracks.Add(albumTrack);
                musicFiles.Add(new Tuple<TrackDo, MusicFileDo>(track, new MusicFileDo
                {
                    DriveType = info.File.DriveType,
                    FileId = info.File.Id,
                    Length = info.File.Size,
                    Name = info.File.Name,
                    Root = info.File.Root,
                    MachineId = MachineId
                }));
            }
        }

        private async Task PrepareAlbums(ICollection<TrackInfoResult> trackInfoMusicFiles)
        {
            foreach (var info in trackInfoMusicFiles)
            {
                var albumTitle = info.TrackInfo.Album;
                var file = info.File;
                var service = _driveServiceFactory.GetService(file.DriveType, file.Root);
                var parentId = service.GetParentId(file.Id);
                var folder = await service.GetFolderWithIdAysnc(parentId);
                var type = service.GetExtension(file.Id);
                var albumKey = string.Concat(albumTitle, "-", folder.Id, "-", type, "-", MachineId);
                if (!albumCollection.ContainsKey(albumKey))
                {
                    albumCollection.Add(albumKey, new AlbumDo { Title = albumTitle, UniqueId = albumKey });
                }

                albumForTracks.Add(info.File.Id, albumCollection[albumKey]);
            }
        }

        private void Initialize(ICollection<TrackInfoResult> trackInfoMusicFiles)
        {
            tracks = new List<TrackDo>();

            trackGenres = new List<Tuple<TrackDo, GenreDo>>();
            albumTracks = new List<Tuple<TrackDo, AlbumDo>>();
            trackArtists = new List<Tuple<TrackDo, ArtistDo>>();
            albumArtists = new List<Tuple<AlbumDo, ArtistDo>>();
            trackFiles = new List<Tuple<TrackDo, MusicFileDo>>();
            musicFiles = new List<Tuple<TrackDo, MusicFileDo>>();

            genres = trackInfoMusicFiles.SelectMany(i => i.TrackInfo.Genres)
                .Distinct()
                .Select(g => new GenreDo { Name = g })
                .ToDictionary(g => g.Name);
            artists = trackInfoMusicFiles.SelectMany(i => i.TrackInfo.Artists)
                .Union(trackInfoMusicFiles.SelectMany(i => i.TrackInfo.AlbumArtists))
                .Distinct()
                .Select(a => new ArtistDo { Name = a })
                .ToDictionary(a => a.Name);

            albumCollection = new Dictionary<string, AlbumDo>();
            albumForTracks = new Dictionary<string, AlbumDo>();
        }

        private static string GetMachineId()
        {
            string cpuInfo = string.Empty;
            ManagementClass mc = new ManagementClass("win32_processor");
            ManagementObjectCollection moc = mc.GetInstances();

            foreach (ManagementObject mo in moc)
            {
                if (cpuInfo == "")
                {
                    //Get only the first CPU's ID
                    cpuInfo = mo.Properties["processorID"].Value.ToString();
                    break;
                }
            }
            return cpuInfo;
        }
    }
}
