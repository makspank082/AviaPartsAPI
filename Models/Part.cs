using System.ComponentModel.DataAnnotations;

namespace AviaPartsAPI.Models
{
    public class Part
    {
        public int Id { get; set; }

        [Required, MaxLength(50)]
        public string SerialNumber {  get; set; } = string.Empty;

        [Required, MinLength(3), MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        [Range(0, int.MaxValue)]
        public int StockQuantity { get; set; } = 0;

        [Range(1, 1000)]
        public int MinimumStockLevel { get; set; } = 10;

        [Range(1, 10000)]
        public int ReorderQuantity { get; set; } = 50;

        [Required, MaxLength(20)]
        public string StorageLocation { get; set; } = "A-01";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? LastUpdatedAt { get; set; }
        public DateTime? LastStockTakeDate { get; set; }
        public int? SupplierId { get; set; }
        public int? CategoryId { get; set; }
        public bool IsActive { get; set; } = true;
        public PartStatus Status { get; set; } = PartStatus.InStock;
    }
}
