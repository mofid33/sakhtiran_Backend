using System;
using System.Collections.Generic;

namespace MarketPlace.API.Data.Models
{
    public partial class TShopSurveyAnswers
    {
        public int AnsId { get; set; }
        public int FkCommentId { get; set; }
        public int FkQuestionId { get; set; }
        public int FkCustomerId { get; set; }
        public int FkShopId { get; set; }
        public int AnsValue { get; set; }
        public long FkOrderItemId { get; set; }

        public virtual TGoodsComment FkComment { get; set; }
        public virtual TCustomer FkCustomer { get; set; }
        public virtual TOrderItem FkOrderItem { get; set; }
        public virtual TShopSurveyQuestions FkQuestion { get; set; }
        public virtual TShop FkShop { get; set; }
    }
}
