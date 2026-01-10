using System;
using System.IO;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using PontPesee.API.DTOs;
using PontPesee.API.Reports;
using PontPesee.API.Services;
using PontPesee.API.Models;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace PontPesee.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportsController : ControllerBase
    {
        private readonly IPeseeService _peseeService;
        private readonly IReportQueue _reportQueue;

        public ReportsController(IPeseeService peseeService, IReportQueue reportQueue)
        {
            _peseeService = peseeService;
            _reportQueue = reportQueue;
        }

        [HttpGet("testpdf")]
        public IActionResult TestPdf()
        {
            try
            {
                var doc = QuestPDF.Fluent.Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Size(QuestPDF.Helpers.PageSizes.A4);
                        page.Margin(50);
                        page.Content().AlignCenter().AlignMiddle().Column(col =>
                        {
                            col.Item().Text("Test PDF").FontSize(24).Bold();
                            col.Item().Text("QuestPDF is working").FontSize(12);
                        });
                    });
                });

                using var ms = new MemoryStream();
                doc.GeneratePdf(ms);
                var bytes = ms.ToArray();
                return File(bytes, "application/pdf", "test.pdf");
            }
            catch (System.Exception ex)
            {
                Console.Error.WriteLine(ex.ToString());
                return StatusCode(500, new { error = "Test PDF generation failed", details = ex.ToString() });
            }
        }

        [HttpPost("statistics/excel")]
        public async Task<IActionResult> ExportStatisticsExcel([FromBody] PeseeFilterDto filter)
        {
            // Cap page size to avoid extremely large queries (protect DB)
            const int MaxPageSize = 10000;
            filter.Page = 0;
            if (filter.PageSize <= 0 || filter.PageSize > MaxPageSize)
                filter.PageSize = MaxPageSize;

            var stats = await _peseeService.GetPeseesAsync(filter);

            using var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Statistiques");

            // Header
            var headers = new[] { "Ticket", "Site", "Date", "Immatriculation", "Fournisseur", "Client", "Produit", "Mouvement", "Poids Net (kg)", "Annulé" };
            for (int i = 0; i < headers.Length; i++)
            {
                ws.Cell(1, i + 1).Value = headers[i];
                ws.Cell(1, i + 1).Style.Font.Bold = true;
            }

            // Rows
            var row = 2;
            foreach (var p in stats.Pesees)
            {
                ws.Cell(row, 1).Value = p.NumTicket;
                ws.Cell(row, 2).Value = p.CodeSite;
                ws.Cell(row, 3).Value = p.Dmv?.ToString("yyyy-MM-dd HH:mm:ss");
                ws.Cell(row, 4).Value = p.Immatriculation;
                ws.Cell(row, 5).Value = p.IdFournisseur;
                ws.Cell(row, 6).Value = p.NomClient;
                ws.Cell(row, 7).Value = p.Label;
                ws.Cell(row, 8).Value = p.Mouvement;
                ws.Cell(row, 9).Value = p.PoidsNet;
                ws.Cell(row, 10).Value = p.Annuler ? "Oui" : "Non";
                row++;
            }

            ws.Columns().AdjustToContents();

            using var ms = new MemoryStream();
            wb.SaveAs(ms);
            ms.Position = 0;

            var fileName = $"statistiques_{DateTime.UtcNow:yyyyMMdd_HHmmss}.xlsx";
            return File(ms.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

        [HttpPost("statistics/pdf")]
        public async Task<IActionResult> ExportStatisticsPdf([FromBody] PeseeFilterDto filter)
        {
            // Cap page size to avoid extremely large queries (protect DB)
            const int MaxPageSize = 10000;
            filter.Page = 0;
            if (filter.PageSize <= 0 || filter.PageSize > MaxPageSize)
                filter.PageSize = MaxPageSize;

            var stats = await _peseeService.GetPeseesAsync(filter);

            try
            {
                var doc = QuestPDF.Fluent.Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Size(QuestPDF.Helpers.PageSizes.A4);
                        page.Margin(20);

                        page.Header().Height(60).AlignCenter().Row(row =>
                        {
                            row.RelativeItem().Column(column =>
                            {
                                column.Item().Text("Pont Pesée").FontSize(20).Bold();
                                column.Item().Text("Rapport de Statistiques").FontSize(12).SemiBold();
                            });
                        });

                        page.Content().PaddingVertical(10).Column(col =>
                        {
                            col.Item().Row(r =>
                            {
                                r.RelativeItem().Column(column =>
                                {
                                    column.Item().Text($"Nombre de pesées: {stats.NombrePesees}").Bold();
                                    column.Item().Text($"Poids brut total: {stats.PoidsBrutTotal:N2} kg");
                                    column.Item().Text($"Poids net total: {stats.PoidsNetTotal:N2} kg");
                                });
                            });

                            // Simple rows
                            col.Item().PaddingTop(8).Column(c =>
                            {
                                // header
                                c.Item().Row(rh =>
                                {
                                    rh.RelativeItem().Text("Ticket").Bold();
                                    rh.RelativeItem().Text("Date").Bold();
                                    rh.RelativeItem().Text("Site").Bold();
                                    rh.RelativeItem().Text("Immatriculation").Bold();
                                    rh.RelativeItem().Text("Fournisseur").Bold();
                                    rh.RelativeItem().Text("Client").Bold();
                                    rh.RelativeItem().Text("Produit").Bold();
                                    rh.RelativeItem().Text("Mouvement").Bold();
                                    rh.RelativeItem().AlignRight().Text("PoidsNet (kg)").Bold();
                                });

                                foreach (var p in stats.Pesees)
                                {
                                    c.Item().Row(r =>
                                    {
                                        r.RelativeItem().Text(p.NumTicket);
                                        r.RelativeItem().Text(p.Dmv?.ToString("yyyy-MM-dd"));
                                        r.RelativeItem().Text(p.CodeSite);
                                        r.RelativeItem().Text(p.Immatriculation);
                                        r.RelativeItem().Text(p.IdFournisseur);
                                        r.RelativeItem().Text(p.NomClient);
                                        r.RelativeItem().Text(p.Label);
                                        r.RelativeItem().Text(p.Mouvement);
                                        r.RelativeItem().AlignRight().Text(p.PoidsNet?.ToString("N2") ?? "");
                                    });
                                }
                            });
                        });

                        page.Footer().AlignCenter().Text($"Généré le {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}").FontSize(9);
                    });
                });

                using var ms = new MemoryStream();
                doc.GeneratePdf(ms);
                var pdfBytes = ms.ToArray();
                var fileName = $"statistiques_{DateTime.UtcNow:yyyyMMdd_HHmmss}.pdf";
                return File(pdfBytes, "application/pdf", fileName);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.ToString());
                return StatusCode(500, new { error = "PDF generation failed", details = ex.ToString() });
            }
        }

        // Test-only endpoint: accept pre-built stats DTO and generate PDF without DB access
        [HttpPost("statistics/pdf/test")]
        public IActionResult ExportStatisticsPdfFromDto([FromBody] PeseeStatsDto stats)
        {
            try
            {
                var doc = QuestPDF.Fluent.Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Size(QuestPDF.Helpers.PageSizes.A4);
                        page.Margin(20);

                        page.Header().Height(60).AlignCenter().Row(row =>
                        {
                            row.RelativeItem().Column(column =>
                            {
                                column.Item().Text("Pont Pesée").FontSize(20).Bold();
                                column.Item().Text("Rapport de Statistiques").FontSize(12).SemiBold();
                            });
                        });

                        page.Content().PaddingVertical(10).Column(col =>
                        {
                            col.Item().Row(r =>
                            {
                                r.RelativeItem().Column(column =>
                                {
                                    column.Item().Text($"Nombre de pesées: {stats.NombrePesees}").Bold();
                                    column.Item().Text($"Poids brut total: {stats.PoidsBrutTotal:N2} kg");
                                    column.Item().Text($"Poids net total: {stats.PoidsNetTotal:N2} kg");
                                });
                            });

                            col.Item().PaddingTop(8).Column(c =>
                            {
                                c.Item().Row(rh =>
                                {
                                    rh.RelativeItem().Text("Ticket").Bold();
                                    rh.RelativeItem().Text("Date").Bold();
                                    rh.RelativeItem().Text("Site").Bold();
                                    rh.RelativeItem().Text("Immatriculation").Bold();
                                    rh.RelativeItem().Text("Fournisseur").Bold();
                                    rh.RelativeItem().Text("Client").Bold();
                                    rh.RelativeItem().Text("Produit").Bold();
                                    rh.RelativeItem().Text("Mouvement").Bold();
                                    rh.RelativeItem().AlignRight().Text("PoidsNet (kg)").Bold();
                                });

                                foreach (var p in stats.Pesees)
                                {
                                    c.Item().Row(r =>
                                    {
                                        r.RelativeItem().Text(p.NumTicket);
                                        r.RelativeItem().Text(p.Dmv?.ToString("yyyy-MM-dd"));
                                        r.RelativeItem().Text(p.CodeSite);
                                        r.RelativeItem().Text(p.Immatriculation);
                                        r.RelativeItem().Text(p.IdFournisseur);
                                        r.RelativeItem().Text(p.NomClient);
                                        r.RelativeItem().Text(p.Label);
                                        r.RelativeItem().Text(p.Mouvement);
                                        r.RelativeItem().AlignRight().Text(p.PoidsNet?.ToString("N2") ?? "");
                                    });
                                }
                            });
                        });

                        page.Footer().AlignCenter().Text($"Généré le {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}").FontSize(9);
                    });
                });

                using var ms = new MemoryStream();
                doc.GeneratePdf(ms);
                var pdfBytes = ms.ToArray();
                var fileName = $"statistiques_test_{DateTime.UtcNow:yyyyMMdd_HHmmss}.pdf";
                return File(pdfBytes, "application/pdf", fileName);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.ToString());
                return StatusCode(500, new { error = "PDF generation failed", details = ex.ToString() });
            }
        }

        [HttpPost("statistics/pdf/queue")]
        public IActionResult EnqueueStatisticsPdf([FromBody] PeseeFilterDto filter)
        {
            // Enqueue job and return 202 with job id and URLs
            var jobId = _reportQueue.Enqueue(filter);
            var statusUrl = Url.Action(nameof(GetQueuedJobStatus), new { id = jobId });
            var downloadUrl = Url.Action(nameof(DownloadQueuedPdf), new { id = jobId });

            return Accepted(new { jobId, statusUrl, downloadUrl });
        }

        [HttpGet("statistics/pdf/queue/{id}/status")]
        public IActionResult GetQueuedJobStatus(string id)
        {
            if (!_reportQueue.TryGetJob(id, out var job) || job == null)
                return NotFound(new { error = "Job not found" });

            return Ok(new
            {
                job.Id,
                status = job.Status.ToString(),
                file = job.FilePath,
                error = job.Error
            });
        }

        [HttpGet("statistics/pdf/queue/{id}/download")]
        public IActionResult DownloadQueuedPdf(string id)
        {
            if (!_reportQueue.TryGetJob(id, out var job) || job == null)
                return NotFound(new { error = "Job not found" });

            if (job.Status != ReportJobStatus.Completed)
                return StatusCode(202, new { status = job.Status.ToString() });

            if (string.IsNullOrEmpty(job.FilePath) || !System.IO.File.Exists(job.FilePath))
                return NotFound(new { error = "File not available" });

            var bytes = System.IO.File.ReadAllBytes(job.FilePath!);
            var fileName = System.IO.Path.GetFileName(job.FilePath!);
            return File(bytes, "application/pdf", fileName);
        }
    }
}
