using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MusicLibrary.DataAccess.Entities
{
    [Table("AlbumArtist", Schema = "Library")]
    public class AlbumArtistDo
    {
        [Key]
        public int AlbumId { get; set; }
        [Key]
        public int ArtistId { get; set; }
    }
}
