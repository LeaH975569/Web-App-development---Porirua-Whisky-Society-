using Microsoft.AspNetCore.Mvc;
using PWS.Data;
using PWS.Services;

namespace PWS.Components
{
    public class SurveyYears(ApplicationDbContext context) : ViewComponent
    {
        private readonly ApplicationDbContext _context = context;

        public IViewComponentResult Invoke()
        {
            //DBextension to show all years that have published surveys in the dropdown menu
            return View(_context.GetAllSurveyYears());
        }
    }
}
