using AviaPartsAPI.Models.DTOs;

namespace AviaPartsAPI.Services.Interfaces
{
    public interface IPartQueryService
    {
        Task<PartResponseDto> GetPartByIdAsync(int id, CancellationToken cancellationToken = default);

        Task<IEnumerable<PartResponseDto>> GetAllPartsAsync(CancellationToken cancellationToken = default);

        Task<PagedResponse<PartResponseDto>> GetPartsPagedAsync(
            int pageNumber = 1,
            int pageSize = 10,
            CancellationToken cancellationToken = default);

        Task<IEnumerable<PartResponseDto>> GetLowStockPartsAsync(
        CancellationToken cancellationToken = default);
    }
}