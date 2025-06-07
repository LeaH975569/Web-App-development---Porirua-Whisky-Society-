using Microsoft.AspNetCore.Mvc;
using PWS.Controllers;

namespace PWS.Services
{
    public class UploadManager
    {
        private readonly string _WhiskeyUploadFilePath = "/images/tasted_whiskey_uploaded";
        private readonly string _BlogUploadFilePath = "/images/blog_thumbnail_uploaded";
        private readonly string _uploadFilePath = "";
        private readonly Controller _controller;

        /// <summary>
        /// Init with controller. The controller is used to set the model state and the file path used
        /// </summary>
        /// <param name="controller"></param>
        /// <exception cref="Exception"></exception>
        public UploadManager(Controller controller)
        {
            _controller = controller;

            if (controller is WhiskeyAdminController)
                _uploadFilePath = _WhiskeyUploadFilePath;
            else if (controller is BlogAdminController)
                _uploadFilePath = _BlogUploadFilePath;
            else
                throw new Exception("Unexpected controller provided!");
        }

        /// <summary>
        /// Checks if imageFile is vaild, adds ModelError to the ModelState if not.
        /// </summary>
        /// <param name="imageFile"></param>
        public void CheckImageFileState(IFormFile imageFile)
        {
            if (imageFile == null || imageFile.Length == 0)
            {
                _controller.ModelState.AddModelError("ImageFile", "File not selected");
            }

            var permittedExtensions = new[] { ".jpg", ".png" };
            var extension = Path.GetExtension(imageFile.FileName).ToLowerInvariant();

            if (string.IsNullOrEmpty(extension) || !permittedExtensions.Contains(extension))
            {
                _controller.ModelState.AddModelError("ImageFile", "Invalid file type.");
            }

            // Optional: Validate MIME type as well
            var mimeType = imageFile.ContentType;
            var permittedMimeTypes = new[] { "image/jpeg", "image/png" };
            if (!permittedMimeTypes.Contains(mimeType))
            {
                _controller.ModelState.AddModelError("ImageFile", "Invalid MIME type.");
            }

            //Validating the File Size
            if (imageFile.Length > 2000000) // Limit to 2 MB
            {
                _controller.ModelState.AddModelError("ImageFile", "The file is too large.");
            }
        }

        public async Task<string> AsyncSingleFileUpload(IFormFile imageFile, string fileName = "")
        {
            if (string.IsNullOrEmpty(fileName))
            {
                fileName = imageFile.FileName;
            }
            else
            {
                string extension = Path.GetExtension(imageFile.FileName).ToLowerInvariant();
                fileName = fileName + extension;
            }

            if (_controller.ModelState.IsValid)
            {
                // Delete already uploaded image. Not typically required as it will overwrite it ONLY IF THE EXTENSION IS THE SAME!
                string fileToDelete = Path.GetFileNameWithoutExtension(fileName);
                var permittedExtensions = new[] { ".jpg", ".png" };
                foreach (var ext in permittedExtensions)
                {
                    System.IO.File.Delete(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot" + _uploadFilePath, fileToDelete + ext));
                }

                string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot" + _uploadFilePath, fileName);
                //Using Buffering
                //using (var stream = System.IO.File.Create(filePath))
                //{
                //    // The file is saved in a buffer before being processed
                //    await SingleFile.CopyToAsync(stream);
                //}

                //Using Streaming
                using (var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                {
                    await imageFile.CopyToAsync(stream);
                }

                // Process the file here (e.g., save to the database, storage, etc.)
                return _uploadFilePath + "\\" + fileName;
            }

            return "";
        }

        /// <summary>
        /// Deletes file if it's path is '_uploadsFilePath'
        /// </summary>
        /// <param name="file"></param>
        public void DeleteUploadedImage(string file)
        {
            if (file == null)
                return;

            if (!file.Contains(_uploadFilePath))
                return;

            System.IO.File.Delete(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot" + file));
        }
    }
}
