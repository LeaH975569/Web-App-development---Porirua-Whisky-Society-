using PWS.Models;

namespace PWS.Services
{
    public static class TastingItemExtensions
    {
        public static float GetAverageScore(this TastingItem tastingItem)
        {
            if(tastingItem.TastingResponses.Count > 0)
            return tastingItem.TastingResponses.Average(x => (x.Taste + x.Aroma + x.Finish) / 3);
            return 0;
        }
        public static float GetAverageAromaScore(this TastingItem tastingItem)
        {
            if (tastingItem.TastingResponses.Count > 0)
                return tastingItem.TastingResponses.Average(x => x.Aroma);
            return 0;
        }
        public static float GetAverageFinishScore(this TastingItem tastingItem)
        {
            if (tastingItem.TastingResponses.Count > 0)
                return tastingItem.TastingResponses.Average(x =>  x.Finish);
            return 0;
        }
        public static float GetAverageTasteScore(this TastingItem tastingItem)
        {
            if (tastingItem.TastingResponses.Count > 0)
                return tastingItem.TastingResponses.Average(x => x.Taste);
            return 0;
        }
    }
}
