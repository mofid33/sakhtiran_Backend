using System;
using System.Collections.Generic;

namespace MarketPlace.API.Data.Models
{
    public partial class TGoodsSurveyQuestions
    {
        public TGoodsSurveyQuestions()
        {
            TGoodsSurveyAnswers = new HashSet<TGoodsSurveyAnswers>();
        }

        public int QueId { get; set; }
        public int FkCategoryId { get; set; }
        public string QuestionText { get; set; }

        public virtual TCategory FkCategory { get; set; }
        public virtual ICollection<TGoodsSurveyAnswers> TGoodsSurveyAnswers { get; set; }
    }
}
