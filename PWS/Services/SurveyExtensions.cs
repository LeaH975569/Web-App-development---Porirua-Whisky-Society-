using PWS.Data;
using PWS.Models;

namespace PWS.Services
{
    public static class SurveyExtensions
    {
        public static bool IsOpen(this Survey survey)
        {
            if (DateTime.Now > survey.Start && DateTime.Now < survey.End)
            {
                return true;
            }
            return false;
        }
        public static void AddResponses(this Survey survey, ApplicationDbContext context, string sessionId)
        {
            foreach (TastingItem tasting in survey.Tastings)
            {
                if (!context.TastingResponses.Any(x => x.SessionId == sessionId && x.TastingItem.Id == tasting.Id))
                    context.TastingResponses.Add(new TastingResponse { SessionId = sessionId, TastingItem = tasting });
                context.SaveChanges();
            }
        }

    }
}
