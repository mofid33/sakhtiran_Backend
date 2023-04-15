using System;
using System.Collections.Generic;

namespace MarketPlace.API.Data.Dtos.Survey
{
    //
    public class GoodsSurveyAnswersDto
    {
        public int AnsId { get; set; }
        public int? FkCommentId { get; set; }
        public int FkQuestionId { get; set; }
        public int FkCustomerId { get; set; }
        public int FkGoodsId { get; set; }
        public int AnsValue { get; set; } 

    }
}
