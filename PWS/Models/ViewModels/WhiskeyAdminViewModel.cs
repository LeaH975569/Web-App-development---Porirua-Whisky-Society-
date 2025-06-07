using System.ComponentModel.DataAnnotations;

namespace PWS.Models.ViewModels
{
    public enum SortMode
    {
        Name,
        ScoreSetting,
        Score,
        TastedDate
    }

    public class WhiskeyAdminViewModel
    {
        // Don't get rid of this! This is used for getting the display names for the whiskey fields
        public Whiskey? Whiskey { get; set; } = default;
        public IEnumerable<Whiskey>? Whiskeys { get; set; } = default;
        public List<int>? YearSelect { get; set; } = default;

        // OrderBy
        [Display(Name = "OrderBy")]
        public SortMode SortMode { get; set; } = SortMode.Name;
        [Display(Name = "Descending")]
        public bool OrderDes { get; set; } = false;

        // Filter
        [Display(Name = "Search Via Name")]
        public string? SearchString { get; set; } = "";
        [Display(Name = "Min Score")]
        [Range(0, 100)]
        public int ScoreMin { get; set; } = 0;
        [Display(Name = "Max Score")]
        [Range(0, 100)]
        public int ScoreMax { get; set; } = 100;
        [Display(Name = "Tasted Year")]
        public int SearchYear { get; set; } = -1;
    }
}
