using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PontPesee.API.Models
{
    [Table("report_jobs")]
    public class ReportJobEntity
    {
        [Key]
        [MaxLength(36)]
        public string Id { get; set; } = string.Empty;

        [Column(TypeName = "longtext")]
        public string? FilterJson { get; set; }

        [MaxLength(50)]
        public string? Status { get; set; }

        [MaxLength(500)]
        public string? FilePath { get; set; }

        public string? Error { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? CompletedAt { get; set; }

        [MaxLength(100)]
        public string? OwnerUserId { get; set; }
    }
}
