using Microsoft.EntityFrameworkCore;
using PontPesee.API.Data;
using PontPesee.API.DTOs;
using PontPesee.API.Models;

namespace PontPesee.API.Services
{
    public class StatisticsService : IStatisticsService
    {
        private readonly ApplicationDbContext _context;

        public StatisticsService(ApplicationDbContext context)
        {
            _context = context;
        }

        private IQueryable<Pesee> GetBaseQuery(StatisticsFilterDto filter)
        {
            try
            {
                _context.Database.SetCommandTimeout(180);
            }
            catch { }

            var query = filter.TableName?.ToLower() == "peseep2"
                ? _context.Set<Pesee>().FromSqlRaw("SELECT * FROM peseep2")
                : _context.Pesees.AsQueryable();

            query = query.Where(p => p.Imprime && p.CodePesee != 0 && p.Poids2 != 0);

            if (filter.DateDebut.HasValue)
                query = query.Where(p => p.Dmv >= filter.DateDebut.Value);

            if (filter.DateFin.HasValue)
                query = query.Where(p => p.Dmv <= filter.DateFin.Value.AddDays(1).AddSeconds(-1));

            if (!string.IsNullOrEmpty(filter.Mouvement))
                query = query.Where(p => p.Mouvement == filter.Mouvement);

            if (!string.IsNullOrEmpty(filter.Fournisseur))
                query = query.Where(p => p.IdFournisseur == filter.Fournisseur);

            if (!string.IsNullOrEmpty(filter.Client))
                query = query.Where(p => p.NomClient == filter.Client);

            if (!string.IsNullOrEmpty(filter.Produit))
                query = query.Where(p => p.Label == filter.Produit);

            if (!string.IsNullOrEmpty(filter.CodeSite))
                query = query.Where(p => p.CodeSite == filter.CodeSite);

            return query;
        }

        public async Task<AdvancedStatisticsDto> GetAdvancedStatisticsAsync(StatisticsFilterDto filter)
        {
            var query = GetBaseQuery(filter);
            var data = await query.ToListAsync();

            var result = new AdvancedStatisticsDto();

            // Summary
            result.Summary = new StatisticsSummaryDto
            {
                TotalPesees = data.Count,
                TotalPoidsBrut = data.Sum(p => p.Poids1 ?? 0),
                TotalPoidsNet = data.Sum(p => p.PoidsNet ?? 0),
                PoidsMoyen = data.Count > 0 ? data.Average(p => p.PoidsNet ?? 0) : 0,
                NombreFournisseurs = data.Select(p => p.IdFournisseur).Where(f => !string.IsNullOrEmpty(f)).Distinct().Count(),
                NombreClients = data.Select(p => p.NomClient).Where(c => !string.IsNullOrEmpty(c)).Distinct().Count(),
                NombreProduits = data.Select(p => p.Label).Where(l => !string.IsNullOrEmpty(l)).Distinct().Count(),
                NombreVehicules = data.Select(p => p.Immatriculation).Where(i => !string.IsNullOrEmpty(i)).Distinct().Count(),
                PremierePesee = data.Count > 0 ? data.Min(p => p.Dmv) : null,
                DernierePesee = data.Count > 0 ? data.Max(p => p.Dmv) : null
            };

            // Par Fournisseur (Top N)
            result.ParFournisseur = data
                .Where(p => !string.IsNullOrEmpty(p.IdFournisseur))
                .GroupBy(p => new { CodeFournisseur = p.CodeFournisseur ?? "", IdFournisseur = p.IdFournisseur ?? "" })
                .Select(g => new EntityStatDto
                {
                    Code = g.Key.CodeFournisseur,
                    Nom = g.Key.IdFournisseur,
                    NombrePesees = g.Count(),
                    PoidsBrutTotal = g.Sum(p => p.Poids1 ?? 0),
                    PoidsNetTotal = g.Sum(p => p.PoidsNet ?? 0),
                    PoidsMoyen = g.Average(p => p.PoidsNet ?? 0)
                })
                .OrderByDescending(s => s.PoidsNetTotal)
                .Take(filter.TopN)
                .ToList();

            // Par Client (Top N)
            result.ParClient = data
                .Where(p => !string.IsNullOrEmpty(p.NomClient))
                .GroupBy(p => new { CodeClient = p.CodeClient ?? "", NomClient = p.NomClient ?? "" })
                .Select(g => new EntityStatDto
                {
                    Code = g.Key.CodeClient,
                    Nom = g.Key.NomClient,
                    NombrePesees = g.Count(),
                    PoidsBrutTotal = g.Sum(p => p.Poids1 ?? 0),
                    PoidsNetTotal = g.Sum(p => p.PoidsNet ?? 0),
                    PoidsMoyen = g.Average(p => p.PoidsNet ?? 0)
                })
                .OrderByDescending(s => s.PoidsNetTotal)
                .Take(filter.TopN)
                .ToList();

            // Par Produit (Top N)
            result.ParProduit = data
                .Where(p => !string.IsNullOrEmpty(p.Label))
                .GroupBy(p => p.Label ?? "")
                .Select(g => new EntityStatDto
                {
                    Code = g.Key,
                    Nom = g.Key,
                    NombrePesees = g.Count(),
                    PoidsBrutTotal = g.Sum(p => p.Poids1 ?? 0),
                    PoidsNetTotal = g.Sum(p => p.PoidsNet ?? 0),
                    PoidsMoyen = g.Average(p => p.PoidsNet ?? 0)
                })
                .OrderByDescending(s => s.PoidsNetTotal)
                .Take(filter.TopN)
                .ToList();

            // Par Mouvement
            var totalPoidsNet = data.Sum(p => p.PoidsNet ?? 0);
            result.ParMouvement = data
                .GroupBy(p => p.Mouvement ?? "Non defini")
                .Select(g => new MouvementStatDto
                {
                    Mouvement = g.Key,
                    NombrePesees = g.Count(),
                    PoidsBrutTotal = g.Sum(p => p.Poids1 ?? 0),
                    PoidsNetTotal = g.Sum(p => p.PoidsNet ?? 0),
                    Pourcentage = totalPoidsNet > 0 ? Math.Round(g.Sum(p => p.PoidsNet ?? 0) / totalPoidsNet * 100, 2) : 0
                })
                .OrderByDescending(s => s.PoidsNetTotal)
                .ToList();

            // Par Jour (derniers 30 jours)
            result.ParJour = data
                .Where(p => p.Dmv.HasValue)
                .GroupBy(p => p.Dmv!.Value.Date)
                .Select(g => new PeriodStatDto
                {
                    Periode = g.Key.ToString("yyyy-MM-dd"),
                    DateDebut = g.Key,
                    DateFin = g.Key.AddDays(1).AddSeconds(-1),
                    NombrePesees = g.Count(),
                    PoidsBrutTotal = g.Sum(p => p.Poids1 ?? 0),
                    PoidsNetTotal = g.Sum(p => p.PoidsNet ?? 0)
                })
                .OrderBy(s => s.DateDebut)
                .TakeLast(30)
                .ToList();

            // Par Mois (derniers 12 mois)
            result.ParMois = data
                .Where(p => p.Dmv.HasValue)
                .GroupBy(p => new { p.Dmv!.Value.Year, p.Dmv!.Value.Month })
                .Select(g => new PeriodStatDto
                {
                    Periode = $"{g.Key.Year}-{g.Key.Month:D2}",
                    DateDebut = new DateTime(g.Key.Year, g.Key.Month, 1),
                    DateFin = new DateTime(g.Key.Year, g.Key.Month, 1).AddMonths(1).AddSeconds(-1),
                    NombrePesees = g.Count(),
                    PoidsBrutTotal = g.Sum(p => p.Poids1 ?? 0),
                    PoidsNetTotal = g.Sum(p => p.PoidsNet ?? 0)
                })
                .OrderBy(s => s.DateDebut)
                .TakeLast(12)
                .ToList();

            return result;
        }

        public async Task<List<EntityStatDto>> GetStatsByFournisseurAsync(StatisticsFilterDto filter)
        {
            var query = GetBaseQuery(filter);
            var data = await query.ToListAsync();

            return data
                .Where(p => !string.IsNullOrEmpty(p.IdFournisseur))
                .GroupBy(p => new { p.CodeFournisseur, p.IdFournisseur })
                .Select(g => new EntityStatDto
                {
                    Code = g.Key.CodeFournisseur ?? "",
                    Nom = g.Key.IdFournisseur ?? "",
                    NombrePesees = g.Count(),
                    PoidsBrutTotal = g.Sum(p => p.Poids1 ?? 0),
                    PoidsNetTotal = g.Sum(p => p.PoidsNet ?? 0),
                    PoidsMoyen = g.Average(p => p.PoidsNet ?? 0)
                })
                .OrderByDescending(s => s.PoidsNetTotal)
                .Take(filter.TopN)
                .ToList();
        }

        public async Task<List<EntityStatDto>> GetStatsByClientAsync(StatisticsFilterDto filter)
        {
            var query = GetBaseQuery(filter);
            var data = await query.ToListAsync();

            return data
                .Where(p => !string.IsNullOrEmpty(p.NomClient))
                .GroupBy(p => new { p.CodeClient, p.NomClient })
                .Select(g => new EntityStatDto
                {
                    Code = g.Key.CodeClient ?? "",
                    Nom = g.Key.NomClient ?? "",
                    NombrePesees = g.Count(),
                    PoidsBrutTotal = g.Sum(p => p.Poids1 ?? 0),
                    PoidsNetTotal = g.Sum(p => p.PoidsNet ?? 0),
                    PoidsMoyen = g.Average(p => p.PoidsNet ?? 0)
                })
                .OrderByDescending(s => s.PoidsNetTotal)
                .Take(filter.TopN)
                .ToList();
        }

        public async Task<List<EntityStatDto>> GetStatsByProduitAsync(StatisticsFilterDto filter)
        {
            var query = GetBaseQuery(filter);
            var data = await query.ToListAsync();

            return data
                .Where(p => !string.IsNullOrEmpty(p.Label))
                .GroupBy(p => p.Label)
                .Select(g => new EntityStatDto
                {
                    Code = g.Key ?? "",
                    Nom = g.Key ?? "",
                    NombrePesees = g.Count(),
                    PoidsBrutTotal = g.Sum(p => p.Poids1 ?? 0),
                    PoidsNetTotal = g.Sum(p => p.PoidsNet ?? 0),
                    PoidsMoyen = g.Average(p => p.PoidsNet ?? 0)
                })
                .OrderByDescending(s => s.PoidsNetTotal)
                .Take(filter.TopN)
                .ToList();
        }

        public async Task<List<MouvementStatDto>> GetStatsByMouvementAsync(StatisticsFilterDto filter)
        {
            var query = GetBaseQuery(filter);
            var data = await query.ToListAsync();

            var totalPoidsNet = data.Sum(p => p.PoidsNet ?? 0);

            return data
                .GroupBy(p => p.Mouvement ?? "Non defini")
                .Select(g => new MouvementStatDto
                {
                    Mouvement = g.Key,
                    NombrePesees = g.Count(),
                    PoidsBrutTotal = g.Sum(p => p.Poids1 ?? 0),
                    PoidsNetTotal = g.Sum(p => p.PoidsNet ?? 0),
                    Pourcentage = totalPoidsNet > 0 ? Math.Round(g.Sum(p => p.PoidsNet ?? 0) / totalPoidsNet * 100, 2) : 0
                })
                .OrderByDescending(s => s.PoidsNetTotal)
                .ToList();
        }

        public async Task<List<PeriodStatDto>> GetStatsByPeriodAsync(StatisticsFilterDto filter, string periodType)
        {
            var query = GetBaseQuery(filter);
            var data = await query.Where(p => p.Dmv.HasValue).ToListAsync();

            if (periodType.ToLower() == "month" || periodType.ToLower() == "mois")
            {
                return data
                    .GroupBy(p => new { p.Dmv!.Value.Year, p.Dmv!.Value.Month })
                    .Select(g => new PeriodStatDto
                    {
                        Periode = $"{g.Key.Year}-{g.Key.Month:D2}",
                        DateDebut = new DateTime(g.Key.Year, g.Key.Month, 1),
                        DateFin = new DateTime(g.Key.Year, g.Key.Month, 1).AddMonths(1).AddSeconds(-1),
                        NombrePesees = g.Count(),
                        PoidsBrutTotal = g.Sum(p => p.Poids1 ?? 0),
                        PoidsNetTotal = g.Sum(p => p.PoidsNet ?? 0)
                    })
                    .OrderBy(s => s.DateDebut)
                    .ToList();
            }
            else if (periodType.ToLower() == "week" || periodType.ToLower() == "semaine")
            {
                return data
                    .GroupBy(p => new {
                        Year = p.Dmv!.Value.Year,
                        Week = System.Globalization.ISOWeek.GetWeekOfYear(p.Dmv!.Value)
                    })
                    .Select(g => new PeriodStatDto
                    {
                        Periode = $"{g.Key.Year}-S{g.Key.Week:D2}",
                        DateDebut = System.Globalization.ISOWeek.ToDateTime(g.Key.Year, g.Key.Week, DayOfWeek.Monday),
                        DateFin = System.Globalization.ISOWeek.ToDateTime(g.Key.Year, g.Key.Week, DayOfWeek.Sunday),
                        NombrePesees = g.Count(),
                        PoidsBrutTotal = g.Sum(p => p.Poids1 ?? 0),
                        PoidsNetTotal = g.Sum(p => p.PoidsNet ?? 0)
                    })
                    .OrderBy(s => s.DateDebut)
                    .ToList();
            }
            else // day / jour
            {
                return data
                    .GroupBy(p => p.Dmv!.Value.Date)
                    .Select(g => new PeriodStatDto
                    {
                        Periode = g.Key.ToString("yyyy-MM-dd"),
                        DateDebut = g.Key,
                        DateFin = g.Key.AddDays(1).AddSeconds(-1),
                        NombrePesees = g.Count(),
                        PoidsBrutTotal = g.Sum(p => p.Poids1 ?? 0),
                        PoidsNetTotal = g.Sum(p => p.PoidsNet ?? 0)
                    })
                    .OrderBy(s => s.DateDebut)
                    .ToList();
            }
        }
    }
}
