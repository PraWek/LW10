using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using StockApp.Services;

namespace StockApp
{
    /// <summary>
    /// точка входа консольного приложения
    /// </summary>
    internal class Program
    {
        static async Task Main()
        {
            if (!File.Exists("ticker.txt"))
            {
                Console.WriteLine("Создайте файл ticker.txt с тикерами по одному в строке");
                return;
            }

            string[] tickers = await File.ReadAllLinesAsync("ticker.txt");

            string fromDate = new DateTime(2025, 1, 1).ToString("yyyy-MM-dd");
            string toDate = new DateTime(2025, 11, 19).ToString("yyyy-MM-dd");

            List<Task> tasks = new();
            foreach (var ticker in tickers)
            {
                string trimmed = ticker.Trim();
                if (trimmed.Length == 0) continue;
                tasks.Add(TickerProcessor.ProcessTickerAsync(trimmed, fromDate, toDate));
            }

            await Task.WhenAll(tasks);
            Console.WriteLine("Готово! Все данные сохранены и проанализированы");
        }
    }
}
