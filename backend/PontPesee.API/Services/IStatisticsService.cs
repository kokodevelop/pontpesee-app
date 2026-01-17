using PontPesee.API.DTOs;

namespace PontPesee.API.Services
{
    public interface IStatisticsService
    {
        Task<AdvancedStatisticsDto> GetAdvancedStatisticsAsync(StatisticsFilterDto filter);
        Task<List<EntityStatDto>> GetStatsByFournisseurAsync(StatisticsFilterDto filter);
        Task<List<EntityStatDto>> GetStatsByClientAsync(StatisticsFilterDto filter);
        Task<List<EntityStatDto>> GetStatsByProduitAsync(StatisticsFilterDto filter);
        Task<List<MouvementStatDto>> GetStatsByMouvementAsync(StatisticsFilterDto filter);
        Task<List<PeriodStatDto>> GetStatsByPeriodAsync(StatisticsFilterDto filter, string periodType);
    }
}
