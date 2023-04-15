using System;
using System.Collections.Generic;

namespace MarketPlace.API.Data.Models
{
    public partial class TShopSurveyQuestions
    {
        public TShopSurveyQuestions()
        {
            TShopSurveyAnswers = new HashSet<TShopSurveyAnswers>();
        }

        public int QueId { get; set; }
        public string QuestionText { get; set; }
        public bool Status { get; set; }

        public virtual ICollection<TShopSurveyAnswers> TShopSurveyAnswers { get; set; }
    }
}
