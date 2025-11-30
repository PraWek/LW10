using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StockApp.Models
{
    /// <summary>
    /// представляет торговый тикер
    /// </summary>
    public class Ticker
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Symbol { get; set; } = string.Empty;

        public List<Price> Prices { get; set; } = new();
        public List<TodaysCondition> Conditions { get; set; } = new();
    }
}
