using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PontPesee.API.DTOs;
using PontPesee.API.Services;

namespace PontPesee.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PeseeController : ControllerBase
    {
        private readonly IPeseeService _peseeService;

        public PeseeController(IPeseeService peseeService)
        {
            _peseeService = peseeService;
        }

        /// <summary>
        /// Récupère les pesées avec filtres et statistiques
        /// </summary>
        [HttpPost("search")]
        public async Task<ActionResult<PeseeStatsDto>> SearchPesees([FromBody] PeseeFilterDto filter)
        {
            try
            {
                var result = await _peseeService.GetPeseesAsync(filter);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erreur lors de la récupération des données", error = ex.Message });
            }
        }

        /// <summary>
        /// Récupère une pesée par son numéro de ticket
        /// </summary>
        [HttpGet("ticket")]
        public async Task<IActionResult> GetByTicket([FromQuery] string numTicket, [FromQuery] string tableName = "pesee")
        {
            try
            {
                var pesee = await _peseeService.GetPeseeByTicketAsync(numTicket, tableName);

                if (pesee == null)
                    return NotFound(new { message = "Ticket non trouvé" });

                return Ok(pesee);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erreur lors de la récupération du ticket", error = ex.Message });
            }
        }

        /// <summary>
        /// Récupère la liste des provenances
        /// </summary>
        [HttpGet("provenances")]
        public async Task<ActionResult<List<string>>> GetProvenances([FromQuery] string tableName = "pesee")
        {
            try
            {
                var provenances = await _peseeService.GetProvenancesAsync(tableName);
                return Ok(provenances);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erreur", error = ex.Message });
            }
        }

        /// <summary>
        /// Récupère la liste des destinations
        /// </summary>
        [HttpGet("destinations")]
        public async Task<ActionResult<List<string>>> GetDestinations([FromQuery] string tableName = "pesee")
        {
            try
            {
                var destinations = await _peseeService.GetDestinationsAsync(tableName);
                return Ok(destinations);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erreur", error = ex.Message });
            }
        }

        /// <summary>
        /// Récupère la liste des fournisseurs
        /// </summary>
        [HttpGet("fournisseurs")]
        public async Task<ActionResult<Dictionary<string, string>>> GetFournisseurs([FromQuery] string tableName = "pesee")
        {
            try
            {
                var fournisseurs = await _peseeService.GetFournisseursAsync(tableName);
                return Ok(fournisseurs);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erreur", error = ex.Message });
            }
        }

        /// <summary>
        /// Récupère la liste des clients
        /// </summary>
        [HttpGet("clients")]
        public async Task<ActionResult<Dictionary<string, string>>> GetClients([FromQuery] string tableName = "pesee")
        {
            try
            {
                var clients = await _peseeService.GetClientsAsync(tableName);
                return Ok(clients);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erreur", error = ex.Message });
            }
        }

        /// <summary>
        /// Récupère la liste des produits
        /// </summary>
        [HttpGet("produits")]
        public async Task<ActionResult<List<string>>> GetProduits([FromQuery] string tableName = "pesee")
        {
            try
            {
                var produits = await _peseeService.GetProduitsAsync(tableName);
                return Ok(produits);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erreur", error = ex.Message });
            }
        }

        /// <summary>
        /// Récupère la liste des véhicules
        /// </summary>
        [HttpGet("vehicules")]
        public async Task<ActionResult<List<string>>> GetVehicules([FromQuery] string tableName = "pesee")
        {
            try
            {
                var vehicules = await _peseeService.GetVehiculesAsync(tableName);
                return Ok(vehicules);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erreur", error = ex.Message });
            }
        }
    }
}
