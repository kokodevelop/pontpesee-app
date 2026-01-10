using System;
using PontPesee.API.DTOs;

namespace PontPesee.API.Models
{
    public enum ReportJobStatus
    {
        Pending,
        Processing,
        Completed,
        Failed
    }

    public class ReportJob
    {
        public string Id { get; set; } = string.Empty;
        public PeseeFilterDto? Filter { get; set; }
        public ReportJobStatus Status { get; set; } = ReportJobStatus.Pending;
        public string? FilePath { get; set; }
        public string? Error { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? CompletedAt { get; set; }
    }
}
