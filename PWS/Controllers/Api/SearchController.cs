using Microsoft.AspNetCore.Mvc;
using PWS.Data;

namespace PWS.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class SearchController(ApplicationDbContext context) : ControllerBase
    {
        readonly ApplicationDbContext _context = context;

        public IActionResult SearchWhiskey([FromBody] string searchQuery)
        {

            return new JsonResult(searchQuery == string.Empty ? null : _context.Whiskeys.Where(x => x.WhiskeyName.Contains(searchQuery)).ToList());
        }
    }
}
