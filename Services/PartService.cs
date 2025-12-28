using AviaPartsAPI.Models;
using AviaPartsAPI.Models.DTOs;
using System.Linq;

namespace AviaPartsAPI.Services
{
    public class PartService : IPartService
    {
        private readonly List<Part> _parts;

        private int _nextId = 1;

        public PartService()
        {
            _parts = new List<Part>();

            SeedTestData();
        }
        private void SeedTestData()
        {
            CreatePart(new CreatePartDto
            {
                Name = "Турбина",
                Description = "Турбина высокого давления",
                SerialNumber = "TUR-001001",
                InitialQuantity = 15,
                MinimumStockLevel = 5,
                StorageLocation = "A-01"
            });
            CreatePart(new CreatePartDto
            {
                Name = "Лонжерон",
                Description = "Основной силовой элемент крыла",
                SerialNumber = "LON-002002",
                InitialQuantity = 3,
                MinimumStockLevel = 10,
                StorageLocation = "B-05"
            });
        }
        public IEnumerable<PartResponseDto> GetAllParts()
        {
            return _parts
                .Where(p => p.IsActive)
                .Select(p => MapToDto(p))
                .ToList();
        }
        public PartResponseDto? GetPartById(int id)
        {

            var part = _parts.FirstOrDefault(p => p.Id == id && p.IsActive);


            if (part == null)
                return null;

            return MapToDto(part);
        }
        public PartResponseDto CreatePart(CreatePartDto dto)
        {
            var part = new Part
            {
                Id = _nextId++,
                Name = dto.Name,
                Description = dto.Description,
                SerialNumber = dto.SerialNumber,
                StockQuantity = dto.InitialQuantity,
                MinimumStockLevel = dto.MinimumStockLevel,
                ReorderQuantity = 50,
                StorageLocation = dto.StorageLocation,
                CreatedAt = DateTime.UtcNow,
                IsActive = true,
                Status = CalculateStatus(dto.InitialQuantity, dto.MinimumStockLevel)
            };
            _parts.Add(part);

            return MapToDto(part);
        }
        public PartResponseDto? UpdatePart(int id, UpdatePartDto dto)
        {
            var part = _parts.FirstOrDefault(p => p.Id == id && p.IsActive);

            if (part == null)
                return null;


            if (dto.Name != null)
                part.Name = dto.Name;

            if (dto.Description != null)
                part.Description = dto.Description;

            if (dto.StorageLocation != null)
                part.StorageLocation = dto.StorageLocation;

            if (dto.MinimumStockLevel.HasValue)
                part.MinimumStockLevel = dto.MinimumStockLevel.Value;

            if (dto.ReorderQuantity.HasValue)
                part.ReorderQuantity = dto.ReorderQuantity.Value;

            part.LastUpdatedAt = DateTime.UtcNow;

            part.Status = CalculateStatus(part.StockQuantity, part.MinimumStockLevel);

            return MapToDto(part);
        }
        public bool DeletePart(int id)
        {
            var part = _parts.FirstOrDefault(p => p.Id == id && p.IsActive);

            if (part == null)
                return false;

            part.IsActive = false;
            part.LastUpdatedAt = DateTime.UtcNow;

            return true;
        }
        public IEnumerable<PartResponseDto> GetLowStockParts()
        {
            return _parts
                .Where(p => p.IsActive
                           && p.StockQuantity <= p.MinimumStockLevel
                           && p.Status != PartStatus.OutOfStock)
                .OrderBy(p => p.StockQuantity)
                .ThenByDescending(p => p.LastUpdatedAt ?? DateTime.MinValue)
                .Select(p => MapToDto(p))
                .ToList();
        }
        private PartResponseDto MapToDto(Part part)
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
                CategoryId = part.CategoryId,
            };
        }
        private PartStatus CalculateStatus(int stockQuantity, int minimumStockLevel)
        {
            if (stockQuantity == 0)
                return PartStatus.OutOfStock;
            if (stockQuantity <= minimumStockLevel)
                return PartStatus.LowStock;
            return PartStatus.InStock;
        }
    }
}
