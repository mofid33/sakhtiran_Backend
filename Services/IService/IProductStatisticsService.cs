using System.Collections.Generic;
using System.Threading.Tasks;
using MarketPlace.API.Data.Dtos.Accept;
using MarketPlace.API.Data.Dtos.Customer;
using MarketPlace.API.Data.Dtos.Goods;
using MarketPlace.API.Data.Dtos.Pagination;
using MarketPlace.API.Data.Dtos.ProductsStatistics;
using MarketPlace.API.Data.Dtos.Specification;
using MarketPlace.API.Data.Dtos.Variation;
using MarketPlace.API.Helper;

namespace MarketPlace.API.Services.IService
{
    public interface IProductStatisticsService
    {
        Task<ApiResponse<Pagination<ProductStatisticsDto>>> GetMostPopularGoods(ProductStatisticsPaginationDto pagination);
        Task<ApiResponse<Pagination<ProductStatisticsDto>>> GetMostVisitedGoods(ProductStatisticsPaginationDto pagination);
        Task<ApiResponse<Pagination<ProductStatisticsDto>>> GetBestSellerGoods(ProductStatisticsPaginationDto pagination);
        Task<ApiResponse<Pagination<ProductStatisticsDetailsDto>>> GetGoodsCustomerLikeAndView(PaginationFormDto pagination , string type);
        Task<ApiResponse<Pagination<ProductStatisticsDto>>> GetNoSelleGoods(ProductStatisticsPaginationDto pagination);
        Task<ApiResponse<Pagination<ProductStatisticsDetailsDto>>> GetGoodsSellDetails(PaginationFormDto pagination);

    }
}