using Microsoft.AspNetCore.Mvc;
using PontPesee.API.DTOs;
using PontPesee.API.Services;

namespace PontPesee.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginDto loginDto)
        {
            var result = await _authService.AuthenticateAsync(loginDto);

            if (result == null)
            {
                return Unauthorized(new { message = "Nom utilisateur ou mot de passe incorrect" });
            }

            return Ok(result);
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            // En JWT, le logout côté serveur est optionnel
            // Le client doit simplement supprimer le token
            return Ok(new { message = "Déconnexion réussie" });
        }
    }
}
