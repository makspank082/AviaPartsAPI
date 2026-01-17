using AviaPartsAPI.Data;
using AviaPartsAPI.Services.Commands;
using AviaPartsAPI.Services.Interfaces;
using AviaPartsAPI.Services.Queries;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using AviaPartsAPI.Services.HealthChecks;
using AviaPartsAPI.Middleware; 

namespace AviaPartsAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddControllers();

            builder.Services.AddScoped<IPartQueryService, PartQueryService>();
            builder.Services.AddScoped<IPartCommandService, PartCommandService>();

            builder.Services.AddScoped<DatabaseHealthCheck>();
            builder.Services.AddScoped<InventoryHealthCheck>();

            builder.Services.AddHealthChecks()
                .AddCheck<DatabaseHealthCheck>(
                    name: "PostgreSQL Database",
                    failureStatus: HealthStatus.Degraded,
                    tags: new[] { "database", "infrastructure", "ready" })
                .AddCheck<InventoryHealthCheck>(
                    name: "Inventory Business Logic",
                    failureStatus: HealthStatus.Degraded,
                    tags: new[] { "business", "inventory", "ready" });

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "AviaPartsAPI", Version = "v1" });
            });

            var app = builder.Build();

            app.UseMiddleware<ExceptionHandlingMiddleware>();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();

            app.MapControllers();

            app.MapHealthChecks("/health");


            app.MapHealthChecks("/ready", new HealthCheckOptions
            {
                Predicate = reg => reg.Tags.Contains("ready"),
                ResponseWriter = WriteHealthCheckResponse
            });


            app.MapHealthChecks("/health/detailed", new HealthCheckOptions
            {
                ResponseWriter = WriteHealthCheckResponse
            });

            app.MapGet("/", () => Results.Redirect("/swagger/index.html"))
                .ExcludeFromDescription();

            using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                dbContext.Database.Migrate();
            }

            app.Run();
        }

        private static Task WriteHealthCheckResponse(HttpContext context, HealthReport report)
        {
            context.Response.ContentType = "application/json; charset=utf-8";

            var response = new
            {
                Status = report.Status.ToString(),
                Timestamp = DateTime.UtcNow.ToString("o"),
                Duration = report.TotalDuration.TotalSeconds,
                Checks = report.Entries.Select(e => new
                {
                    Name = e.Key,
                    Status = e.Value.Status.ToString(),
                    Duration = e.Value.Duration.TotalSeconds,
                    Description = e.Value.Description,
                    Data = e.Value.Data,
                    Exception = e.Value.Exception?.Message
                })
            };

            return context.Response.WriteAsync(
                System.Text.Json.JsonSerializer.Serialize(response,
                    new System.Text.Json.JsonSerializerOptions
                    {
                        WriteIndented = true
                    }));
        }
    }
}