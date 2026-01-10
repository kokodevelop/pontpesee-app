using System;
using System.Collections.Concurrent;
using PontPesee.API.Models;
using PontPesee.API.DTOs;
using PontPesee.API.Data;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;

namespace PontPesee.API.Services
{
    public interface IReportQueue
    {
        string Enqueue(PeseeFilterDto filter);
        bool TryDequeue(out ReportJob? job);
        bool TryGetJob(string id, out ReportJob? job);
        void MarkCompleted(string id, string filePath);
        void MarkFailed(string id, string error);
    }

    public class ReportQueue : IReportQueue
    {
        private readonly ConcurrentQueue<string> _queue = new ConcurrentQueue<string>();
        private readonly ConcurrentDictionary<string, ReportJob> _jobs = new ConcurrentDictionary<string, ReportJob>();
        private readonly IServiceProvider _serviceProvider;

        public ReportQueue(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public string Enqueue(PeseeFilterDto filter)
        {
            var id = Guid.NewGuid().ToString("N");
            var job = new ReportJob
            {
                Id = id,
                Filter = filter,
                Status = ReportJobStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };

            // persist job entity
            using (var scope = _serviceProvider.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var entity = new ReportJobEntity
                {
                    Id = id,
                    FilterJson = JsonSerializer.Serialize(filter),
                    Status = "PENDING",
                    CreatedAt = job.CreatedAt,
                    FilePath = null,
                    Error = null,
                    OwnerUserId = null
                };
                db.ReportJobs.Add(entity);
                db.SaveChanges();
            }

            _jobs[id] = job;
            _queue.Enqueue(id);
            return id;
        }

        public bool TryDequeue(out ReportJob? job)
        {
            job = null;
            if (!_queue.TryDequeue(out var id)) return false;
            if (_jobs.TryGetValue(id, out var j))
            {
                job = j;
                // mark DB entity as processing
                using (var scope = _serviceProvider.CreateScope())
                {
                    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    var entity = db.ReportJobs.Find(id);
                    if (entity != null)
                    {
                        entity.Status = "PROCESSING";
                        db.SaveChanges();
                    }
                }
                job.Status = ReportJobStatus.Processing;
                return true;
            }
            return false;
        }

        public bool TryGetJob(string id, out ReportJob? job)
        {
            return _jobs.TryGetValue(id, out job);
        }

        public void MarkCompleted(string id, string filePath)
        {
            if (_jobs.TryGetValue(id, out var job))
            {
                job.FilePath = filePath;
                job.Status = ReportJobStatus.Completed;
                job.CompletedAt = DateTime.UtcNow;
                // update DB entity
                using (var scope = _serviceProvider.CreateScope())
                {
                    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    var entity = db.ReportJobs.Find(id);
                    if (entity != null)
                    {
                        entity.FilePath = filePath;
                        entity.Status = "COMPLETED";
                        entity.CompletedAt = job.CompletedAt;
                        db.SaveChanges();
                    }
                }
            }
        }

        public void MarkFailed(string id, string error)
        {
            if (_jobs.TryGetValue(id, out var job))
            {
                job.Error = error;
                job.Status = ReportJobStatus.Failed;
                job.CompletedAt = DateTime.UtcNow;
                using (var scope = _serviceProvider.CreateScope())
                {
                    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    var entity = db.ReportJobs.Find(id);
                    if (entity != null)
                    {
                        entity.Error = error;
                        entity.Status = "FAILED";
                        entity.CompletedAt = job.CompletedAt;
                        db.SaveChanges();
                    }
                }
            }
        }
    }
}
