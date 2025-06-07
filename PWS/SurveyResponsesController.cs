using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PWS.Data;
using PWS.Models;

namespace PWS
{
    public class SurveyResponsesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SurveyResponsesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: SurveyResponses
        public async Task<IActionResult> SurveyResponsesIndex()
        {
            return View(await _context.TastingResponses.ToListAsync());
        }



        // GET: SurveyResponses/Details/5
        public async Task<IActionResult> SurveyResponsesDetails(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tastingResponse = await _context.TastingResponses
                .FirstOrDefaultAsync(m => m.Id == id);

            if (tastingResponse == null)
            {
                return NotFound();
            }

            return View(tastingResponse);
        }

        // GET: SurveyResponses/Create
        public IActionResult SurveyResponsesCreate()
        {
            return View();
        }

        // POST: SurveyResponses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SurveyResponsesCreate([Bind("Id,SessionId,UserName,Aroma,Taste,Finish,Notes")] TastingResponse tastingResponse)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tastingResponse);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(tastingResponse);
        }

        // GET: SurveyResponses/Edit/5
        public async Task<IActionResult> SurveyResponsesEdit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tastingResponse = await _context.TastingResponses.FindAsync(id);
            if (tastingResponse == null)
            {
                return NotFound();
            }
            return View(tastingResponse);
        }

        // POST: SurveyResponses/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SurveyResponsesEdit(int id, [Bind("Id,SessionId,UserName,Aroma,Taste,Finish,Notes")] TastingResponse tastingResponse)
        {
            if (id != tastingResponse.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tastingResponse);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TastingResponseExists(tastingResponse.Id))
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
            return View(tastingResponse);
        }

        // GET: SurveyResponses/Delete/5
        public async Task<IActionResult> SurveyResponsesDelete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tastingResponse = await _context.TastingResponses
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tastingResponse == null)
            {
                return NotFound();
            }

            return View(tastingResponse);
        }

        // POST: SurveyResponses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SurveyResponsesDeleteConfirmed(int id)
        {
            var tastingResponse = await _context.TastingResponses.FindAsync(id);
            if (tastingResponse != null)
            {
                _context.TastingResponses.Remove(tastingResponse);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TastingResponseExists(int id)
        {
            return _context.TastingResponses.Any(e => e.Id == id);
        }






    }
}
