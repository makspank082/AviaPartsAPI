using Microsoft.Extensions.Diagnostics.HealthChecks;
using AviaPartsAPI.Data;
using Microsoft.EntityFrameworkCore;

namespace AviaPartsAPI.Services.HealthChecks
{
    public class DatabaseHealthCheck:IHealthCheck
    {
        private readonly AppDbContext _context;

        public DatabaseHealthCheck(AppDbContext context)
        {
            _context = context; 
        }

        public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext healthCheckContext, 
        CancellationToken cancellationToken = default) 
        {
            try
            {
                var canConnect = await _context.Database.CanConnectAsync(cancellationToken);

                if (!canConnect)
                    return HealthCheckResult.Unhealthy(
                        "Не удается подключиться к базе данных PostgreSQL");


                var partsCount = await _context.Parts.CountAsync(cancellationToken);


                var pendingMigrations = await _context.Database
                    .GetPendingMigrationsAsync(cancellationToken);

                var data = new Dictionary<string, object>
                {
                    ["total_parts"] = partsCount, 
                    ["has_pending_migrations"] = pendingMigrations.Any(), 
                    ["pending_migrations_count"] = pendingMigrations.Count() 
                };

                if (pendingMigrations.Any())
                {
                    return HealthCheckResult.Degraded(
                        $"База подключена, но есть {pendingMigrations.Count()} pending миграций",
                        data: data); 
                }


                return HealthCheckResult.Healthy(
                    $"База данных PostgreSQL подключена. Всего деталей: {partsCount}",
                    data); 
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy(
                    "Ошибка при проверке базы данных",
                    data: new Dictionary<string, object>
                    {
                        ["error"] = "Database check failed"
                    });
            }
        }
    }
}
