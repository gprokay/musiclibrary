using MusicLibrary.DataAccess.QueryHelper;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MusicLibrary.DataAccess.Entities
{
    [Table("Album", Schema = "Library")]
    public class AlbumDo
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }
        public string Title { get; set; }
        [Unique]
        public string UniqueId { get; set; }
    }
}
