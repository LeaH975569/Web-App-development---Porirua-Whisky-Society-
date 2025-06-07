using PWS.Models;

namespace PWS.ViewModels
{
    public class TastingResponseViewModel
    {
        public IEnumerable<TastingItem> TastingItems { get; set; }
        public IEnumerable<Whiskey> Whiskeys { get; set; }

        public TastingResponse TastingResponses { get; set; }

    }
}
