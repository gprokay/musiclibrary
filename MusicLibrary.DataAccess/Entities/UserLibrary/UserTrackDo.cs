using MusicLibrary.DataAccess.QueryHelper;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MusicLibrary.DataAccess.Entities
{
    [Table("UserTrack", Schema = "UserLibrary")]
    public class UserTrackDo
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }

        [Unique]
        public int TrackId { get; set; }

        [Unique]
        public int UserId { get; set; }
    }
}
