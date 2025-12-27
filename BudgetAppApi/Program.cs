using BudgetAppApi.Data;
using BudgetAppApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using Scalar.AspNetCore;
using System.Security.Cryptography;
using System.Text;
namespace BudgetAppApi
{
    public class Program
    {
        public static string HashPassword(string password)
        {
            byte[] salt = new byte[128 / 8];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            return Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 256 / 8
            ));
        }
        public static void Main(string[] args)
        {

            var builder = WebApplication.CreateBuilder(args);
            var key = builder.Configuration["Jwt:Key"];
            var issuer = builder.Configuration["Jwt:Issuer"];
            var audience = builder.Configuration["Jwt:Audience"];
            builder.Services.AddSingleton<JwtService>();

            builder.Services.AddAuthentication("Bearer")
             .AddJwtBearer("Bearer", options =>
             {
                 options.RequireHttpsMetadata = false;
                 options.TokenValidationParameters = new TokenValidationParameters
                 {
                     ValidateIssuer = true,
                     ValidateAudience = true, // <- temporarily ignore audience
                                              
                     ValidateIssuerSigningKey = true,
                     ValidIssuer = builder.Configuration["Jwt:Issuer"],
                     ValidAudience = builder.Configuration["Jwt:Audience"],
                     IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
                 };

             });
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAngularApp",
                    policy => policy
                        .WithOrigins("http://localhost:4200")
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                            .AllowCredentials());
            });
 
            builder.Services.AddDbContext<BudgetAppApiContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("BudgetAppApiContext") ?? throw new InvalidOperationException("Connection string 'BudgetAppApiContext' not found.")));
            builder.Services.AddControllers();

            // Add services to the container.
            builder.Services.AddAuthorization();

            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            builder.Services.AddEndpointsApiExplorer();


            var app = builder.Build();

            app.UseAuthentication();

            app.UseAuthorization();
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.MapScalarApiReference();
            }

            using (var scope = app.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<BudgetAppApiContext>();
                context.Database.EnsureCreated();

                if (!context.User.Any())
                {
                    var user = new User
                    {
                        Username = "a",
                        Email = "a@example.com",
                        PasswordHash = HashPassword("123")
                    };
                    context.User.Add(user);
                    context.SaveChanges();
                }
            }

            //app.UseHttpsRedirection();
            app.UseCors("AllowAngularApp");

            app.UseAuthentication();

            app.UseAuthorization();

            app.MapControllers();
            app.Run();


        }
    }
}
