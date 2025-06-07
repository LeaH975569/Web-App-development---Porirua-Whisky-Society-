using Microsoft.EntityFrameworkCore;
using PWS.Data;
using PWS.Models;
using PWS.Models.ViewModels;
using static Azure.Core.HttpHeader;

namespace PWS.Services
{
    public static class DBExtensions
    {
        #region TasingItemMethods
        public static IQueryable<TastingItem> CompleteTastingItem(this ApplicationDbContext context)
        {
            return context.TastingItems.Include(x => x.Whiskey).Include(x => x.Survey);
        }
        public static TastingItem? TastingItemById(this ApplicationDbContext context, int? id)
        {
            return context.TastingItems.Include(x => x.Whiskey).Include(x => x.Survey).Include(x => x.TastingResponses).FirstOrDefault(x => x.Id == id);
        }
        #endregion
        public static IQueryable<int> GetAllWhiskeyYears(this ApplicationDbContext context)
        {
            return context.Whiskeys.GroupBy(w => w.TastedDate).Where(date => date.Key != null && date.Key.Value.Year > 1).Select(g => g.Key.Value.Year).Distinct();
        }
        public static Whiskey WhiskeyById(this ApplicationDbContext context, int? id)
        {
            return context.Whiskeys.Include(x => x.Tastings.Where(x => x.Survey.Published)).ThenInclude(x => x.TastingResponses)
                .Include(x => x.Tastings.Where(x => x.Survey.Published)).ThenInclude(x => x.Survey).Where(x => x.WhiskeyId == id).FirstOrDefault();
        }
        public static IQueryable<Whiskey> CompleteWhiskey(this ApplicationDbContext context)
        {
            return context.Whiskeys.Include(x => x.Tastings.Where(x => x.Survey.Published)).ThenInclude(x => x.TastingResponses)
                .Include(x => x.Tastings.Where(x => x.Survey.Published)).ThenInclude(x => x.Survey);
        }
        public static IQueryable<Whiskey> CompleteWhiskeyScore(this ApplicationDbContext context, float scoreMin,float scoreMax)
        {
            return context.Whiskeys.Include(x => x.Tastings.Where(x => x.Survey.Published)).ThenInclude(x => x.TastingResponses)
                .Include(x => x.Tastings.Where(x => x.Survey.Published)).ThenInclude(x => x.Survey)
                .Where(w => w.TotalScore >= scoreMin && w.TotalScore <= scoreMax);
        }

        public static List<TastingResponse> ResponseBySession(this ApplicationDbContext context, String sessionId, int id)
        {
            var survey = context.SurveyFromTastingItem(id);
            if (survey != null && survey.IsOpen())
            {
                var response = context.TastingResponses.Include(x => x.TastingItem).Include(x => x.WhiskeyGuess).Where(x => x.SessionId == sessionId && x.TastingItem.Id == id).ToList();
                if (response.Count < 1)
                {
                    var newResponse = new TastingResponse { SessionId = sessionId, TastingItem = context.TastingItems.Find(id) };
                    var name = context.TastingResponses.FirstOrDefault(x => x.SessionId == sessionId && x.UserName.Length > 0)?.UserName;
                    if (name != null) { newResponse.UserName = name; }
                    context.TastingResponses.Add(newResponse);
                    context.SaveChanges();
                    response.Add(newResponse);
                }
                return response;
            }
            return new List<TastingResponse>();
        }
        public static bool SaveTastingResponse(this ApplicationDbContext context, TastingResponseSave response)
        {
            var survey = context.SurveyFromTastingItem(response.TastingItem);
            if (survey != null && survey.IsOpen())
            {
                var tasting = context.TastingResponses.Where(x => x.SessionId == response.SessionId && x.Id == response.Id).FirstOrDefault();
                if(tasting != null)
                {
                    tasting.SessionId = response.SessionId;
                    tasting.TastingItem = context.TastingItems.Find(response.TastingItem);
                    tasting.Aroma = response.Aroma;
                    tasting.Finish = response.Finish;
                    tasting.Taste = response.Taste;
                    tasting.Notes = response.Notes;
                    tasting.WhiskeyGuess = response.WhiskeyGuess > 0 ? context.Whiskeys.Find(response.WhiskeyGuess) : null;
                    tasting.Id = response.Id;
                    tasting.UserName = response.UserName;

                                    // dont include the response being replaces otherwise there is a tracking error
                    var tastings = context.TastingResponses.Where(x => x.SessionId == response.SessionId && x.Id != response.Id).ToList();
                    tastings.ForEach(x => x.UserName = response.UserName);
                    context.UpdateRange(tastings);
                    context.Update(tasting);
                    context.SaveChanges();
                    return true;
                };
                return false;

            }
            else return false;

        }
        #region SurveyMethods
        public static List<Whiskey> WhiskeysInSurvey(this ApplicationDbContext context, int surveyId)
        {
            return context.Surveys.Where(x => x.Id == surveyId).SelectMany(x => x.Tastings).Select(x => x.Whiskey).ToList();
        }

        //get currently published surveys by a year number sorted by date and include all child objects so they can be used for calculations 
        public static List<Survey> SurveyByYear(this ApplicationDbContext context, int year)
        {
            return context.Surveys.Where(x => x.End.Year == year && x.Published).OrderBy(x => x.End)
                .Include(x => x.Tastings).ThenInclude(x => x.TastingResponses)
                .Include(x => x.Tastings).ThenInclude(x => x.Whiskey).ToList();
        }
        // gets a list of years that contain published surveys for use in constructing menus 
        public static List<int> GetAllSurveyYears(this ApplicationDbContext context)
        {
            return context.Surveys.Where(x => x.Published).GroupBy(w => w.End).Select(g => g.Key.Year).Distinct().ToList();
        }

        public static Survey? SurveyByUuid(this ApplicationDbContext context, string? uuid)
        {
            return context.Surveys.Include(x => x.Tastings).ThenInclude(x => x.Whiskey).FirstOrDefault(x => x.Uuid == uuid);
        }
        public static Survey? SurveyById(this ApplicationDbContext context, int? id)
        {
            return context.Surveys.Include(x => x.Tastings).ThenInclude(x => x.Whiskey)
                .Include(x => x.Tastings).ThenInclude(x => x.TastingResponses).FirstOrDefault(x => x.Id == id);
        }
        public static Survey? SurveyFromTastingItem(this ApplicationDbContext context, int? id)
        {
            var survey = context.TastingItems.Include(x => x.Survey).FirstOrDefault(x => x.Id == id)?.Survey;
            return survey;
        }
        #endregion
    }
}
