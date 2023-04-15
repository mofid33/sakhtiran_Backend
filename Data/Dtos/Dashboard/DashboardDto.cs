using System.Collections.Generic;

namespace MarketPlace.API.Data.Dtos.Dashboard
{
    public class DashboardDto
    {
        public long TodayOrders { get; set; }
        public decimal TodayTotal { get; set; }
        public decimal TodayIncome { get; set; }
        public long Orders { get; set; }
        public decimal Total { get; set; }
        public decimal Income { get; set; }
        public decimal Tax { get; set; }
        public decimal Discount { get; set; }
        public long Customer { get; set; }
        public long Shop { get; set; }
        public long Goods { get; set; }
        public long OutOfStock { get; set; }
        public long Category { get; set; }
        public long Promotions { get; set; }

        public List<DashboardOrderStatusDto> OrderStatus { get; set; }
        public List<DashboardOrderReturningStatusDto> OrderReturningStatus { get; set; }
        public List<OrderChartDto> Chart { get; set; }
        public List<RecentOrderDto> RecentOrder { get; set; }
        public List<RecentReturnOrderDto> RecentReturnOrder { get; set; }
        public List<ApproveRequestDto> ApproveRequest { get; set; }
        public List<ShopRequestDto> ShopRequest { get; set; }
    }
}