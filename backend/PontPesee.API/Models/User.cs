using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PontPesee.API.Models
{
    [Table("user")]
    public class User
    {
        [Key]
        [Column("NUt")]
        public int Id { get; set; }

        [Required]
        [Column("NomUt")]
        [StringLength(100)]
        public string NomUtilisateur { get; set; } = string.Empty;

        [Required]
        [Column("motpasse")]
        [StringLength(255)]
        public string MotPasse { get; set; } = string.Empty;

        [Column("LeGroupe")]
        [StringLength(50)]
        public string? Groupe { get; set; }

        [Column("Actif")]
        public bool? Actif { get; set; }
    }
}
