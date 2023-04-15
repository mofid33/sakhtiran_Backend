using System;
using System.Collections.Generic;

namespace MarketPlace.API.Data.Models
{
    public partial class TGoodsComment
    {
        public TGoodsComment()
        {
            TGoodsCommentPoints = new HashSet<TGoodsCommentPoints>();
            TGoodsSurveyAnswers = new HashSet<TGoodsSurveyAnswers>();
            TShopSurveyAnswers = new HashSet<TShopSurveyAnswers>();
        }

        public int CommentId { get; set; }
        public int FkCustomerId { get; set; }
        public int FkGoodsId { get; set; }
        public int? FkVarietyId { get; set; }
        public double ReviewPoint { get; set; }
        public string CommentText { get; set; }
        public DateTime CommentDate { get; set; }
        public bool? IsAccepted { get; set; }

        public virtual TCustomer FkCustomer { get; set; }
        public virtual TGoods FkGoods { get; set; }
        public virtual TGoodsProvider FkVariety { get; set; }
        public virtual ICollection<TGoodsCommentPoints> TGoodsCommentPoints { get; set; }
        public virtual ICollection<TGoodsSurveyAnswers> TGoodsSurveyAnswers { get; set; }
        public virtual ICollection<TShopSurveyAnswers> TShopSurveyAnswers { get; set; }
    }
}
