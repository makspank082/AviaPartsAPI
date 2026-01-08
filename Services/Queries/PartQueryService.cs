using AviaPartsAPI.Data;
using AviaPartsAPI.Models;
using AviaPartsAPI.Models.DTOs;
using AviaPartsAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AviaPartsAPI.Services.Queries
{
    public class PartQueryService : IPartQueryService
    {
        private readonly AppDbContext _context;

        public PartQueryService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<PartResponseDto> GetPartByIdAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            var part = await _context.Parts
                .FindAsync(new object[] { id }, cancellationToken);

            return part == null ? null : MapToDto(part);
        }

        public async Task<IEnumerable<PartResponseDto>> GetAllPartsAsync(
            CancellationToken cancellationToken = default)
        {
            var parts = await _context.Parts
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            return parts.Select(MapToDto);
        }

        public async Task<PagedResponse<PartResponseDto>> GetPartsPagedAsync(
            int pageNumber = 1,
            int pageSize = 10,
            CancellationToken cancellationToken = default)
        {
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize > 100) pageSize = 100;
            if (pageSize < 1) pageSize = 10;

            var query = _context.Parts.AsNoTracking();
            var totalCount = await query.CountAsync(cancellationToken);

            var items = await query
                .OrderBy(p => p.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(p => MapToDto(p))
                .ToListAsync(cancellationToken);

            return new PagedResponse<PartResponseDto>(items, pageNumber, pageSize, totalCount);
        }

        private static PartResponseDto MapToDto(Part part)
        {
            return new PartResponseDto
            {
                Id = part.Id,
                SerialNumber = part.SerialNumber,
                Name = part.Name,
                Description = part.Description,
                StockQuantity = part.StockQuantity,
                MinimumStockLevel = part.MinimumStockLevel,
                StorageLocation = part.StorageLocation,
                Status = part.Status,
                CreatedAt = part.CreatedAt,
                SupplierId = part.SupplierId,
                CategoryId = part.CategoryId
            };
        }

        public async Task<IEnumerable<PartResponseDto>> GetLowStockPartsAsync(
            CancellationToken cancellationToken = default)
        {
            var lowStockParts = await _context.Parts
                .Where(p => p.StockQuantity < p.MinimumStockLevel && p.StockQuantity > 0)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            return lowStockParts.Select(MapToDto);
        }
    }
}
