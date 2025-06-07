using Microsoft.AspNetCore.Mvc;
using PWS.Models;

namespace PWS.Controllers
{
    public class GalleryController : Controller
    {
        public IActionResult Index()
        {
            var galleryPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "GalleryPics");
            var imageFiles = Directory.EnumerateFiles(galleryPath)
                                      .Select(fileName => new GalleryImage
                                      {
                                          FileName = Path.GetFileName(fileName),
                                          FilePath = "/GalleryPics/" + Path.GetFileName(fileName)
                                      })
                                      .ToList();

            return View(imageFiles);
        }
    }
}
