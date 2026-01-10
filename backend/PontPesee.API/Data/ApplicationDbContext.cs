using Microsoft.EntityFrameworkCore;
using PontPesee.API.Models;

namespace PontPesee.API.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Tables principales
        public DbSet<Pesee> Pesees { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<ReportJobEntity> ReportJobs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuration pour la table pesee
            modelBuilder.Entity<Pesee>(entity =>
            {
                entity.ToTable("pesee");
                entity.HasKey(e => e.CodePesee);

                // Index pour améliorer les performances
                entity.HasIndex(e => e.NumTicket);
                entity.HasIndex(e => e.Imprime);
                entity.HasIndex(e => e.Dmv);
                entity.HasIndex(e => e.CodeSite);
            });

            // Configuration pour la table user
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("user");
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.NomUtilisateur).IsUnique();
            });

            // report jobs
            modelBuilder.Entity<ReportJobEntity>(entity =>
            {
                entity.ToTable("report_jobs");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.FilterJson).HasColumnType("longtext").IsRequired(false);
                entity.HasIndex(e => e.Status);
                entity.HasIndex(e => e.CreatedAt);
            });
        }

        // Méthode pour changer dynamiquement la table (pesee ou peseep2)
        public IQueryable<Pesee> GetPeseesByTable(string tableName)
        {
            if (tableName.ToLower() == "peseep2")
            {
                // Créer une requête sur peseep2
                return Set<Pesee>().FromSqlRaw($"SELECT * FROM peseep2");
            }
            else
            {
                // Par défaut pesee
                return Pesees;
            }
        }
    }
}
