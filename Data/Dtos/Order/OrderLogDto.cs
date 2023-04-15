using System;

namespace MarketPlace.API.Data.Dtos.Order
{
    public class OrderLogDto
    {
        public int LogId { get; set; }
        public long FkOrderId { get; set; }
        public long? FkOrderItemId { get; set; }
        public int? FkStatusId { get; set; }
        public string StatusTitle { get; set; }
        public Guid FkUserId { get; set; }
        public string UserName { get; set; }
        public string LogComment { get; set; }
        public string LogDateTime { get; set; }
    }
}