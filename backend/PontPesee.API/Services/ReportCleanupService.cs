using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace PontPesee.API.Services
{
    public class ReportCleanupService : BackgroundService
    {
        private readonly ILogger<ReportCleanupService> _logger;
        private readonly IServiceProvider _services;
        private readonly IConfiguration _configuration;

        public ReportCleanupService(ILogger<ReportCleanupService> logger, IServiceProvider services, IConfiguration configuration)
        {
            _logger = logger;
            _services = services;
            _configuration = configuration;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var retentionDays = _configuration.GetValue<int?>("Reports:RetentionDays") ?? 7;
            var intervalMinutes = _configuration.GetValue<int?>("Reports:CleanupIntervalMinutes") ?? 60;
            var generatedRelative = _configuration.GetValue<string?>("Reports:GeneratedFilesPath") ?? "generated";

            _logger.LogInformation("ReportCleanupService starting. RetentionDays={RetentionDays}, IntervalMinutes={IntervalMinutes}", retentionDays, intervalMinutes);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    // Clean temp folder files created by background worker (prefix statistiques_ or statistiques_{jobId})
                    CleanDirectory(Path.GetTempPath(), retentionDays, "statistiques_");

                    // Clean project generated folder if exists
                    var contentRoot = AppContext.BaseDirectory; // bin folder; move up to project root
                    var projectRoot = Directory.GetParent(contentRoot)!.Parent!.Parent!.FullName;
                    var generatedDir = Path.Combine(projectRoot, generatedRelative);
                    if (Directory.Exists(generatedDir))
                    {
                        CleanDirectory(generatedDir, retentionDays, "statistiques_");
                    }

                    _logger.LogInformation("ReportCleanupService completed sweep");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error during report cleanup");
                }

                await Task.Delay(TimeSpan.FromMinutes(intervalMinutes), stoppingToken);
            }
        }

        private void CleanDirectory(string dir, int retentionDays, string prefix)
        {
            try
            {
                var cutoff = DateTime.UtcNow.AddDays(-retentionDays);
                var di = new DirectoryInfo(dir);
                var files = di.GetFiles().Where(f => f.Name.StartsWith(prefix, StringComparison.OrdinalIgnoreCase) && f.LastWriteTimeUtc < cutoff).ToList();
                foreach (var f in files)
                {
                    try
                    {
                        f.Delete();
                        _logger.LogInformation("Deleted old report file: {File}", f.FullName);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Failed to delete file {File}", f.FullName);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to clean directory {Dir}", dir);
            }
        }
    }
}
