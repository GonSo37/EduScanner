using Microsoft.AspNetCore.Mvc;
using MVC_EduScanner.Models;
using System.Diagnostics;
using MVC_EduScanner.Services;
using System.Threading.Tasks;

namespace MVC_EduScanner.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly TimetableScraper _scraper;

        public HomeController(ILogger<HomeController> logger, TimetableScraper scraper)
        {
            _logger = logger;
            _scraper = scraper;
        }


        public async Task<IActionResult> ScraperResult()
        {
            var updatedContent = await _scraper.SubmitForm();
            List<string> links = await _scraper.RunAutomation();
            return View(links);
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
