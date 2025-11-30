using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using StockApp.Data;
using StockApp.Models;

namespace StockApp.Services
{
    /// <summary>
    /// выполняет загрузку, сохранение и анализ данных по тикерам
    /// </summary>
    public static class TickerProcessor
    {
        private static readonly SemaphoreSlim semaphore = new(5);
        private static readonly string apiToken =
            "TOKEN";

        /// <summary>
        /// обрабатывает один тикер: загружает данные, сохраняет цены и анализирует состояние
        /// </summary>
        /// <param name="ticker">символ тикера</param>
        /// <param name="fromDate">дата начала периода</param>
        /// <param name="toDate">дата окончания периода</param>
        /// <returns>задача выполнения обработки тикера</returns>
        /// <exception cref="HttpRequestException">возникает при ошибках API</exception>
        public static async Task ProcessTickerAsync(string ticker, string fromDate, string toDate)
        {
            await semaphore.WaitAsync();
            try
            {
                using var db = new StockDbContext();

                var tickerEntity = db.Tickers.FirstOrDefault(t => t.Symbol == ticker)
                                   ?? CreateTicker(db, ticker);

                await LoadPricesAsync(db, tickerEntity, fromDate, toDate);
                AnalyzeLastTwoPrices(db, tickerEntity);
            }
            finally
            {
                semaphore.Release();
                await Task.Delay(300);
            }
        }

        private static Ticker CreateTicker(StockDbContext db, string ticker)
        {
            var entity = new Ticker { Symbol = ticker };
            db.Tickers.Add(entity);
            db.SaveChanges();
            return entity;
        }

        private static async Task LoadPricesAsync(StockDbContext db, Ticker ticker, string from, string to)
        {
            string url =
                $"https://api.marketdata.app/v1/stocks/candles/D/{ticker.Symbol}/?format=json&token={apiToken}&from={from}&to={to}";

            using HttpClient client = new();
            var response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();

            string json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            if (!root.TryGetProperty("s", out var status) || status.GetString() != "ok")
                return;

            var times = root.GetProperty("t");
            var highs = root.GetProperty("h");
            var lows = root.GetProperty("l");

            for (int i = 0; i < times.GetArrayLength(); i++)
            {
                double price = (highs[i].GetDouble() + lows[i].GetDouble()) / 2;
                long unix = times[i].GetInt64();
                DateTime date = DateTimeOffset.FromUnixTimeSeconds(unix).UtcDateTime;

                db.Prices.Add(new Price
                {
                    TickerId = ticker.Id,
                    PriceValue = price,
                    Date = date
                });
            }

            db.SaveChanges();
        }

        private static void AnalyzeLastTwoPrices(StockDbContext db, Ticker ticker)
        {
            var lastTwo = db.Prices
                .Where(p => p.TickerId == ticker.Id)
                .OrderByDescending(p => p.Date)
                .Take(2)
                .ToList();

            if (lastTwo.Count < 2) return;

            string state = lastTwo[0].PriceValue > lastTwo[1].PriceValue
                ? "выросла"
                : lastTwo[0].PriceValue < lastTwo[1].PriceValue
                    ? "упала"
                    : "не изменилась";

            db.TodaysConditions.Add(new TodaysCondition
            {
                TickerId = ticker.Id,
                State = state
            });

            db.SaveChanges();
            Console.WriteLine($"Акция {ticker.Symbol} {state}");
        }
    }
}
