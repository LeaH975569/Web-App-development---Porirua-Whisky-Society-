using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PWS.Data;
using PWS.Models;
using PWS.Models.ViewModels;
using PWS.Services;

namespace PWS.Controllers
{
    [Authorize(Roles = "Admin")]
    public class WhiskeyAdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UploadManager _uploadManager;

        public WhiskeyAdminController(ApplicationDbContext context)
        {
            _context = context;
            _uploadManager = new UploadManager(this);
        }

        // GET: WhiskeyAdmin
        public async Task<IActionResult> Index(WhiskeyAdminViewModel? viewModel = null)
        {
            if (viewModel == null)
                viewModel = new();

            var whiskeys = _context.CompleteWhiskeyScore(viewModel.ScoreMin, viewModel.ScoreMax); ;

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

            viewModel.Whiskeys = await whiskeys.ToListAsync();
            viewModel.YearSelect = await _context.GetAllWhiskeyYears().ToListAsync();

            //await _context.Whiskeys.ToListAsync()
            return View(viewModel);
        }

        // GET: WhiskeyAdmin/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var whiskey = _context.WhiskeyById(id);
                
            if (whiskey == null)
            {
                return NotFound();
            }

            return View(whiskey);
        }

        // GET: WhiskeyAdmin/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: WhiskeyAdmin/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("WhiskeyId,WhiskeyName,WhiskeyDescription,WhiskeyScoreSetting,WhiskeyFinish,WhiskeyAroma,WhiskeyTaste,TotalScore,WhiskeyImageUrl,TastedDate,ImageFile")] Whiskey whiskey)
        {

            if (!whiskey.IsTastedDateVaild())
                ModelState.AddModelError(nameof(whiskey.TastedDate), "Missing Tasted Date");

            if (whiskey.ImageFile != null && ModelState.IsValid)
                _uploadManager.CheckImageFileState(whiskey.ImageFile);

            if (ModelState.IsValid)
            {
                _context.Add(whiskey);

                await _context.SaveChangesAsync();

                if (whiskey.ImageFile != null)
                {
                    whiskey.WhiskeyImageUrl = await _uploadManager.AsyncSingleFileUpload(whiskey.ImageFile, whiskey.WhiskeyId.ToString()); // Fun fact, after adding the item to the DB, EF will hydrate the model with ID. Thats why we can use it here.

                    await _context.SaveChangesAsync(); // Not prefered to have another update, but I don't know an easier way to get the generated id of a newly created item without creating it first
                }

                return RedirectToAction(nameof(Index));
            }
            return View(whiskey);
        }

        // GET: WhiskeyAdmin/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var whiskey = await _context.Whiskeys.FindAsync(id);
            if (whiskey == null)
            {
                return NotFound();
            }
            return View(whiskey);
        }

        // POST: WhiskeyAdmin/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("WhiskeyId,WhiskeyName,WhiskeyDescription,WhiskeyScoreSetting,UseSurveyResults,DoGenerateTotalScore,WhiskeyFinish,WhiskeyAroma,WhiskeyTaste,TotalScore,WhiskeyImageUrl,TastedDate,ImageFile")] Whiskey whiskey)
        {
            if (id != whiskey.WhiskeyId)
            {
                return NotFound();
            }

            if (whiskey.WhiskeyScoreSetting != WhiskeyScoreSetting.SurveyResults)
            {
                if (!whiskey.IsTastedDateVaild())
                {
                    ModelState.AddModelError(nameof(whiskey.TastedDate), "Missing Tasted Date");
                }
            }
            if (whiskey.ImageFile != null && ModelState.IsValid)
                _uploadManager.CheckImageFileState(whiskey.ImageFile);

            if (ModelState.IsValid)
            {
                if (whiskey.ImageFile != null)
                    whiskey.WhiskeyImageUrl = await _uploadManager.AsyncSingleFileUpload(whiskey.ImageFile, whiskey.WhiskeyId.ToString());

                try
                {
                    _context.Update(whiskey);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WhiskeyExists(whiskey.WhiskeyId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(whiskey);
        }

        // GET: WhiskeyAdmin/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var whiskey = await _context.Whiskeys
                .FirstOrDefaultAsync(m => m.WhiskeyId == id);
            if (whiskey == null)
            {
                return NotFound();
            }

            return View(whiskey);
        }

        // POST: WhiskeyAdmin/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var whiskey = await _context.Whiskeys.FindAsync(id);
            if (whiskey != null)
            {
                _context.RemoveRange(_context.TastingResponses.Where(x => x.TastingItem.Whiskey.WhiskeyId == id));
                _context.RemoveRange(_context.TastingItems.Where(x => x.Whiskey.WhiskeyId == id));
                _uploadManager.DeleteUploadedImage(whiskey.WhiskeyImageUrl);
                _context.Whiskeys.Remove(whiskey);
                _context.TastingItems.Where(x => x.Whiskey.WhiskeyId == id);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool WhiskeyExists(int id)
        {
            return _context.Whiskeys.Any(e => e.WhiskeyId == id);
        }
    }
}