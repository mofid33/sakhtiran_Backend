using System;
using System.Collections.Generic;
using MarketPlace.API.Data.Dtos.ReturningStatus;

namespace MarketPlace.API.Data.Dtos.OrderReturning
{
    public class OrderReturningListDto
    {
        public int ReturningId { get; set; }
        public int FkStatusId { get; set; }
        public int FkReturningActionId { get; set; }
        public string StatusTitle { get; set; }
        public long FkOrderId { get; set; }
        public long FkOrderItemId { get; set; }
        public string RegisterDateTime { get; set; }
        public int shopId { get; set; }
        public string ShopTitle { get; set; }
        public string CustomerName { get; set; }
        public int CustomerId { get; set; }
        public int GoodsId { get; set; }
        public string GoodsTitle { get; set; }
        public string GoodsCode { get; set; }
        public string GoodsSerialNumber { get; set; }
        public string GoodsImage { get; set; }
        public List<ReturningStatusFormDto> ReturningStatusList { get; set; }
    }
}