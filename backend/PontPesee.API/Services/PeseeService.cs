using Microsoft.EntityFrameworkCore;
using PontPesee.API.Data;
using PontPesee.API.DTOs;
using PontPesee.API.Models;

namespace PontPesee.API.Services
{
    public class PeseeService : IPeseeService
    {
        private readonly ApplicationDbContext _context;

        public PeseeService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PeseeStatsDto> GetPeseesAsync(PeseeFilterDto filter)
        {
            // Increase command timeout for long-running queries (short-term mitigation)
            try
            {
                _context.Database.SetCommandTimeout(180); // 3 minutes
            }
            catch
            {
                // If the provider doesn't support setting timeout, ignore and proceed
            }

            // Récupérer les données depuis la table appropriée
            var query = filter.TableName?.ToLower() == "peseep2"
                ? _context.Set<Pesee>().FromSqlRaw("SELECT * FROM peseep2")
                : _context.Pesees.AsQueryable();

            // Appliquer les filtres de base
            query = query.Where(p => p.Imprime && p.CodePesee != 0 && p.Poids2 != 0);

            // Filtres de dates
            if (filter.DateDebut.HasValue)
            {
                query = query.Where(p => p.Dmv >= filter.DateDebut.Value);
            }

            if (filter.DateFin.HasValue)
            {
                query = query.Where(p => p.Dmv <= filter.DateFin.Value.AddDays(1).AddSeconds(-1));
            }

            // Filtres métier
            if (!string.IsNullOrEmpty(filter.Mouvement))
            {
                query = query.Where(p => p.Mouvement == filter.Mouvement);
            }

            if (!string.IsNullOrEmpty(filter.Fournisseur))
            {
                query = query.Where(p => p.IdFournisseur == filter.Fournisseur);
            }

            if (!string.IsNullOrEmpty(filter.Client))
            {
                query = query.Where(p => p.NomClient == filter.Client);
            }

            if (!string.IsNullOrEmpty(filter.Destination))
            {
                query = query.Where(p => p.Destination == filter.Destination);
            }

            if (!string.IsNullOrEmpty(filter.Provenance))
            {
                query = query.Where(p => p.Provenance == filter.Provenance);
            }

            if (!string.IsNullOrEmpty(filter.Produit))
            {
                query = query.Where(p => p.Label == filter.Produit);
            }

            if (!string.IsNullOrEmpty(filter.Vehicule))
            {
                query = query.Where(p => p.Immatriculation == filter.Vehicule);
            }

            if (!string.IsNullOrEmpty(filter.Chauffeur))
            {
                query = query.Where(p => p.Chauffeur == filter.Chauffeur);
            }

            if (!string.IsNullOrEmpty(filter.CodeSite))
            {
                query = query.Where(p => p.CodeSite == filter.CodeSite);
            }

            // Filtres d'état
            if (filter.Acceptes == true)
            {
                query = query.Where(p => p.Imprime && !p.Annuler);
            }
            else if (filter.Annules == true)
            {
                query = query.Where(p => p.Annuler);
            }

            // Tri par date décroissante
            query = query.OrderByDescending(p => p.CodePesee);

            // Statistiques
            var totalCount = await query.CountAsync();
            var poidsBrutTotal = await query.SumAsync(p => (double?)p.Poids1) ?? 0;
            var poidsNetTotal = await query.SumAsync(p => (double?)p.PoidsNet) ?? 0;

            // Pagination (Page commence à 0)
            var pageSize = filter.PageSize > 0 ? filter.PageSize : 10;
            var page = filter.Page >= 0 ? filter.Page : 0;
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            var pesees = await query
                .Skip(page * pageSize)
                .Take(pageSize)
                .Select(p => new PeseeDto
                {
                    CodePesee = p.CodePesee,
                    CodeSite = p.CodeSite,
                    Campagne = p.Campagne,
                    Immatriculation = p.Immatriculation,
                    Poids1 = p.Poids1,
                    DateP1 = p.DateP1,
                    DateP2 = p.DateP2,
                    Poids2 = p.Poids2,
                    PoidsNet = p.PoidsNet,
                    CodeFournisseur = p.CodeFournisseur,
                    IdFournisseur = p.IdFournisseur,
                    CodeClient = p.CodeClient,
                    NomClient = p.NomClient,
                    Mouvement = p.Mouvement,
                    Provenance = p.Provenance,
                    Destination = p.Destination,
                    Imprime = p.Imprime,
                    NumTicket = p.NumTicket,
                    Chauffeur = p.Chauffeur,
                    Peseur = p.Peseur,
                    Peseur2 = p.Peseur2,
                    Annuler = p.Annuler,
                    Label = p.Label,
                    Dmv = p.Dmv,
                    Remorque = p.Remorque
                })
                .ToListAsync();

            return new PeseeStatsDto
            {
                NombrePesees = totalCount,
                PoidsBrutTotal = poidsBrutTotal,
                PoidsNetTotal = poidsNetTotal,
                Pesees = pesees,
                TotalPages = totalPages,
                CurrentPage = page
            };
        }

        public async Task<PeseeDto?> GetPeseeByTicketAsync(string numTicket, string tableName)
        {
            IQueryable<Pesee> baseQuery;

            if (tableName.ToLower() == "peseep2")
            {
                // Pour peseep2, utiliser FromSqlRaw sans paramètre dans le WHERE
                baseQuery = _context.Set<Pesee>().FromSqlRaw("SELECT * FROM peseep2");
            }
            else
            {
                // Pour pesee, utiliser le DbSet normal
                baseQuery = _context.Pesees;
            }

            // Appliquer le filtre et la projection
            var peseeDto = await baseQuery
                .Where(p => p.NumTicket == numTicket)
                .Select(p => new PeseeDto
                {
                    CodePesee = p.CodePesee,
                    CodeSite = p.CodeSite,
                    Campagne = p.Campagne,
                    Immatriculation = p.Immatriculation,
                    Poids1 = p.Poids1,
                    DateP1 = p.DateP1,
                    DateP2 = p.DateP2,
                    Poids2 = p.Poids2,
                    PoidsNet = p.PoidsNet,
                    CodeFournisseur = p.CodeFournisseur,
                    IdFournisseur = p.IdFournisseur,
                    CodeClient = p.CodeClient,
                    NomClient = p.NomClient,
                    Mouvement = p.Mouvement,
                    Provenance = p.Provenance,
                    Destination = p.Destination,
                    Imprime = p.Imprime,
                    NumTicket = p.NumTicket,
                    Chauffeur = p.Chauffeur,
                    Peseur = p.Peseur,
                    Peseur2 = p.Peseur2,
                    Annuler = p.Annuler,
                    Label = p.Label,
                    Dmv = p.Dmv,
                    Remorque = p.Remorque
                })
                .FirstOrDefaultAsync();

            return peseeDto;
        }

        public async Task<List<string>> GetProvenancesAsync(string tableName)
        {
            var query = tableName.ToLower() == "peseep2"
                ? _context.Set<Pesee>().FromSqlRaw("SELECT DISTINCT provenance FROM peseep2 WHERE provenance IS NOT NULL AND provenance <> ''")
                : _context.Pesees.Where(p => !string.IsNullOrEmpty(p.Provenance));

            return await query
                .Select(p => p.Provenance!)
                .Distinct()
                .OrderBy(p => p)
                .ToListAsync();
        }

        public async Task<List<string>> GetDestinationsAsync(string tableName)
        {
            var query = tableName.ToLower() == "peseep2"
                ? _context.Set<Pesee>().FromSqlRaw("SELECT DISTINCT destination FROM peseep2 WHERE destination IS NOT NULL AND destination <> ''")
                : _context.Pesees.Where(p => !string.IsNullOrEmpty(p.Destination));

            return await query
                .Select(p => p.Destination!)
                .Distinct()
                .OrderBy(p => p)
                .ToListAsync();
        }

        public async Task<Dictionary<string, string>> GetFournisseursAsync(string tableName)
        {
            var query = tableName.ToLower() == "peseep2"
                ? _context.Set<Pesee>().FromSqlRaw("SELECT DISTINCT CodeFournisseur, IdFournisseur FROM peseep2 WHERE CodeFournisseur IS NOT NULL")
                : _context.Pesees.Where(p => !string.IsNullOrEmpty(p.CodeFournisseur));

            var fournisseurs = await query
                .Select(p => new { p.CodeFournisseur, p.IdFournisseur })
                .ToListAsync();

            // Grouper par CodeFournisseur et prendre la première valeur pour éviter les doublons
            return fournisseurs
                .GroupBy(f => f.CodeFournisseur)
                .ToDictionary(
                    g => g.Key!,
                    g => g.First().IdFournisseur ?? ""
                );
        }

        public async Task<Dictionary<string, string>> GetClientsAsync(string tableName)
        {
            var query = tableName.ToLower() == "peseep2"
                ? _context.Set<Pesee>().FromSqlRaw("SELECT DISTINCT codeclient, nomclient FROM peseep2 WHERE codeclient IS NOT NULL")
                : _context.Pesees.Where(p => !string.IsNullOrEmpty(p.CodeClient));

            var clients = await query
                .Select(p => new { p.CodeClient, p.NomClient })
                .ToListAsync();

            // Grouper par CodeClient et prendre la première valeur pour éviter les doublons
            return clients
                .GroupBy(c => c.CodeClient)
                .ToDictionary(
                    g => g.Key!,
                    g => g.First().NomClient ?? ""
                );
        }

        public async Task<List<string>> GetProduitsAsync(string tableName)
        {
            var query = tableName.ToLower() == "peseep2"
                ? _context.Set<Pesee>().FromSqlRaw("SELECT DISTINCT Label FROM peseep2 WHERE Label IS NOT NULL AND Label <> ''")
                : _context.Pesees.Where(p => !string.IsNullOrEmpty(p.Label));

            return await query
                .Select(p => p.Label!)
                .Distinct()
                .OrderBy(p => p)
                .ToListAsync();
        }

        public async Task<List<string>> GetVehiculesAsync(string tableName)
        {
            var query = tableName.ToLower() == "peseep2"
                ? _context.Set<Pesee>().FromSqlRaw("SELECT DISTINCT Immatriculation FROM peseep2 WHERE Immatriculation IS NOT NULL AND Immatriculation <> ''")
                : _context.Pesees.Where(p => !string.IsNullOrEmpty(p.Immatriculation));

            return await query
                .Select(p => p.Immatriculation!)
                .Distinct()
                .OrderBy(p => p)
                .ToListAsync();
        }
    }
}
