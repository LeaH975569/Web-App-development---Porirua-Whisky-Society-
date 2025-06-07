using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace PWS.Models
{
    public class Survey
    {
        public int Id { get; set; }
        public string? Uuid { get; set; }
        [Length(1,50)]
        public string Title { get; set; } = string.Empty;
        [Length(0, 50)]
        public string? Subtitle { get; set; } = string.Empty;
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public bool Published { get; set; }
        [JsonIgnore]
        public ICollection<TastingItem> Tastings { get; set; } = [];
    }
}
