using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MusicLibrary.DataAccess.Entities
{
    [Table("TrackArtist", Schema = "Library")]
    public class TrackArtistDo
    {
        [Key]
        public int TrackId { get; set; }
        [Key]
        public int ArtistId { get; set; }
    }
}
