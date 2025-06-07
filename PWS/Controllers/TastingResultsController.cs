using Microsoft.AspNetCore.Mvc;
using PWS.Data;
using PWS.Services;

namespace PWS.Controllers
{
    public class TastingResultsController(ApplicationDbContext context) : Controller
    {
        private readonly ApplicationDbContext _context = context;

        public IActionResult Index(int year)
        {
            // logic for getting the correct surveys is all handled in the DbExtensions 
            ViewBag.year = year.ToString();
            return View(_context.SurveyByYear(year));
        }
    }
}
