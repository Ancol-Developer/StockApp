using Entities.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using ServiceContracts;
using StockApp.Models;
using StockApp.ServiceContracts;

namespace StockApp.Controllers
{
    [Route("[controller]")]
    public class TradeController : Controller
    {
        private readonly IFinnhubService _finnhubService;
        private readonly IStockService _stockService;
        private readonly IConfiguration _configuration;
        private readonly TradingOption _tradingOptions;

        public TradeController(IFinnhubService finnhubService,IStockService stockService, IOptions<TradingOption> tradingOptions,IConfiguration configuration)
        {
            _finnhubService = finnhubService;
            this._stockService = stockService;
            this._configuration = configuration;
            _tradingOptions = tradingOptions.Value;
        }
        [Route("/")]
        [Route("[action]")]
        [Route("~/[controller]")]
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

        [Route("[action]")]
        [HttpPost]
        public IActionResult BuyOrder(BuyOrderRequest buyOrderRequest)
        {
            buyOrderRequest.DateAndTimeOfOrder = DateTime.Now;
            ModelState.Clear();
            TryValidateModel(buyOrderRequest);
            if(!ModelState.IsValid)
            {
                ViewBag.Errors= ModelState.Values.SelectMany(v => v.Errors).Select(v=>v.ErrorMessage).ToList();
                StockTrade trade = new StockTrade()
                {
                    StockName=buyOrderRequest.StockName, Price=buyOrderRequest.Price,
                    StockSymbol=buyOrderRequest.StockSymbol
                };
                return View("Index", trade);
            }
            BuyOrderResponse buyOrderResponse = _stockService.CreateBuyOrder(buyOrderRequest);
            return RedirectToAction(nameof(Order));
        }

        [Route("[action]")]
        [HttpPost]
        public IActionResult SellOrder(SellOrderRequest sellOrderRequest)
        {
            sellOrderRequest.DateAndTimeOfOrder = DateTime.Now;
            ModelState.Clear();
            TryValidateModel(sellOrderRequest);
            if (!ModelState.IsValid)
            {
                ViewBag.Errors = ModelState.Values.SelectMany(v => v.Errors).Select(v => v.ErrorMessage).ToList();
                StockTrade trade = new StockTrade()
                {
                    StockName = sellOrderRequest.StockName,
                    Price = sellOrderRequest.Price,
                    StockSymbol = sellOrderRequest.StockSymbol
                };
                return View("Index", trade);
            }
            SellOrderResponse sellOrderResponse = _stockService.CreateSellOrder(sellOrderRequest);
            return RedirectToAction(nameof(Order));
        }

        [Route("[action]")]
        public IActionResult Order()
        {
            Orders orders = new Orders()
            {
                BuyOrders=_stockService.GetBuyOrders(),
                SellOrders=_stockService.GetSellOrders(),
            };
            return View(orders);
        }
    }
}
