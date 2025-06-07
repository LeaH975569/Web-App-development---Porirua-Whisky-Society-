using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using PWS.Data;
using PWS.Models;
using PWS.Services;

namespace PWS.Controllers
{
    public class SurveyController(ApplicationDbContext context) : Controller
    {
        private readonly ApplicationDbContext _context = context;

        public IActionResult Index(string? id, string? sessionId)
        {
            if (id.IsNullOrEmpty()) { return RedirectToAction("Index", "Home"); }
            if (sessionId.IsNullOrEmpty())
            {
                //set something on the session to actually keep it alive or asp.net generates a new one on every request
                HttpContext.Session.SetInt32("alive", 1);
                return RedirectToAction("Index", new { sessionId = HttpContext.Session.Id, id = id });
            }
            //return View(_context.Surveys.Include(x => x.Tastings).ThenInclude(x => x.Whiskey).First(x => x.Subtitle == "tt"));
            var Survey = _context.SurveyByUuid(id);
            if (Survey == null)
            {
                return RedirectToAction("Index", "Home");
            }
            if (Survey.IsOpen())
            {
                //Survey.AddResponses(_context, HttpContext.Session.Id);
                ViewBag.SessionId = sessionId;
                return View(Survey);
            }
            return RedirectToAction("Index", "Home");
        }

    }
}
