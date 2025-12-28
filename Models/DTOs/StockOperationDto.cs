using System.ComponentModel.DataAnnotations;

namespace AviaPartsAPI.Models.DTOs;

public class StockOperationDto
{
    [Required]
    public StockOperationType OperationType { get; set; }

    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Количество должно быть положительным")]
    public int Quantity { get; set; }

    [MaxLength(200)]
    public string? Reason { get; set; }
    // Примеры: "Списание в цех сборки", "Брак", "Инвентаризация"
}

public enum StockOperationType
{
    Withdrawal,     // Списание со склада
    Replenishment,  // Пополнение склада
    Adjustment,     // Корректировка (инвентаризация)
    Transfer        // Перемещение между складами
}
