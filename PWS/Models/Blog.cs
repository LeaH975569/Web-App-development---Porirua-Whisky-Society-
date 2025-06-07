using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Drawing;

namespace PWS.Models
{
    public class Blog
    {
        /// <summary>
        /// Careful, this does not apply retroactively and only applies to new notes!
        /// </summary>
        static public int SummaryCharacterCount = 250;

        public int Id { get; set; }
        [Required(ErrorMessage = "Blog Entry requires a title!")]
        [Display(Name = "Title")]
        [StringLength(50)]
        public string Title { get; set; } = string.Empty;
        [Required(ErrorMessage = "Blog Entry must have content!")]
        [Display(Name ="Blog Content")]
        public string Content { get; set; } = string.Empty;
        /// <summary>
        /// What gets displayed when in a list
        /// </summary>
        public string Summary { get; set; } = string.Empty;
        /// <summary>
        /// Determines if visible.
        /// </summary>
        public bool IsPublished { get; set; } = false;
        public DateTime PublishedDate { get; set; }
        public DateTime UpdatedDate { get; set; }

        private string imageUrl = "";
        public string ImageUrl 
        { 
            get {  return imageUrl; }
            set
            {
                imageUrl = value.Replace("\\", "/");
            }
        }
        [NotMapped]
        [Display(Name = "Blog Thumbnail")]
        public IFormFile? ImageFile { get; set; }
    }
}
