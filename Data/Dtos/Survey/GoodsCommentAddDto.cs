using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MarketPlace.API.Data.Models;

namespace MarketPlace.API.Data.Dtos.Survey
{

    /// کامنت های کاربران برای وب سایت
    public class GoodsCommentAddDto
    {

        public int CommentId { get; set; }
        public string CommentText { get; set; }
        public string CustomerName { get; set; }
        public int CustomerId { get; set; }
        public long OrderItemId { get; set; }
        public int LikeCount { get; set; }
        public double ReviewPoint { get; set; }
        public bool? IsAccepted { get; set; }

        public List<TGoodsCommentPoints> TGoodsCommentPoints { get; set; }
        public List<TShopSurveyAnswers> ShopSurveyAnswers { get; set; }
    }
}
