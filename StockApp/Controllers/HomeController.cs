using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using StockApp.Models;
using StockApp.ServiceContracts;

namespace StockApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly IFinnhubService _finnhubService;
        private readonly IConfiguration _configuration;
        private readonly TradingOption _tradingOptions;

        public HomeController(IFinnhubService finnhubService, IOptions<TradingOption> tradingOptions,IConfiguration configuration)
        {
            _finnhubService = finnhubService;
            this._configuration = configuration;
            _tradingOptions = tradingOptions.Value;
        }
        [Route("/")]
        public async Task<IActionResult> Index()
        {
            if (string.IsNullOrEmpty(_tradingOptions.DefaultStockSymbol))
            {
                _tradingOptions.DefaultStockSymbol = "MSFT";
            }
            //Get company profile from API server 
            Dictionary<string, object> companyProfileDictionary = await _finnhubService.GetCompanyProfile(_tradingOptions.DefaultStockSymbol);
            //Get stock price quotes from API service
            Dictionary<string, object> stockQuoteDictionary = await _finnhubService.GetStockPriceQuote(_tradingOptions.DefaultStockSymbol);
            StockTrade stockTrade = new StockTrade
            {
                StockSymbol = _tradingOptions.DefaultStockSymbol
            };
            //load data from finnhubservice
            if (companyProfileDictionary!=null && stockQuoteDictionary != null)
            {
                stockTrade = new StockTrade() { StockSymbol= Convert.ToString(companyProfileDictionary["ticker"]),
                    StockName = Convert.ToString(companyProfileDictionary["name"]),
                    Price = Convert.ToDouble(stockQuoteDictionary["c"].ToString())
                };
            }
            ViewBag.FinnhubToken = _configuration["FinnhubToken"];

            return View(stockTrade);
        }
    }
}
