using System;
using System.Collections.Generic;

namespace MarketPlace.API.Data.Models
{
    public partial class TGoodsSurveyAnswers
    {
        public int AnsId { get; set; }
        public int? FkCommentId { get; set; }
        public int FkQuestionId { get; set; }
        public int FkCustomerId { get; set; }
        public int FkGoodsId { get; set; }
        public int AnsValue { get; set; }

        public virtual TGoodsComment FkComment { get; set; }
        public virtual TCustomer FkCustomer { get; set; }
        public virtual TGoods FkGoods { get; set; }
        public virtual TGoodsSurveyQuestions FkQuestion { get; set; }
    }
}
