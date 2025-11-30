using Microsoft.EntityFrameworkCore;
using StockApp.Models;
using System.Collections.Generic;

namespace StockApp.Data
{
    /// <summary>
    /// контекст базы данных для работы с тикерами и ценами
    /// </summary>
    public class StockDbContext : DbContext
    {
        public DbSet<Ticker> Tickers { get; set; } = null!;
        public DbSet<Price> Prices { get; set; } = null!;
        public DbSet<TodaysCondition> TodaysConditions { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) =>
            optionsBuilder.UseSqlServer(
                "Server=user\\SQLEXPRESS;Database=StockDB;Trusted_Connection=True;Encrypt=False;");
    }
}
