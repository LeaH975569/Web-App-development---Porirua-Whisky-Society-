using Microsoft.AspNetCore.Mvc;
using PWS.Data;
using PWS.Models;

namespace PWS.Components
{
    public class RecentBlog : ViewComponent
    {
        private readonly ApplicationDbContext _context;

        public RecentBlog(ApplicationDbContext context)
        {
            _context = context;
        }

        public IViewComponentResult Invoke()
        {
            var blog = _context.Blogs.Where(b => b.IsPublished).OrderByDescending(b => b.PublishedDate).Take(1).FirstOrDefault();
            
            if (blog == null)
                blog = new Blog { Title = "No blog to show!", Summary = "This is awkward. If you want something here, then create a blog!" };

            return View(blog);
        }
    }
}
