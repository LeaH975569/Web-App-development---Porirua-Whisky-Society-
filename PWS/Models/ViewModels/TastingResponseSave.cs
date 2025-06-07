namespace PWS.Models.ViewModels
{
    public class TastingResponseSave
    {
        public int Id { get; set; }
        public required int? TastingItem { get; set; }
        public required string SessionId { get; set; }
        public String UserName { get; set; } = string.Empty;
        public float Aroma { get; set; } = 25;
        public float Taste { get; set; } = 50;
        public float Finish { get; set; } = 66;
        public string? Notes { get; set; }
        public int WhiskeyGuess { get; set; }
    }
}
