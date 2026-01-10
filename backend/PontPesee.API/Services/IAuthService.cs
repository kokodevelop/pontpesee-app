using PontPesee.API.DTOs;

namespace PontPesee.API.Services
{
    public interface IAuthService
    {
        Task<LoginResponseDto?> AuthenticateAsync(LoginDto loginDto);
        string GenerateJwtToken(string username, string profil);
    }
}
