using System.Collections.Generic;
using System.Threading.Tasks;
using MarketPlace.API.Data.Dtos.Dashboard;

namespace MarketPlace.API.Data.Repositories.IRepository
{
    public interface IDashboardRepository
    {
        Task<List<RecentOrderDto>> GetDashboardOrder(int statusId,int itemCount,decimal rate);
        Task<List<RecentReturnOrderDto>> GetDashboardReturnOrder(int statusId,int itemCount);
        Task<List<ShopRequestDto>> GetDashboardShopRequest(int itemCount);
        Task<List<GoodsRequestDto>> GetDashboardGoodsRequest(int itemCount);
        Task<List<BrandRequestDto>> GetDashboardBrandRequest(int itemCount);
        Task<List<GarenteeRequestDto>> GetDashboardGarenteeRequest(int itemCount);
        Task<List<WithDrawalRequestDto>> GetDashboardWithDrawalRequest(int itemCount,decimal rate);
        Task<int> GetSettingItemCount();
        Task<List<ApproveRequestDto>> GetApproveRequest();
        Task<List<DashboardOrderReturningStatusDto>> GetOrderReturningStatus();
        Task<List<DashboardOrderStatusDto>> GetOrderStatus();
        Task<long> GetCategoryCount();
        Task<long> GetCustomerCount();
        Task<long> GetOutOfStockCount();
        Task<long> GetPromotionsCount();
        Task<long> GetShopCount();
        Task<long> GetGoodsCount();
        Task<long> GetOrdersCount();
        Task<long> GetTodayOrdersCount();
        Task<decimal> GetIncome(decimal rate);
        Task<decimal> GetDiscount(decimal rate);
        Task<decimal> GetTax(decimal rate);
        Task<decimal> GetTotal(decimal rate);
        Task<decimal> GetTodayIncome(decimal rate);
        Task<decimal> GetTodayTotal(decimal rate);
        Task<List<OrderChartDto>> GetDashboardChart();
    }
}