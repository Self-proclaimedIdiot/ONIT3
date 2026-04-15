using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
namespace MyProject.Server.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
       : base(options)
        {
            Database.EnsureCreated();
        }
        public virtual DbSet<Citizen> citizens { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Пытаемся взять строку из переменной окружения (которую передаст Docker)
            var connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION");

            // Если переменной нет (например, при локальном запуске вне Docker), 
            // можно оставить дефолтную для разработки, но для задания лучше брать всё из окружения
            if (string.IsNullOrEmpty(connectionString))
            {
                // Только для локальных тестов без докера
                connectionString = "Host=db;Port=5432;Database=ONIT;Username=postgres;Password=12345678";
            }

            optionsBuilder.UseNpgsql(connectionString);
        }
    }
}
