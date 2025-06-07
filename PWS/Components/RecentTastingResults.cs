using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PWS.Data;
using PWS.Services;

namespace PWS.Components
{
    public class RecentTastingResults : ViewComponent
    {
        private readonly ApplicationDbContext _context;

        public RecentTastingResults(ApplicationDbContext context)
        {
            _context = context;
        }

        public IViewComponentResult Invoke(int amountToShow = 6)
        {
            var year =_context.GetAllSurveyYears().FirstOrDefault();
            var Survey = _context.SurveyByYear(year).OrderByDescending(s => s.End).Take(amountToShow).ToList();  
            return View(Survey);
        }
    }
}
 