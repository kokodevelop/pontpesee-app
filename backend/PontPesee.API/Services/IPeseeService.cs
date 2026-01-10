using PontPesee.API.DTOs;
using PontPesee.API.Models;

namespace PontPesee.API.Services
{
    public interface IPeseeService
    {
        Task<PeseeStatsDto> GetPeseesAsync(PeseeFilterDto filter);
        Task<PeseeDto?> GetPeseeByTicketAsync(string numTicket, string tableName);
        Task<List<string>> GetProvenancesAsync(string tableName);
        Task<List<string>> GetDestinationsAsync(string tableName);
        Task<Dictionary<string, string>> GetFournisseursAsync(string tableName);
        Task<Dictionary<string, string>> GetClientsAsync(string tableName);
        Task<List<string>> GetProduitsAsync(string tableName);
        Task<List<string>> GetVehiculesAsync(string tableName);
    }
}
