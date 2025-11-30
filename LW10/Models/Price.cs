using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StockApp.Models
{
    /// <summary>
    /// представляет цену акции в определённую дату
    /// </summary>
    public class Price
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey(nameof(Ticker))]
        public int TickerId { get; set; }

        public double PriceValue { get; set; }

        public DateTime Date { get; set; }

        public Ticker Ticker { get; set; } = null!;
    }
}
