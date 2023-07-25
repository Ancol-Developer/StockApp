using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using ServiceContracts;
using StockApp.Models;
using StockApp.ServiceContracts;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace StockApp.Controllers
{
    [Route("[controller]")]
    public class StocksController : Controller
    {
        private readonly IFinnhubService _finnhubService;
        private readonly TradingOption _tradingoptions;

        public StocksController(IFinnhubService finnhubService, IOptions<TradingOption> tradingoptions)
        {
            _finnhubService = finnhubService;
            _tradingoptions = tradingoptions.Value;
        }
        [Route("/")]
        [Route("[action]/{stock?}")]
        [Route("~/[action]/{stock?}")]

        public async Task<IActionResult> Explore(string? stock, bool showAll = false)
        {
            // get company profile from API Server
            List<Dictionary<string, string>>? stocksDictionary = await _finnhubService.GetStocks();
            List<Stock> stocks = new List<Stock>();
            // filter the stocks
            if (stocksDictionary is not null)
            {
                if (!showAll && _tradingoptions.Top25PopularStocks != null)
                {
                    string[]? top25PopularStocksList = _tradingoptions.Top25PopularStocks.Split(',');
                    if (top25PopularStocksList is not null)
                    {
                        stocksDictionary = stocksDictionary.Where(temp => top25PopularStocksList.Contains(Convert.ToString(temp["symbol"]))).ToList();
                    }
                }
            }
            // convert dictionary object into Stock object
            stocks = stocksDictionary.Select(temp => new Stock()
            {
                StockName = Convert.ToString(temp["description"]),
                StockSymbol = Convert.ToString(temp["symbol"])
            }).ToList();
            ViewBag.stock = stock;
            return View(stocks);
        }
    }
}
