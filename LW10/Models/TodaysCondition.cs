using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StockApp.Models
{
    /// <summary>
    /// описывает состояние цены акции за последний день
    /// </summary>
    public class TodaysCondition
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey(nameof(Ticker))]
        public int TickerId { get; set; }

        public string State { get; set; } = string.Empty;

        public Ticker Ticker { get; set; } = null!;
    }
}
