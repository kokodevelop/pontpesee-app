using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PontPesee.API.Models
{
    [Table("pesee")]
    public class Pesee
    {
        [Key]
        [Column("codepesee")]
        public int CodePesee { get; set; }

        [Column("CodeSite")]
        [StringLength(100)]
        public string? CodeSite { get; set; }

        [Column("Campagne")]
        [StringLength(50)]
        public string? Campagne { get; set; }

        [Column("Immatriculation")]
        [StringLength(50)]
        public string? Immatriculation { get; set; }

        [Column("Poids1")]
        public double? Poids1 { get; set; }

        [Column("DateP1")]
        public DateTime? DateP1 { get; set; }

        [Column("DateP2")]
        public DateTime? DateP2 { get; set; }

        [Column("Poids2")]
        public double? Poids2 { get; set; }

        [Column("PoidsNet")]
        public double? PoidsNet { get; set; }

        [Column("CodeFournisseur")]
        [StringLength(50)]
        public string? CodeFournisseur { get; set; }

        [Column("IdFournisseur")]
        [StringLength(200)]
        public string? IdFournisseur { get; set; }

        [Column("CodeClient")]
        [StringLength(50)]
        public string? CodeClient { get; set; }

        [Column("nomclient")]
        [StringLength(200)]
        public string? NomClient { get; set; }

        [Column("Mouvement")]
        [StringLength(50)]
        public string? Mouvement { get; set; }

        [Column("Provenance")]
        [StringLength(200)]
        public string? Provenance { get; set; }

        [Column("Destination")]
        [StringLength(200)]
        public string? Destination { get; set; }

        [Column("imprime")]
        public bool Imprime { get; set; }

        [Column("NumTicket")]
        [StringLength(50)]
        public string? NumTicket { get; set; }

        [Column("Exportateur")]
        [StringLength(200)]
        public string? Exportateur { get; set; }

        [Column("Transporteur")]
        [StringLength(200)]
        public string? Transporteur { get; set; }

        [Column("Chauffeur")]
        [StringLength(200)]
        public string? Chauffeur { get; set; }

        [Column("Barcode")]
        [StringLength(100)]
        public string? Barcode { get; set; }

        [Column("heureP1")]
        public DateTime? HeureP1 { get; set; }

        [Column("heureP2")]
        public DateTime? HeureP2 { get; set; }

        [Column("Peseur")]
        [StringLength(100)]
        public string? Peseur { get; set; }

        [Column("Peseur2")]
        [StringLength(100)]
        public string? Peseur2 { get; set; }

        [Column("Annuler")]
        public bool Annuler { get; set; }

        [Column("Label")]
        [StringLength(200)]
        public string? Label { get; set; }

        [Column("Ref_Piece")]
        [StringLength(100)]
        public string? RefPiece { get; set; }

        [Column("cnsment")]
        [StringLength(500)]
        public string? Consignement { get; set; }

        [Column("IDEmballage")]
        [StringLength(50)]
        public string? IdEmballage { get; set; }

        [Column("PoidsEmb")]
        public double? PoidsEmb { get; set; }

        [Column("Refac2")]
        public double? Refac2 { get; set; }

        [Column("Emballage")]
        [StringLength(100)]
        public string? Emballage { get; set; }

        [Column("dmv")]
        public DateTime? Dmv { get; set; }

        [Column("ImageVeh")]
        public byte[]? ImageVeh { get; set; }

        [Column("remorque")]
        [StringLength(50)]
        public string? Remorque { get; set; }
    }
}
