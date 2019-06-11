using MusicLibrary.DataAccess.QueryHelper;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MusicLibrary.DataAccess.Entities
{
    [Table("Artist", Schema = "Library")]
    public class ArtistDo
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }
        [Unique]
        public string Name { get; set; }
    }
}
