using MusicLibrary.DataAccess.Connection;
using MusicLibrary.DataAccess.Repositories;
using System.Configuration;

namespace MusicLibrary.DataAccess.Context
{
    public class MusicLibraryContext
    {
        public UserRepository UserRepository { get; }
        public AlbumRepository AlbumRepository { get; }
        public GenreRepository GenreRepository { get; }
        public ArtistRepository ArtistRepository { get; }
        public TrackRepository TrackRepository { get; }
        public TrackGenreRepository TrackGenreRepository { get; }
        public TrackArtistRepository TrackArtistRepository { get; }
        public AlbumArtistRepository AlbumArtistRepository { get; }
        public TrackFileRepostiory TrackFileRepostiory { get; }
        public MusicFileRepository MusicFileRepository { get; }

        public MusicLibraryContext(string connectionString)
        {
            var connectionManager = new ConnectionManager(new ConnectionStringSettings("connection", connectionString, "System.Data.SqlClient"));
            UserRepository = new UserRepository(connectionManager);
            AlbumRepository = new AlbumRepository(connectionManager);
            GenreRepository = new GenreRepository(connectionManager);
            ArtistRepository = new ArtistRepository(connectionManager);
            TrackRepository = new TrackRepository(connectionManager);
            TrackGenreRepository = new TrackGenreRepository(connectionManager);
            TrackArtistRepository = new TrackArtistRepository(connectionManager);
            AlbumArtistRepository = new AlbumArtistRepository(connectionManager);
            TrackFileRepostiory = new TrackFileRepostiory(connectionManager);
            MusicFileRepository = new MusicFileRepository(connectionManager);
        }
    }
}
