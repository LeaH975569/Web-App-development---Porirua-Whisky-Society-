using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PWS.Data;
using PWS.Models;

namespace PWS.Controllers
{
    [Authorize(Roles = "Admin")]
    public class WhiskeysAdminBakController : Controller
    {
        private readonly ApplicationDbContext _context;

        public WhiskeysAdminBakController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: WhiskeysAdmin
        public async Task<IActionResult> Index()
        {
            return View(await _context.Whiskeys.ToListAsync());
        }

        // GET: WhiskeysAdmin/Details/5
        public async Task<IActionResult> Details(int? id)
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

        // GET: WhiskeysAdmin/Create
        public IActionResult Create()
        {
            ViewBag.imageList = new SelectList(GetImages());
            return View();
        }

        // POST: WhiskeysAdmin/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("WhiskeyId,WhiskeyName,WhiskeyDescription,WhiskeyFinish,WhiskeyAroma,WhiskeyTaste,WhiskeyImageUrl")] Whiskey whiskey)
        {
            if (ModelState.IsValid)
            {
                _context.Add(whiskey);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.imageList = new SelectList(GetImages(), GetImages().Where(x => x.Equals(whiskey.WhiskeyImageUrl)));
            return View(whiskey);
        }

        // GET: WhiskeysAdmin/Edit/5
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
            ViewBag.imageList = new SelectList(GetImages(), GetImages().Where(x => x.Equals(whiskey.WhiskeyImageUrl)));
            return View(whiskey);
        }

        // POST: WhiskeysAdmin/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("WhiskeyId,WhiskeyName,WhiskeyDescription,WhiskeyFinish,WhiskeyAroma,WhiskeyTaste,WhiskeyImageUrl")] Whiskey whiskey)
        {
            if (id != whiskey.WhiskeyId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
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
            ViewBag.imageList = new SelectList(GetImages(), GetImages().Where(x => x.Equals(whiskey.WhiskeyImageUrl)));
            return View(whiskey);
        }

        // GET: WhiskeysAdmin/Delete/5
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

        // POST: WhiskeysAdmin/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var whiskey = await _context.Whiskeys.FindAsync(id);
            if (whiskey != null)
            {
                _context.Whiskeys.Remove(whiskey);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool WhiskeyExists(int id)
        {
            return _context.Whiskeys.Any(e => e.WhiskeyId == id);
        }
        private List<string> GetImages()
        {
            var urls = new List<string>();
            urls.AddRange(Directory.GetFiles("wwwroot/images/Whiskey/"));
            urls = urls.Select(x => x.Replace("wwwroot", "")).ToList();
            return urls;
        }
    }
}
