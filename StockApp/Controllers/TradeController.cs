using Entities.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using ServiceContracts;
using StockApp.Filters.ActionFilters;
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
        private readonly ILogger<TradeController> _logger;
        private readonly TradingOption _tradingOptions;

        public TradeController(IFinnhubService finnhubService,IStockService stockService, IOptions<TradingOption> tradingOptions,IConfiguration configuration, ILogger<TradeController> logger)
        {
            _finnhubService = finnhubService;
            this._stockService = stockService;
            this._configuration = configuration;
            this._logger = logger;
            _tradingOptions = tradingOptions.Value;
        }
        [Route("[action]/{stockSymbol?}")]
        [Route("~/[controller]/{stockSymbol?}")]
        public async Task<IActionResult> Index(string StockSymbol)
        {
            _logger.LogInformation("In TradeController Index action method ");
            _logger.LogDebug("StockSymbol: {stockSymbol}",StockSymbol);
            if (string.IsNullOrEmpty(StockSymbol))
            {
                StockSymbol = "MSFT";
            }
            //Get company profile from API server 
            Dictionary<string, object> companyProfileDictionary = await _finnhubService.GetCompanyProfile(StockSymbol);
            //Get stock price quotes from API service
            Dictionary<string, object> stockQuoteDictionary = await _finnhubService.GetStockPriceQuote(StockSymbol);
            StockTrade stockTrade = new StockTrade
            {
                StockSymbol = StockSymbol
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
        [TypeFilter(typeof(CreateOrderActionFilter))]
        public async Task<IActionResult> BuyOrder(BuyOrderRequest buyOrderRequest)
        {
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
            BuyOrderResponse buyOrderResponse = await _stockService.CreateBuyOrder(buyOrderRequest);
            return RedirectToAction(nameof(Order));
        }

        [Route("[action]")]
        [HttpPost]
        [TypeFilter(typeof(CreateOrderActionFilter))]
        public async Task<IActionResult> SellOrder(SellOrderRequest sellOrderRequest)
        {
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
            SellOrderResponse sellOrderResponse = await _stockService.CreateSellOrder(sellOrderRequest);
            return RedirectToAction(nameof(Order));
        }

        [Route("[action]")]
        public async Task<IActionResult> Order()
        {
            Orders orders = new Orders()
            {
                BuyOrders= await _stockService.GetBuyOrders(),
                SellOrders=await _stockService.GetSellOrders(),
            };
            return View(orders);
        }
    }
}
