using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PWS.Models;
using PWS.Data;

namespace PWS.Controllers
{
    public class BlogController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BlogController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Blogs
        public async Task<IActionResult> Index()
        {
            var blogs = _context.Blogs.Where(b => b.IsPublished)
                .Select(x => new Blog { Id = x.Id, Title = x.Title, Summary = x.Summary, PublishedDate = x.PublishedDate, UpdatedDate = x.UpdatedDate, ImageUrl = x.ImageUrl }) // <= This sped up loading by a bunch
                .OrderByDescending(x => x.PublishedDate); 

            return View(await blogs.ToListAsync());
        }

        //public IActionResult Index()
        //{
        //    return View();
        //}
        [Route("/Blog/{id:int}")]
        public IActionResult BlogEntry(int id)
        {
            var blog = _context.Blogs.Where(b => b.Id == id && b.IsPublished)
                .FirstOrDefault();

            if (blog == null)
                return NotFound();

            return View(blog);
        }
    }
}
