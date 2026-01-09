using Microsoft.Extensions.Diagnostics.HealthChecks;
using AviaPartsAPI.Data;
using Microsoft.EntityFrameworkCore;
using AviaPartsAPI.Models;

namespace AviaPartsAPI.Services.HealthChecks;

public class InventoryHealthCheck : IHealthCheck
{
    private readonly AppDbContext _context;

    public InventoryHealthCheck(AppDbContext context)
    {
        _context = context;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext healthCheckContext,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var criticalParts = await _context.Parts
                .Where(p => p.StockQuantity <= p.MinimumStockLevel * 0.3) 
                .Select(p => new
                {
                    p.Id,
                    p.Name,
                    StockQuantity = p.StockQuantity,
                    MinimumStockLevel = p.MinimumStockLevel
                })
                .Take(10) 
                .ToListAsync(cancellationToken);


            var outdatedInventoryParts = await _context.Parts
                .Where(p => p.LastStockTakeDate == null ||
                           p.LastStockTakeDate < DateTime.UtcNow.AddMonths(-6))
                .CountAsync(cancellationToken);


            var data = new Dictionary<string, object>
            {
                ["critical_parts_count"] = criticalParts.Count,
                ["critical_parts"] = criticalParts, 
                ["outdated_inventory_count"] = outdatedInventoryParts,
                ["check_timestamp"] = DateTime.UtcNow 
            };


            var status = HealthCheckResult.Healthy("Склад в норме", data);

            if (criticalParts.Count > 0)
            {
                status = HealthCheckResult.Degraded(
                    $"ВНИМАНИЕ: {criticalParts.Count} деталей с критически низким запасом",
                    data: data);
            }


            if (outdatedInventoryParts > 20) 
            {
                status = HealthCheckResult.Degraded(
                    $"ВНИМАНИЕ: {outdatedInventoryParts} деталей требуют переучёта (> 6 месяцев)",
                    data: data);
            }

            return status;
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy(
                "Ошибка при проверке бизнес-логики склада",
                exception: ex);
        }
    }
}
