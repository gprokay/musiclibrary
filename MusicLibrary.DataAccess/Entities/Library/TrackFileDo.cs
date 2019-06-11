using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MusicLibrary.DataAccess.Entities
{
    [Table("TrackFile", Schema = "Library")]
    public class TrackFileDo
    {
        [Key]
        public int TrackId { get; set; }

        [Key]
        public int MusicFileId { get; set; }
    }
}
