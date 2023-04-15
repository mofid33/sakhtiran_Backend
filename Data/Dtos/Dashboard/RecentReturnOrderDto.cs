using System;

namespace MarketPlace.API.Data.Dtos.Dashboard
{
    public class RecentReturnOrderDto
    {
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }

        public int ShopId { get; set; }
        public string ShopTitle { get; set; }


        public int GoodsId { get; set; }
        public string GoodsTitle { get; set; }
        public string GoodsCode { get; set; }
        public string GoodsSerialNumber { get; set; }
        public string GoodsImage { get; set; }

        public int ReturningId { get; set; }
        public int FkReturningReasonId { get; set; }
        public string ReturningReasonTitle { get; set; }
        public int FkReturningActionId { get; set; }
        public string ReturningActionTitle { get; set; }
        public string RegisterDateTime { get; set; }


    }
}