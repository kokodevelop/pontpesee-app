using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PontPesee.API.Data;
using Microsoft.EntityFrameworkCore;

namespace PontPesee.API.Controllers
{
    [ApiController]
    [Route("api/reports/debug")]
    public class DebugReportsController : ControllerBase
    {
        private readonly ApplicationDbContext _db;

        public DebugReportsController(ApplicationDbContext db)
        {
            _db = db;
        }

        [HttpGet("report-jobs/count")]
        public async Task<IActionResult> GetReportJobsCount()
        {
            var count = await _db.ReportJobs.CountAsync();
            return Ok(new { count });
        }

        [HttpGet("report-jobs/{id}")]
        public async Task<IActionResult> GetReportJob(string id)
        {
            var job = await _db.ReportJobs.FindAsync(id);
            if (job == null) return NotFound();
            return Ok(job);
        }

        [HttpPost("report-jobs/insert-test")]
        public async Task<IActionResult> InsertTestJob()
        {
            var id = Guid.NewGuid().ToString("N");
                        // ensure table exists (debug helper - uses MySQL DDL)
                        var createSql = @"CREATE TABLE IF NOT EXISTS `report_jobs` (
    `Id` varchar(36) NOT NULL,
    `FilterJson` longtext,
    `Status` varchar(50),
    `FilePath` varchar(500),
    `Error` longtext,
    `CreatedAt` datetime(6) NOT NULL,
    `CompletedAt` datetime(6),
    `OwnerUserId` varchar(100),
    PRIMARY KEY (`Id`)
) ENGINE=InnoDB;";
                        await _db.Database.ExecuteSqlRawAsync(createSql);
            var job = new Models.ReportJobEntity
            {
                Id = id,
                FilterJson = "{ \"tableName\": \"pesee\", \"test\": true }",
                Status = "PENDING",
                CreatedAt = DateTime.UtcNow
            };

            try
            {
                _db.ReportJobs.Add(job);
                await _db.SaveChangesAsync();
                return Ok(new { jobId = id });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message, detail = ex.ToString() });
            }
        }
    }
}
