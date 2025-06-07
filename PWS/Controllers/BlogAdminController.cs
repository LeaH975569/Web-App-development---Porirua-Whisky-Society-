using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PWS.Data;
using PWS.Models;
using PWS.Services;

namespace PWS.Controllers
{
    [Authorize(Roles = "Admin")]
    public class BlogAdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UploadManager _uploadManager;

        public BlogAdminController(ApplicationDbContext context)
        {
            _context = context;
            _uploadManager = new UploadManager(this);
        }

        // GET: Blogs
        public async Task<IActionResult> Index()
        {
            return View(
                await _context.Blogs
                .Select(x => new Blog { Id = x.Id, Title = x.Title, Summary = x.Summary, PublishedDate = x.PublishedDate, UpdatedDate = x.UpdatedDate,IsPublished = x.IsPublished })
                .OrderBy(b => b.PublishedDate)
                .ToListAsync()
                );
        }

        // GET: Blogs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var blog = await _context.Blogs
                .FirstOrDefaultAsync(m => m.Id == id);
            if (blog == null)
            {
                return NotFound();
            }

            return View(blog);
        }

        // GET: Blogs/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Blogs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Content,Summary,IsPublished,PublishedDate,UpdatedDate,ImageFile")] Blog blog)
        {
            blog.PublishedDate = DateTime.Now;
            blog.UpdatedDate = DateTime.Now;

            if (blog.ImageFile != null && ModelState.IsValid)
                _uploadManager.CheckImageFileState(blog.ImageFile);


            if (ModelState.IsValid)
            {
                _context.Add(blog);
                await _context.SaveChangesAsync();
                if (blog.ImageFile != null)
                {
                    blog.ImageUrl = await _uploadManager.AsyncSingleFileUpload(blog.ImageFile, blog.Id.ToString()); // Fun fact, after adding the item to the DB, EF will hydrate the model with ID. Thats why we can use it here.

                    await _context.SaveChangesAsync(); // Not prefered to have another update, but I don't know an easier way to get the generated id of a newly created item without creating it first
                }

                return RedirectToAction(nameof(Index));
            }
            return View(blog);
        }

        // GET: Blogs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var blog = await _context.Blogs.FindAsync(id);
            if (blog == null)
            {
                return NotFound();
            }
            return View(blog);
        }

        // POST: Blogs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Content,Summary,IsPublished,PublishedDate,ImageFile")] Blog blog)
        {
            if (id != blog.Id)
            {
                return NotFound();
            }

            blog.UpdatedDate = DateTime.Today;

            // Check if this is the first time the blog is being published, update publish date
            if (!_context.Blogs.AsNoTracking().Where(b => id == b.Id).FirstOrDefault()!.IsPublished && blog.IsPublished)
            {
                blog.PublishedDate = DateTime.Today;
            }

            if (blog.ImageFile != null && ModelState.IsValid)
                _uploadManager.CheckImageFileState(blog.ImageFile);

            if (ModelState.IsValid)
            {
                if (blog.ImageFile != null)
                    blog.ImageUrl = await _uploadManager.AsyncSingleFileUpload(blog.ImageFile, blog.Id.ToString());

                try
                {
                    _context.Update(blog);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BlogExists(blog.Id))
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
            return View(blog);
        }

        // GET: Blogs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var blog = await _context.Blogs
                .FirstOrDefaultAsync(m => m.Id == id);
            if (blog == null)
            {
                return NotFound();
            }

            return View(blog);
        }

        // POST: Blogs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var blog = await _context.Blogs.FindAsync(id);
            if (blog != null)
            {
                _uploadManager.DeleteUploadedImage(blog.ImageUrl);
                _context.Blogs.Remove(blog);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BlogExists(int id)
        {
            return _context.Blogs.Any(e => e.Id == id);
        }
    }
}
