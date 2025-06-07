using Microsoft.AspNetCore.Mvc;
using PWS.Data;
using PWS.Models.ViewModels;
using PWS.Services;

namespace PWS.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class TastingResponseController(ApplicationDbContext context) : ControllerBase
    {
        private readonly ApplicationDbContext _context = context;
        [HttpPost("Get")]
        public IActionResult GetResponses([FromBody] TastingResponseRequest req)
        {
            if (req.SessionId == null)
            {
                return NotFound();
            }
            var responses = _context.ResponseBySession(req.SessionId, req.TastingId);
            if (responses.Count > 0)
                return new JsonResult(responses);
            else return BadRequest();
        }
        [HttpPost("Save")]
        public IActionResult SaveResponse([FromBody] TastingResponseSave response)
        {
            if (response == null) return NotFound();
            ;
            return _context.SaveTastingResponse(response) ? Ok("saved") : BadRequest();
        }
    }
}
