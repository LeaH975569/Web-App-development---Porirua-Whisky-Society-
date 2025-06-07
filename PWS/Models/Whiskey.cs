using PWS.Services;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;

namespace PWS.Models
{
    public enum WhiskeyScoreSetting
    {
        NotScored = -1,
        ManualTotal = 0,
        ManualSub = 1,
        SurveyResults = 2
    }

    /// <summary>
    /// TODO. Make get finish, aroma, taste and tasting date from survey results!
    /// </summary>
    public class Whiskey
    {
        /// <summary>
        /// Determines how much characters MiniDescription Returns
        /// </summary>
        [NotMapped]
        public static readonly int MiniDescriptionLength = 100;
        private DateTime? tastedDate;
        private int whiskeyFinish = 0;
        private int whiskeyAroma = 0;
        private int whiskeyTaste = 0;

        [Key]
        public int WhiskeyId { get; set; }
        [Required]
        public required string WhiskeyName { get; set; }
        public string? WhiskeyDescription { get; set; }
        public ICollection<TastingItem> Tastings { get; set; } = [];
        /// <summary>
        /// Based on 'MiniDescriptionLength', returns a substring of 'Description'
        /// </summary>
        public string MiniDescription
        {
            get
            {
                if (WhiskeyDescription == null)
                    return "";

                int count = WhiskeyDescription.Length > MiniDescriptionLength ? MiniDescriptionLength : WhiskeyDescription.Length;

                return WhiskeyDescription.Substring(0, count);
            }
        }

        // --- Scores ---
        #region Scoring
        [Display(Name = "Score Setting", Description = "Determines how the score is set")]
        public WhiskeyScoreSetting WhiskeyScoreSetting { get; set; }
        public bool HasBeenTasted
        {
            get
            {
                if (WhiskeyScoreSetting == WhiskeyScoreSetting.NotScored)
                    return false;

                return true;
            }
        }

        ///// <summary>
        ///// Determines if the score is based on the surveyResults or is manually set
        ///// </summary>
        [Range(0, 100)]
        [Display(Name = "Finish")]
        public int WhiskeyFinish
        {
            get
            {
                if (WhiskeyScoreSetting == WhiskeyScoreSetting.SurveyResults)
                {
                    float? score = Tastings.OrderByDescending(x => x.Survey.End)
                                        .FirstOrDefault()?
                                        .GetAverageFinishScore();
                    return score != null ? (int)score : 0;
                }
                else return whiskeyFinish;
            }
            set => whiskeyFinish = value; 
        }
        [Range(0, 100)]
        [Display(Name = "Aroma")]
        public int WhiskeyAroma {
            get
            {
                if (WhiskeyScoreSetting == WhiskeyScoreSetting.SurveyResults)
                {
                    float? score = Tastings.OrderByDescending(x => x.Survey.End)
                                        .FirstOrDefault()?
                                        .GetAverageAromaScore();
                    return score != null ? (int)score : 0;
                }
                else return whiskeyAroma;
            }
            set => whiskeyAroma = value; 
        }
        [Range(0, 100)]
        [Display(Name = "Taste")]
        public int WhiskeyTaste { 
            get 
            {
                if (WhiskeyScoreSetting == WhiskeyScoreSetting.SurveyResults)
                {
                    float? score = Tastings.OrderByDescending(x => x.Survey.End)
                                        .FirstOrDefault()?
                                        .GetAverageTasteScore();
                    return score != null ? (int)score : 0;
                }
                else return whiskeyTaste;
            } 
            set => whiskeyTaste = value; 
        }         // TODO: Doesn't appear within database!
        private int totalScore { get; set; } = 0;
        /// <summary>
        /// GET: DoGenerateTotalScore is true, then return a generated value based on totalScore, else get a manually set value.
        /// SET: set manual totalScore.
        /// </summary>
        [Range(0, 100)]
        [Display(Name = "Total Score")]
        public int TotalScore
        {
            get
            {
                if (WhiskeyScoreSetting == WhiskeyScoreSetting.ManualSub)
                    return (WhiskeyFinish + WhiskeyAroma + WhiskeyTaste) / 3;

                if (WhiskeyScoreSetting == WhiskeyScoreSetting.NotScored)
                    return 0;
                if (WhiskeyScoreSetting == WhiskeyScoreSetting.SurveyResults)
                {
                    float? score = Tastings.OrderByDescending(x => x.Survey.End)
                                        .FirstOrDefault()?
                                        .GetAverageScore();
                    return score != null ? (int)score : 0;
                }
                    

                return totalScore;
            }
            set
            {
                totalScore = value;
            }
        }
        #endregion

        private string? whiskeyImageUrl = "";
        // This is here to ensure image url are from the server and not anywhere else to stop possible cross site attacks.
        [RegularExpression(@"^(?!(ht|f)tp(s?)\:\/\/)*([a-zA-Z0-9\-\.\?\,\'\/\\\+&%\$#_ ]*)?$", ErrorMessage = "Must not be a URL from another site. Must link to a file path locally on the server!")]
        public string? WhiskeyImageUrl { 
            get { return whiskeyImageUrl; }
            set
            {
                if (value == null)
                {
                    whiskeyImageUrl = value;
                    return;
                }

                whiskeyImageUrl = value.Replace("\\", "/");
            } 
        }
        /// <summary>
        /// Used for uploading files
        /// </summary>
        [NotMapped]
        public IFormFile? ImageFile { get; set; }

        // TODO: Make it get the tasted date from the results!
        public DateTime? TastedDate 
        { get { 
                if (WhiskeyScoreSetting == WhiskeyScoreSetting.SurveyResults && Tastings.Count > 0) 
                {
                    return Tastings?.OrderByDescending(x => x.Survey.End).FirstOrDefault()?.Survey.End; 
                } 
                else return tastedDate; }
            set => tastedDate = value; 
        }

        /// <summary>
        /// Get string detailing the tasting details. Month, year, rank, and theme of the tasting
		/// TODO: Get rank from survey results!
        /// </summary>
        /// <returns>{Month} {Year} | Rank {RankNo} | Theme: {Theme}</returns>
        public string GetTasteDetailsString()
        {
            if (WhiskeyScoreSetting == WhiskeyScoreSetting.NotScored)
                return "Not Ranked";

            if (TastedDate == null)
                return "Not Specified";
            if (WhiskeyScoreSetting == WhiskeyScoreSetting.SurveyResults && Tastings.Count() > 0)
            {
                var t = Tastings?.OrderByDescending(x => x.Survey.End).FirstOrDefault();
                var s = t.Survey;
                var tis = t.Survey.Tastings.OrderByDescending(x => x.GetAverageScore()).ToList();
                var pos = tis.FindIndex(x => x == t) + 1;

                return $"{TastedDate?.ToString("MMMM yyyy", CultureInfo.InvariantCulture)} | Rank:{pos.ToString()} | {s.Title}";
            }

                // {Month} {Year} | Rank {RankNo} | Theme: {Theme}
                return $"{TastedDate?.ToString("MMMM yyyy", CultureInfo.InvariantCulture)}";
        }

        public string GetTasteDateString()
        {
            if (WhiskeyScoreSetting == WhiskeyScoreSetting.NotScored)
                return "--";

            return TastedDate?.ToString("MMMM yyyy", CultureInfo.InvariantCulture);
        }

        public string GetScoreString()
        {
            if (WhiskeyScoreSetting == WhiskeyScoreSetting.NotScored)
                return "--";

            return TotalScore.ToString();
        }
        // This maybe should be else where but we might want access to this
        public static List<string> GetAllWhiskeyImageURLS()
        {
            var urls = new List<string>();
            urls.AddRange(Directory.GetFiles("wwwroot/images/tasted_whiskey/"));
            urls = urls.Select(x => x.Replace("wwwroot", "")).ToList();
            return urls;
        }

        /// <summary>
        /// Check if TastedDate is ok to be null or not.
        /// </summary>
        /// <returns></returns>
        public bool IsTastedDateVaild()
        {
            if (WhiskeyScoreSetting == WhiskeyScoreSetting.NotScored || WhiskeyScoreSetting == WhiskeyScoreSetting.SurveyResults)
                return true;

            if (TastedDate == null)
                return false;

            return true;
        }
    }
}
