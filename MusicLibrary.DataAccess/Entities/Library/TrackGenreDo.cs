using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MusicLibrary.DataAccess.Entities
{
    [Table("TrackGenre", Schema = "Library")]
    public class TrackGenreDo
    {
        [Key]
        public int TrackId { get; set; }
        [Key]
        public int GenreId { get; set; }
    }
}
