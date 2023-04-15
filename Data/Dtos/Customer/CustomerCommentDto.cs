using System;
using System.Collections.Generic;
using MarketPlace.API.Data.Dtos.Shop;
using MarketPlace.API.Data.Dtos.Survey;

namespace MarketPlace.API.Data.Dtos.Customer
{
    public class CustomerCommentDto
    {
        public int CommentId { get; set; }
        public int FkGoodsId { get; set; }
        public string GoodsTitle { get; set; }
        public string GoodsCode { get; set; }
        public string Image { get; set; }
        public string CustomerName { get; set; }
        public string CustomerFamily { get; set; }
        public string SerialNumber { get; set; }
        public double ReviewPoint { get; set; }
        public string CommentText { get; set; }
        public string CommentDate { get; set; }
        public bool? IsAccepted { get; set; }
        public List<ShopFormDto> Shop { get; set; }
        public List<GoodsCommentPointsDto> Points { get; set; }
    }
}