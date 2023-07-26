using Microsoft.AspNetCore.Mvc;
using ServiceContracts.DTO;

namespace StockApp.Controllers
{
    [Route("[controller]/[action]")]
    public class AccountController : Controller
    {
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Register(RegisterDTO registerDTO)
        {
            return RedirectToAction(nameof(TradeController.Index),"Trade");
        }
    }
}
