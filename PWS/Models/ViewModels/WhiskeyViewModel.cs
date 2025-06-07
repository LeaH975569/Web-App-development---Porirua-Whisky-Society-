using System.ComponentModel.DataAnnotations;

namespace PWS.Models.ViewModels
{
    public class WhiskeyViewModel
    {
        // -- Used for Whisky --
        public Whiskey? Whiskey { get; set; }

        // -- Used for Index / whisky list --
        public IEnumerable<Whiskey>? Whiskeys { get; set; }
        public List<int> YearSelect { get; set; } = new List<int>();

        // OrderBy
        [Display(Name = "OrderBy")]
        public SortMode SortMode { get; set; } = SortMode.TastedDate;
        [Display(Name = "Descending")]
        public bool OrderDes { get; set; } = true;

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

        // Pagination
        private int currentPage = 0;
        public int CurrentPage {
            get
            {
                return currentPage;
            }
            set
            {
                if (value < 0)
                    value = 0;

                currentPage = value;
            }
        }
        [Display(Name = "Items per page")]
        public int AmountSelected { get; set; } = 0;
        private int totalItems = 0;
        public int TotalItems { 
            get
            {
                return totalItems;
            }
            set 
            { 
                if (value < 0)
                    value = 0;

                totalItems = value;
            }
        }
        public int TotalPages
        {
            get // Consider storing this rather than calcuating it every time it's called
            {
                if (AmountSelected == 0)
                    return 0;

                return (int)Math.Ceiling((double)TotalItems / (double)AmountSelected);
            }
        }
    }
}
