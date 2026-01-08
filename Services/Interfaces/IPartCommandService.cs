using AviaPartsAPI.Models.DTOs;

namespace AviaPartsAPI.Services.Interfaces
{
    public interface IPartCommandService
    {
        Task<PartResponseDto> CreatePartAsync(
            CreatePartDto partDto,
            CancellationToken cancellationToken = default);

        Task UpdatePartAsync(
            int id,
            UpdatePartDto partDto,
            CancellationToken cancellationToken = default);

        Task DeletePartAsync(int id, CancellationToken cancellationToken = default);

        Task<PartResponseDto> UpdateStockAsync(
            int id,
            StockOperationDto stockDto,
            CancellationToken cancellationToken = default);
    }
}
