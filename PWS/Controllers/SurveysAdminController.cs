using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PWS.Data;
using PWS.Models;
using PWS.Services;
using PWS.ViewModels;
using QRCoder;



namespace PWS.Controllers
{
    [Authorize(Roles = "Admin")]
    public class SurveysAdminController(ApplicationDbContext context) : Controller
    {
        private readonly ApplicationDbContext _context = context;

        // GET: SurveysAdmin
        public async Task<IActionResult> Index()
        {
            return View(await _context.Surveys.ToListAsync());
        }
        // ---Survey Functions

        #region SurveyFunctions

        // GET: SurveysAdmin/Details/5
        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var survey = _context.SurveyById(id);
            if (survey == null)
            {
                return NotFound();
            }
            var location = new Uri($"{Request.Scheme}://{Request.Host}{Request.Path}{Request.QueryString}");

            var url = location.Scheme + "://" + location.Host + (location.Port == 443 || location.Port == 80 ? "" : ":" + location.Port);
            var surveyUrl = url.ToString() + "/Survey/Index/" + survey.Uuid;


            PayloadGenerator.Url generator = new(surveyUrl);
            QRCodeGenerator QrGen = new();
            QRCodeData qrdata = QrGen.CreateQrCode(generator, QRCodeGenerator.ECCLevel.Q);
            Base64QRCode b64 = new(qrdata);
            ViewBag.QR = b64.GetGraphic(5);
            ViewBag.URL = surveyUrl;

            return View(survey);
        }

        // GET: SurveysAdmin/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: SurveysAdmin/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,uuid,Title,Subtitle,Start,End,Published")] Survey survey)
        {
            survey.Uuid = System.Guid.NewGuid().ToString();
            if (ModelState.IsValid)
            {
                _context.Add(survey);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Edit), "SurveysAdmin", new { id = survey.Id });
            }
            return View(survey);
        }

        // GET: SurveysAdmin/Edit/5
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            if (id == 0)
            {
                return RedirectToAction(nameof(Index));
            }

            var survey = _context.SurveyById(id);
            if (survey == null)
            {
                return NotFound();
            }
            return View(survey);
        }

        // POST: SurveysAdmin/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Survey survey)
        {

            if (id != survey.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(survey);
                    _context.SaveChanges();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SurveyExists(survey.Id))
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
            return View(survey);
        }

        // GET: SurveysAdmin/Delete/5
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var survey = _context.SurveyById(id);
            if (survey == null)
            {
                return NotFound();
            }

            return View(survey);
        }

        // POST: SurveysAdmin/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var survey = _context.SurveyById(id);
            if (survey != null)
            {
                _context.RemoveRange(survey.Tastings.SelectMany(x => x.TastingResponses));
                _context.TastingItems.RemoveRange(survey.Tastings);
                _context.Surveys.Remove(survey);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        #endregion

        // ---Item Functions

        #region ItemFunctions

        public IActionResult ItemEdit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tastingItem = _context.TastingItemById(id);
            if (tastingItem == null)
            {
                return NotFound();
            }
            return View(tastingItem);
        }

        // POST: DeleteMe/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ItemEdit(int id, TastingItem tastingItem, int? whiskeyId)
        {
            if (id != tastingItem.Id)
            {
                return NotFound();
            }

            var dbItem = _context.TastingItemById(id);
            if (dbItem == null) return NotFound();

            if (dbItem.Whiskey.WhiskeyId != whiskeyId)
            {
                dbItem.Whiskey = _context.Whiskeys.First(x => x.WhiskeyId == whiskeyId);
            }

            dbItem.Name = tastingItem.Name;
            dbItem.Description = tastingItem.Description;
            ModelState.Clear();

            if (!TryValidateModel(dbItem, nameof(dbItem))) { return View(dbItem); }

            _context.Update(dbItem);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Edit), new { id = dbItem.Survey.Id });


        }
        public IActionResult ItemCreate(int? id)
        {
            if (id == null || _context.Surveys.Where(x => x.Id == id).IsNullOrEmpty()) { return NotFound(); }

            ViewBag.surveyId = id;
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ItemCreate([Bind("Uuid,Name,Description")] TastingItem ti, int surveyId, int whiskeyId)
        {
            var tastingItem = ti;
            tastingItem.Uuid = System.Guid.NewGuid().ToString();
            tastingItem.Survey = _context.Surveys.Where(_x => _x.Id == surveyId).First();
            tastingItem.Whiskey = _context.Whiskeys.Where(_x => _x.WhiskeyId == whiskeyId).FirstOrDefault();
            ModelState.Clear();

            var name = nameof(tastingItem);
            if (!TryValidateModel(tastingItem, name))
            {
                ViewBag.surveyId = surveyId;
                return View(tastingItem);
            }

            _context.Add(tastingItem);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Edit), new { id = surveyId });

        }


        public IActionResult ItemDelete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tastingItem = _context.TastingItemById(id);
            if (tastingItem == null)
            {
                return NotFound();
            }

            return View(tastingItem);
        }

        [HttpPost, ActionName("ItemDelete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ItemDeleteConfirmed(int id)
        {
            var tastingItem = _context.TastingItemById(id);
            int surveyId = 0;
            if (tastingItem != null)
            {
                surveyId = tastingItem.Survey.Id;
                _context.RemoveRange(tastingItem.TastingResponses);
                _context.TastingItems.Remove(tastingItem);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Edit), new { id = surveyId });
        }
        #endregion


        #region SurveyResponseFunctions
        // GET: SurveyResponses
        // Index + Sort and search functions
        public async Task<IActionResult> SurveyResponsesIndex(int surveyId, string? sortOrder, string? searchString, bool clearFilter = false, bool descending = false)
        {
            // Fetch the tasting items related to the surveyId
            var tastingItems = _context.SurveyById(surveyId)?.Tastings;
            if (tastingItems == null) return NotFound();

            //if (!tastingItems.Any())
            //{
            //    return NotFound();
            //}

            // Collect all tasting responses associated with these tasting items
            var tastingResponse = tastingItems.SelectMany(ti => ti.TastingResponses);

            ViewBag.SurveyId = surveyId;


            // Clear filter functionality
            if (clearFilter)
            {
                searchString = null;
                sortOrder = null;
                descending = false;
            }


            ViewBag.SearchString = searchString;
            ViewBag.SortOrder = sortOrder;
            ViewBag.Descending = descending;


            if (!string.IsNullOrEmpty(searchString))
            {
                string searchLower = searchString.ToLower();
                tastingResponse = tastingResponse.Where(ti =>
                    ti.UserName.ToLower().Contains(searchLower) ||
                    ti.TastingItem.Name.ToLower().Contains(searchLower));
            }


            switch (sortOrder)
            {
                case "userName":
                    tastingResponse = descending
                        ? tastingResponse.OrderByDescending(ti => ti.UserName)
                        : tastingResponse.OrderBy(ti => ti.UserName);
                    break;
                case "tastingItem":
                    tastingResponse = descending
                        ? tastingResponse.OrderByDescending(ti => ti.TastingItem.Name)
                        : tastingResponse.OrderBy(ti => ti.TastingItem.Name);
                    break;
                default:
                    break;
            }



            return View(tastingResponse.ToList());
        }


        // GET: SurveyResponses/Details/5
        public async Task<IActionResult> SurveyResponsesDetails(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tastingResponse = await _context.TastingResponses
                .Include(tr => tr.TastingItem).ThenInclude(x => x.Survey)
                .Include(x => x.WhiskeyGuess)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (tastingResponse == null)
            {
                return NotFound();
            }

            var location = new Uri($"{Request.Scheme}://{Request.Host}{Request.Path}{Request.QueryString}");

            var url = location.Scheme + "://" + location.Host + (location.Port == 443 || location.Port == 80 ? "" : ":" + location.Port);
            var surveyUrl = url.ToString() + "/Survey/Index/" + tastingResponse.TastingItem.Survey.Uuid + "?sessionId=" + tastingResponse.SessionId;


            PayloadGenerator.Url generator = new(surveyUrl);
            QRCodeGenerator QrGen = new();
            QRCodeData qrdata = QrGen.CreateQrCode(generator, QRCodeGenerator.ECCLevel.Q);
            Base64QRCode b64 = new(qrdata);
            ViewBag.QR = b64.GetGraphic(5);
            ViewBag.URL = surveyUrl;

            return View(tastingResponse);
        }

        // GET: SurveyResponses/Create
        public IActionResult SurveyResponsesCreate(int surveyId)
        {
            if (_context.Surveys.Where(x => x.Id == surveyId).IsNullOrEmpty())
            {
                return NotFound();
            }

            var viewModel = new TastingResponseViewModel
            {
                TastingItems = _context.TastingItems.Where(ti => ti.Survey.Id == surveyId).ToList(),
                Whiskeys = _context.WhiskeysInSurvey(surveyId)
            };
            ViewBag.SurveyId = surveyId;
            return View(viewModel);
        }

        // POST: SurveyResponses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SurveyResponsesCreate(TastingResponseViewModel tastingResponseView, int surveyId, int whiskeyId, int tastingId)
        {
            var tastingResponse = tastingResponseView.TastingResponses;
            tastingResponse.WhiskeyGuess = _context.Whiskeys.FirstOrDefault(x => x.WhiskeyId == whiskeyId);
            tastingResponse.TastingItem = _context.TastingItemById(tastingId);
            ModelState.Clear();
            if (TryValidateModel(tastingResponse))
            {
                var tastingItem = _context.TastingItemById(tastingId);
                if (tastingItem == null)
                {
                    return NotFound();
                }

                tastingResponse.TastingItem = tastingItem; // Associate the response with the fetched TastingItem
                tastingResponse.WhiskeyGuess = _context.Whiskeys.FirstOrDefault(x => x.WhiskeyId == whiskeyId);
                _context.Add(tastingResponse);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(SurveyResponsesIndex), new { surveyId = surveyId });
            }
            return View(tastingResponseView);
        }

        // GET: SurveyResponses/Edit/5
        public async Task<IActionResult> SurveyResponsesEdit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tastingResponse = await _context.TastingResponses
                .Include(tr => tr.TastingItem).ThenInclude(x => x.Survey) // Include TastingItem for context
                .FirstOrDefaultAsync(tr => tr.Id == id);

            if (tastingResponse == null)
            {
                return NotFound();
            }
            var surveyId = tastingResponse.TastingItem.Survey.Id;
            var tastingResponseView = new TastingResponseViewModel
            {
                TastingResponses = tastingResponse,
                TastingItems = _context.TastingItems.Where(ti => ti.Survey.Id == surveyId).ToList(),
                Whiskeys = _context.WhiskeysInSurvey(surveyId)

            };
            ViewBag.SurveyId = surveyId;
            return View(tastingResponseView);
        }

        // POST: SurveyResponses/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SurveyResponsesEdit(int id, TastingResponseViewModel tastingResponseView, int? whiskeyId, int tastingId, int surveyId)
        {
            var tastingResponse = tastingResponseView.TastingResponses;
            if (id != tastingResponse.Id || tastingResponse == null)
            {
                return NotFound();
            }
            tastingResponse.WhiskeyGuess = _context.Whiskeys.FirstOrDefault(x => x.WhiskeyId == whiskeyId);
            tastingResponse.TastingItem = _context.TastingItems.Include(x => x.Survey).Include(x => x.Whiskey).Where(x => x.Id == tastingId).FirstOrDefault();

            ModelState.Clear();
            if (TryValidateModel(tastingResponse))
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
                return RedirectToAction(nameof(SurveyResponsesIndex), new { surveyId = surveyId });
            }

            tastingResponseView.TastingItems = _context.TastingItems.Include(x => x.Survey).Where(ti => ti.Survey.Id == surveyId).ToList();
            tastingResponseView.Whiskeys = _context.WhiskeysInSurvey(surveyId);
            
            ViewBag.SurveyId = surveyId;
            return View(tastingResponseView);
        }

        // GET: SurveyResponses/Delete/5
        public async Task<IActionResult> SurveyResponsesDelete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tastingResponse = await _context.TastingResponses
              .Include(tr => tr.TastingItem).ThenInclude(x => x.Survey) // Ensure we include related TastingItem
              .Include(x => x.WhiskeyGuess)
              .FirstOrDefaultAsync(m => m.Id == id);

            if (tastingResponse == null)
            {
                return NotFound();
            }

            return View(tastingResponse);
        }

        // POST: SurveyResponses/Delete/5
        [HttpPost, ActionName("SurveyResponsesDelete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SurveyResponsesDeleteConfirmed(int id)
        {
            var tastingResponse = _context.TastingResponses.Include(x => x.TastingItem).ThenInclude(x => x.Survey).FirstOrDefault(x => x.Id == id);
            if (tastingResponse != null)
            {
                _context.TastingResponses.Remove(tastingResponse);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(SurveyResponsesIndex), new { surveyId = tastingResponse?.TastingItem.Survey.Id });
        }


        #endregion


        private bool SurveyExists(int id)
        {
            return _context.Surveys.Any(e => e.Id == id);
        }
        private bool TastingItemExists(int id)
        {
            return _context.TastingItems.Any(e => e.Id == id);
        }
        private bool TastingResponseExists(int id)
        {
            return _context.TastingResponses.Any(e => e.Id == id);
        }

    }
}
