using System.Collections.Generic;
using System.Threading.Tasks;
using MarketPlace.API.Data.Dtos.Dashboard;
using MarketPlace.API.Helper;

namespace MarketPlace.API.Services.IService
{
    public interface IDashboardService 
    {
         Task<ApiResponse<DashboardDto>> GetDashboard();
         Task<ApiResponse<List<RecentOrderDto>>> GetDashboardOrder(int statusId);
         Task<ApiResponse<List<RecentReturnOrderDto>>> GetDashboardReturnOrder(int statusId);
         Task<ApiResponse<List<ShopRequestDto>>> GetDashboardShopRequest();
         Task<ApiResponse<List<GoodsRequestDto>>> GetDashboardGoodsRequest();
         Task<ApiResponse<List<BrandRequestDto>>> GetDashboardBrandRequest();
         Task<ApiResponse<List<GarenteeRequestDto>>> GetDashboardGarenteeRequest();
         Task<ApiResponse<List<WithDrawalRequestDto>>> GetDashboardWithDrawalRequest();
    }
}