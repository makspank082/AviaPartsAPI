using AviaPartsAPI.Data;
using AviaPartsAPI.Services;
using AviaPartsAPI.Services.Commands;
using AviaPartsAPI.Services.Interfaces;
using AviaPartsAPI.Services.Queries;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

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

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "AviaPartsAPI", Version = "v1" });
            });

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();

            app.MapControllers();

            app.MapGet("/", () => Results.Redirect("/swagger/index.html"))
                .ExcludeFromDescription();
            app.Run();
        }
    }
}