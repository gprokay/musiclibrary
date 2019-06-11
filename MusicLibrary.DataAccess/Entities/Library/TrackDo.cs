using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MusicLibrary.DataAccess.Entities
{
    [Table("Track", Schema = "Library")]
    public class TrackDo
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }
        public int AlbumId { get; set; }
        public string Title { get; set; }
        public int DiscNumber { get; set; }
        public int TrackNumber { get; set; }
        public TimeSpan Duration { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
    }
}
