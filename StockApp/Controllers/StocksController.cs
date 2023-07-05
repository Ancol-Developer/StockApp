using Microsoft.AspNetCore.Mvc;
using ServiceContracts;
using StockApp.ServiceContracts;

namespace StockApp.Controllers
{
    [Route("[controller]")]
    public class StocksController : Controller
    {
        private readonly IFinnhubService _finnhubService;
        private readonly IStockService _stockService;

        public StocksController(IFinnhubService finnhubService,IStockService stockService)
        {
            _finnhubService = finnhubService;
            _stockService = stockService;
        }
        [Route("")]
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
    }
}
