using System.ComponentModel.DataAnnotations;

namespace AviaPartsAPI.Models.DTOs;

public class CreatePartDto
{
    [Required(ErrorMessage = "Название детали обязательно")]
    [MinLength(3, ErrorMessage = "Название должно быть не менее 3 символов")]
    [MaxLength(100, ErrorMessage = "Название не должно превышать 100 символов")]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    [Required(ErrorMessage = "Серийный номер обязателен")]
    [RegularExpression(@"^[A-Z]{3}-\d{6}$",
        ErrorMessage = "Формат серийного номера: ABC-123456")]
    public string SerialNumber { get; set; } = string.Empty;

    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Начальное количество должно быть положительным")]
    public int InitialQuantity { get; set; }

    [Range(1, 1000)]
    public int MinimumStockLevel { get; set; } = 10;

    [Required]
    [RegularExpression(@"^[A-Z]-\d{2}$",
        ErrorMessage = "Формат места хранения: A-01, B-12")]
    public string StorageLocation { get; set; } = "A-01";
}

