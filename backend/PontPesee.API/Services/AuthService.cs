using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PontPesee.API.Data;
using PontPesee.API.DTOs;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PontPesee.API.Services
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthService(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<LoginResponseDto?> AuthenticateAsync(LoginDto loginDto)
        {
            // Rechercher l'utilisateur
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.NomUtilisateur == loginDto.Username
                                       && u.MotPasse == loginDto.Password);

            if (user == null)
                return null;

            // Générer le token JWT
            var token = GenerateJwtToken(user.NomUtilisateur, user.Groupe ?? "PESEUR");
            var expiresAt = DateTime.UtcNow.AddHours(8);

            return new LoginResponseDto
            {
                Token = token,
                Username = user.NomUtilisateur,
                Profil = user.Groupe ?? "PESEUR",
                ExpiresAt = expiresAt
            };
        }

        public string GenerateJwtToken(string username, string profil)
        {
            var jwtKey = _configuration["Jwt:Key"];
            Console.WriteLine($"[AuthService] JWT Key from config: {jwtKey ?? "NULL"}");
            Console.WriteLine($"[AuthService] JWT Key Length: {(jwtKey?.Length ?? 0)} caractères");

            if (string.IsNullOrEmpty(jwtKey))
            {
                throw new InvalidOperationException("JWT Key not found in configuration in AuthService!");
            }

            var securityKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtKey));

            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, profil),
                new Claim("Profil", profil)
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(8),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
