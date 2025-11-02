using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using BudgetAppApi.Data;
using BudgetAppApi.Models;

namespace BudgetAppApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddDbContext<BudgetAppApiContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("BudgetAppApiContext") ?? throw new InvalidOperationException("Connection string 'BudgetAppApiContext' not found.")));
            builder.Services.AddControllers();
            // Add services to the container.
            builder.Services.AddAuthorization();

            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();
            
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            using (var scope = app.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<BudgetAppApiContext>();
                context.Database.EnsureCreated();

                if (!context.User.Any())
                {
                    context.User.AddRange(
                        new User { Username = "Admin", Email = "a@example.com", Password = "ap" },
                        new User { Username = "User", Email = "u@example.com", Password = "up" });
                    context.SaveChanges();
                }
            }
            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();
            app.Run();
        }
    }
}
