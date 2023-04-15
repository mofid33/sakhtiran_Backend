using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MarketPlace.API.Data.Dtos.Customer;
using MarketPlace.API.Data.Dtos.Pagination;
using MarketPlace.API.Data.Dtos.ProductsStatistics;

namespace MarketPlace.API.Data.Repositories.IRepository
{
    public interface IProductStatisticsRepository
    {
        Task<List<ProductStatisticsDto>> GetMostPopularGoods(ProductStatisticsPaginationDto pagination);
        Task<List<ProductStatisticsDto>> GetMostVisitedGoods(ProductStatisticsPaginationDto pagination);
        Task<List<ProductStatisticsDto>> GetBestSellerGoods(ProductStatisticsPaginationDto pagination);
         Task<List<ProductStatisticsDetailsDto>>  GetGoodsCustomerLikeAndView(PaginationFormDto pagination , string type);
        Task<int> GetCountGoodsCustomerLikeAndView(PaginationFormDto pagination , string type);
        Task<List<ProductStatisticsDetailsDto>> GetGoodsSellDetails(PaginationFormDto pagination);
        Task<int> GetCountGoodsSellDetails(PaginationFormDto pagination);
        Task<List<ProductStatisticsDto>> GetNoSelleGoods(ProductStatisticsPaginationDto pagination);
        Task<int> CountProductStatisticsNoSaleGoods(ProductStatisticsPaginationDto pagination);
        Task<int> CountBestSellerGoods(ProductStatisticsPaginationDto pagination);
        Task<int> CountProductStatisticsGoods(ProductStatisticsPaginationDto pagination);
    }
}