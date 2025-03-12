using Microsoft.AspNetCore.Mvc;
using MVC_EduScanner.Models;
using System.Diagnostics;
using MVC_EduScanner.Services;
using System.Threading.Tasks;
using System.Collections.Generic;

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

        
        public async Task<IActionResult> AllPlans()
        {
            var updatedContent = await _scraper.SubmitForm();

            List <(string Link, string Name)> links = await _scraper.GetAllLinks();

            return View(links);
        }
        public async Task<IActionResult> ActivePlans()
        {
            var activePlans = await _scraper.GetActivePlansFromFile();
            return View(activePlans);

           
        }
        public async Task<IActionResult> UpdateInformationAboutActivePlans()
        {
            await _scraper.SubmitForm();
            var allPlans = await _scraper.GetAllLinks();
            var activePlans = await _scraper.GetActivePlansFromWebsite(allPlans);
            _scraper.SavePlansInFile(activePlans);
            return RedirectToAction("ActivePlans");
        }

        public async Task<IActionResult> TeacherLectures()
        {
            return View(_scraper.GetTeacherLecture());

        }
        public async Task<IActionResult> Index()
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
