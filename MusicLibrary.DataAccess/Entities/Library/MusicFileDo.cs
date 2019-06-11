using MusicLibrary.DataAccess.QueryHelper;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MusicLibrary.DataAccess.Entities
{
    [Table("MusicFile", Schema = "Library")]
    public class MusicFileDo
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }
        [Unique]
        public string FileId { get; set; }
        public string DriveType { get; set; }
        public string Name { get; set; }
        public string Root { get; set; }
        public long Length { get; set; }
        public int TrackId { get; set; }
        [Unique]
        public string MachineId { get; set; }
    }
}
