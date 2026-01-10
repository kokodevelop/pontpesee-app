using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PontPesee.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HealthController : ControllerBase
    {
        /// <summary>
        /// Endpoint de test sans authentification
        /// </summary>
        [HttpGet("ping")]
        [AllowAnonymous]
        public IActionResult Ping()
        {
            return Ok(new
            {
                status = "OK",
                message = "API is running",
                timestamp = DateTime.UtcNow
            });
        }

        /// <summary>
        /// Endpoint de test avec authentification
        /// </summary>
        [HttpGet("secure-ping")]
        [Authorize]
        public IActionResult SecurePing()
        {
            var username = User.Identity?.Name ?? "Unknown";
            var roles = User.Claims
                .Where(c => c.Type == System.Security.Claims.ClaimTypes.Role)
                .Select(c => c.Value)
                .ToList();

            return Ok(new
            {
                status = "OK",
                message = "You are authenticated",
                username = username,
                roles = roles,
                timestamp = DateTime.UtcNow
            });
        }
    }
}
