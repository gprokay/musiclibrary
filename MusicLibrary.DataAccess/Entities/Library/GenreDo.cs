using MusicLibrary.DataAccess.QueryHelper;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MusicLibrary.DataAccess.Entities
{
    [Table("Genre", Schema = "Library")]
    public class GenreDo
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }
        [Unique]
        public string Name { get; set; }
    }
}
