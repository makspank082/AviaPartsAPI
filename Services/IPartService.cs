using AviaPartsAPI.Models.DTOs;
using AviaPartsAPI.Models;

namespace AviaPartsAPI.Services
{
    public interface IPartService
    {
        IEnumerable<PartResponseDto> GetAllParts();
        PartResponseDto? GetPartById(int id);
        PartResponseDto CreatePart(CreatePartDto dto);
        PartResponseDto? UpdatePart (int id, UpdatePartDto dto);
        bool DeletePart(int id);

        IEnumerable<PartResponseDto> GetLowStockParts();
    }
}
