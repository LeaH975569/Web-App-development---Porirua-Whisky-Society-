using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace PWS.Models
{
    public class TastingItem
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Please enter a name for the tasting item")]
        public string Name { get; set; }
        public string? Description { get; set; }
        public string? Uuid { get; set; }
        [Required(ErrorMessage = "Please select a Whiskey")]
        public Whiskey Whiskey { get; set; }
        [Required]
        public Survey Survey { get; set; }
        [JsonIgnore]
        public ICollection<TastingResponse> TastingResponses { get; set; } = [];

    }
}
