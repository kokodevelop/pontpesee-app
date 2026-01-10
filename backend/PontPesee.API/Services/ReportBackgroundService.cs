using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PontPesee.API.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace PontPesee.API.Services
{
    public class ReportBackgroundService : BackgroundService
    {
        private readonly IReportQueue _queue;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<ReportBackgroundService> _logger;

        public ReportBackgroundService(IReportQueue queue, IServiceProvider serviceProvider, ILogger<ReportBackgroundService> logger)
        {
            _queue = queue;
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("ReportBackgroundService started");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    if (_queue.TryDequeue(out var job) && job != null)
                    {
                        job.Status = ReportJobStatus.Processing;
                        _logger.LogInformation("Processing report job {JobId}", job.Id);

                        try
                        {
                            using var scope = _serviceProvider.CreateScope();
                            var peseeService = scope.ServiceProvider.GetRequiredService(typeof(IPeseeService)) as IPeseeService;
                            if (peseeService == null)
                                throw new InvalidOperationException("IPeseeService not available in scope");

                            // Fetch data from DB using the filter saved in the job
                            var stats = await peseeService.GetPeseesAsync(job.Filter!);

                            // Build PDF document (similar to controller logic)
                            var doc = Document.Create(container =>
                            {
                                container.Page(page =>
                                {
                                    page.Size(PageSizes.A4);
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

                            var temp = Path.GetTempPath();
                            var fileName = $"statistiques_{job.Id}.pdf";
                            var filePath = Path.Combine(temp, fileName);

                            using (var ms = new MemoryStream())
                            {
                                doc.GeneratePdf(ms);
                                await File.WriteAllBytesAsync(filePath, ms.ToArray(), stoppingToken);
                            }

                            _queue.MarkCompleted(job.Id, filePath);
                            _logger.LogInformation("Report job {JobId} completed, file={FilePath}", job.Id, filePath);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Failed to process report job {JobId}", job.Id);
                            _queue.MarkFailed(job.Id, ex.ToString());
                        }
                    }
                    else
                    {
                        await Task.Delay(1000, stoppingToken);
                    }
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                    // shutting down
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unexpected error in ReportBackgroundService loop");
                    await Task.Delay(1000, stoppingToken);
                }
            }

            _logger.LogInformation("ReportBackgroundService stopping");
        }
    }
}
