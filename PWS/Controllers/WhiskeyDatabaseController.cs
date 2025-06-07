using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PWS.Data;
using PWS.Models;
using PWS.Models.ViewModels;
using PWS.Services;

namespace PWS.Controllers
{
    /// <summary>
    /// TODO: Think of a better name rather than 'whisky database'
    /// TODO: Database connectivity & database seeding
    /// </summary>
    public class WhiskeyDatabaseController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly int _defaultAmountToShow = 10;

        public WhiskeyDatabaseController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Returns a view with whiskys that match the filter.
        /// SearchYear does not function!
        /// </summary>
        /// <param name="pagination">What page the user is currently looking at.</param>
        /// <param name="searchName"></param>
        /// <param name="searchYear"></param>
        /// <returns></returns>
        public async Task<IActionResult> Index(WhiskeyViewModel? viewModel = null)
        {
            if (viewModel == null)
                viewModel = new();

            IQueryable<Whiskey> whiskeys;
            // Filter
            if (ModelState.IsValid)
            {
                whiskeys = _context.CompleteWhiskeyScore(viewModel.ScoreMin,viewModel.ScoreMax);
                if (!string.IsNullOrEmpty(viewModel.SearchString))
                {
                    whiskeys = whiskeys.Where(w => w.WhiskeyName.Contains(viewModel.SearchString));
                }

                if (viewModel.SearchYear > 1)
                    whiskeys = whiskeys.Where(w => w.TastedDate != null & w.TastedDate.Value.Year == viewModel.SearchYear);

                switch (viewModel.SortMode)
                {
                    case SortMode.Name:
                        whiskeys = whiskeys.OrderBy(w => w.WhiskeyName);
                        break;
                    case SortMode.ScoreSetting:
                        whiskeys = whiskeys.OrderBy(w => w.TotalScore);
                        break;
                    case SortMode.Score:
                        whiskeys = whiskeys.OrderBy(w => w.WhiskeyScoreSetting);
                        break;
                    case SortMode.TastedDate:
                        whiskeys = whiskeys.OrderBy(w => w.TastedDate);
                        break;
                    default:
                        whiskeys = whiskeys.OrderBy(w => w.WhiskeyName);
                        break;
                }
                if (viewModel.OrderDes)
                    whiskeys = whiskeys.Reverse();
            }
            else
            {
                whiskeys = _context.CompleteWhiskey();
            }


            var totalItems = whiskeys.Count();

            if (viewModel.AmountSelected <= 0)
                viewModel.AmountSelected = _defaultAmountToShow;

            whiskeys = whiskeys.Skip(viewModel.AmountSelected * viewModel.CurrentPage).Take(viewModel.AmountSelected);

            viewModel.Whiskeys = await whiskeys.ToListAsync();
            viewModel.YearSelect = await _context.GetAllWhiskeyYears().ToListAsync();

            viewModel.TotalItems = totalItems;
            //viewModel = new WhiskeyViewModel() { Whiskeys = selected, SearchString = searchName, SearchYear = searchYear, TotalItems = totalItems, CurrentPage = pagination, AmountSelected = _defaultAmountToShow };

            return View(viewModel);
        }

        [Route("/{controller}/{id:int}")]
        public IActionResult Whiskey(int id)
        {
            //var Whiskeys = _context.Whiskeys;
            //var selected = Whiskeys.Where(w => w.WhiskeyId == id).FirstOrDefault();

            var selected = _context.WhiskeyById(id);

            var viewModel = new WhiskeyViewModel() { Whiskey = selected };

            // TODO: Make a custom not found page for whisky!
            if (viewModel.Whiskey == null)
                return NotFound();

            return View(viewModel);
        }
    }
}
