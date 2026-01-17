using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PontPesee.API.DTOs;
using PontPesee.API.Services;

namespace PontPesee.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class StatisticsController : ControllerBase
    {
        private readonly IStatisticsService _statisticsService;

        public StatisticsController(IStatisticsService statisticsService)
        {
            _statisticsService = statisticsService;
        }

        /// <summary>
        /// Obtient les statistiques avancées complètes
        /// </summary>
        [HttpPost("advanced")]
        public async Task<ActionResult<AdvancedStatisticsDto>> GetAdvancedStatistics([FromBody] StatisticsFilterDto filter)
        {
            try
            {
                var result = await _statisticsService.GetAdvancedStatisticsAsync(filter);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Erreur lors de la récupération des statistiques", details = ex.Message });
            }
        }

        /// <summary>
        /// Obtient les statistiques par fournisseur
        /// </summary>
        [HttpPost("by-fournisseur")]
        public async Task<ActionResult<List<EntityStatDto>>> GetStatsByFournisseur([FromBody] StatisticsFilterDto filter)
        {
            try
            {
                var result = await _statisticsService.GetStatsByFournisseurAsync(filter);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Erreur lors de la récupération des statistiques par fournisseur", details = ex.Message });
            }
        }

        /// <summary>
        /// Obtient les statistiques par client
        /// </summary>
        [HttpPost("by-client")]
        public async Task<ActionResult<List<EntityStatDto>>> GetStatsByClient([FromBody] StatisticsFilterDto filter)
        {
            try
            {
                var result = await _statisticsService.GetStatsByClientAsync(filter);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Erreur lors de la récupération des statistiques par client", details = ex.Message });
            }
        }

        /// <summary>
        /// Obtient les statistiques par produit
        /// </summary>
        [HttpPost("by-produit")]
        public async Task<ActionResult<List<EntityStatDto>>> GetStatsByProduit([FromBody] StatisticsFilterDto filter)
        {
            try
            {
                var result = await _statisticsService.GetStatsByProduitAsync(filter);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Erreur lors de la récupération des statistiques par produit", details = ex.Message });
            }
        }

        /// <summary>
        /// Obtient les statistiques par mouvement (Entrée/Sortie)
        /// </summary>
        [HttpPost("by-mouvement")]
        public async Task<ActionResult<List<MouvementStatDto>>> GetStatsByMouvement([FromBody] StatisticsFilterDto filter)
        {
            try
            {
                var result = await _statisticsService.GetStatsByMouvementAsync(filter);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Erreur lors de la récupération des statistiques par mouvement", details = ex.Message });
            }
        }

        /// <summary>
        /// Obtient les statistiques par période (jour, semaine, mois)
        /// </summary>
        [HttpPost("by-period/{periodType}")]
        public async Task<ActionResult<List<PeriodStatDto>>> GetStatsByPeriod([FromBody] StatisticsFilterDto filter, string periodType = "jour")
        {
            try
            {
                var result = await _statisticsService.GetStatsByPeriodAsync(filter, periodType);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Erreur lors de la récupération des statistiques par période", details = ex.Message });
            }
        }
    }
}
