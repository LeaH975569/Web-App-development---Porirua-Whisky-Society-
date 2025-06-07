using System.ComponentModel.DataAnnotations;

namespace PWS.Models
{
    public class TastingResponse
    {
        public int Id { get; set; }
        public required TastingItem? TastingItem { get; set; }
        public required string SessionId { get; set; }
        public String UserName { get; set; } = string.Empty;
        [Range(0, 100)]
        public float Aroma { get; set; } = 50;
        [Range(0, 100)]
        public float Taste { get; set; } = 50;
        [Range(0, 100)]
        public float Finish { get; set; } = 50;
        public string? Notes { get; set; }
        public Whiskey? WhiskeyGuess { get; set; }
    }
}
