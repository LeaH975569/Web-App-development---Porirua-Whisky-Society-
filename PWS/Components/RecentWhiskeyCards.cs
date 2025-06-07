using Microsoft.AspNetCore.Mvc;
using PWS.Data;
using PWS.Services;

namespace PWS.Components
{
    public class RecentWhiskeyCards : ViewComponent
    {
        private readonly ApplicationDbContext _context;

        public RecentWhiskeyCards(ApplicationDbContext context)
        {
            _context = context;
        }

        public IViewComponentResult Invoke(int amountToShow = 5)
        {
            var Whiskeys = _context.CompleteWhiskey().OrderByDescending(w => w.TastedDate).Take(amountToShow);
            return View(Whiskeys);
        }
    }
}
