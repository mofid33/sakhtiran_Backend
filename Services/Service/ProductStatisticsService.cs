using System.Collections.Generic;
using System.Threading.Tasks;
using MarketPlace.API.Data.Dtos.Customer;
using MarketPlace.API.Data.Dtos.Header;
using MarketPlace.API.Data.Dtos.Order;
using MarketPlace.API.Data.Dtos.Pagination;
using MarketPlace.API.Data.Dtos.ProductsStatistics;
using MarketPlace.API.Data.Dtos.Token;
using MarketPlace.API.Data.Enums;
using MarketPlace.API.Data.Repositories.IRepository;
using MarketPlace.API.Helper;
using MarketPlace.API.Services.IService;
using Microsoft.AspNetCore.Http;

namespace MarketPlace.API.Services.Service
{
    public class ProductStatisticsService : IProductStatisticsService
    {
        public HeaderParseDto header { get; set; }
        public TokenParseDto token { get; set; }
        public IMessageLanguageService _ms { get; set; }
        public IProductStatisticsRepository _productStatisticsRepository { get; }
        public ProductStatisticsService(IHttpContextAccessor httpContextAccessor,
        IMessageLanguageService ms,
        ISettingRepository settingRepository,
        IProductStatisticsRepository productStatisticsRepository)
        {
            this._productStatisticsRepository = productStatisticsRepository;
            header = new HeaderParseDto(httpContextAccessor);
            this._ms = ms;
            token = new TokenParseDto(httpContextAccessor);
        }

        public async Task<ApiResponse<Pagination<ProductStatisticsDto>>> GetMostPopularGoods(ProductStatisticsPaginationDto pagination)
        {
            var data = await _productStatisticsRepository.GetMostPopularGoods(pagination);
            if (data == null)
            {
                return new ApiResponse<Pagination<ProductStatisticsDto>>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.GoodsGetting));
            }
            var count = await _productStatisticsRepository.CountProductStatisticsGoods(pagination);
            return new ApiResponse<Pagination<ProductStatisticsDto>>(ResponseStatusEnum.Success, new Pagination<ProductStatisticsDto>(count, data), _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<Pagination<ProductStatisticsDto>>> GetMostVisitedGoods(ProductStatisticsPaginationDto pagination)
        {
            var data = await _productStatisticsRepository.GetMostVisitedGoods(pagination);
            if (data == null)
            {
                return new ApiResponse<Pagination<ProductStatisticsDto>>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.GoodsGetting));
            }
            var count = await _productStatisticsRepository.CountProductStatisticsGoods(pagination);
            return new ApiResponse<Pagination<ProductStatisticsDto>>(ResponseStatusEnum.Success, new Pagination<ProductStatisticsDto>(count, data), _ms.MessageService(Message.Successfull));       
        
        }

        public async Task<ApiResponse<Pagination<ProductStatisticsDto>>> GetBestSellerGoods(ProductStatisticsPaginationDto pagination)
        {
            var data = await _productStatisticsRepository.GetBestSellerGoods(pagination);
            if (data == null)
            {
                return new ApiResponse<Pagination<ProductStatisticsDto>>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.GoodsGetting));
            }
            var count = await _productStatisticsRepository.CountBestSellerGoods(pagination);
            return new ApiResponse<Pagination<ProductStatisticsDto>>(ResponseStatusEnum.Success, new Pagination<ProductStatisticsDto>(count, data), _ms.MessageService(Message.Successfull));       
                
        }

        public async Task<ApiResponse<Pagination<ProductStatisticsDetailsDto>>> GetGoodsCustomerLikeAndView(PaginationFormDto pagination , string type)
        {
            var data = await _productStatisticsRepository.GetGoodsCustomerLikeAndView(pagination , type);
            if (data == null)
            {
                return new ApiResponse<Pagination<ProductStatisticsDetailsDto>>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.GoodsGetting));
            }
            var count = await _productStatisticsRepository.GetCountGoodsCustomerLikeAndView(pagination , type);
            return new ApiResponse<Pagination<ProductStatisticsDetailsDto>>(ResponseStatusEnum.Success, new Pagination<ProductStatisticsDetailsDto>(count, data), _ms.MessageService(Message.Successfull));       
                
        }
        public async Task<ApiResponse<Pagination<ProductStatisticsDto>>> GetNoSelleGoods(ProductStatisticsPaginationDto pagination)
        {
            var data = await _productStatisticsRepository.GetNoSelleGoods(pagination);
            if (data == null)
            {
                return new ApiResponse<Pagination<ProductStatisticsDto>>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.GoodsGetting));
            }
            var count = await _productStatisticsRepository.CountProductStatisticsNoSaleGoods(pagination);
            return new ApiResponse<Pagination<ProductStatisticsDto>>(ResponseStatusEnum.Success, new Pagination<ProductStatisticsDto>(count, data), _ms.MessageService(Message.Successfull));       
                
        }

        public async Task<ApiResponse<Pagination<ProductStatisticsDetailsDto>>> GetGoodsSellDetails(PaginationFormDto pagination)
        {
           var data = await _productStatisticsRepository.GetGoodsSellDetails(pagination);
            if (data == null)
            {
                return new ApiResponse<Pagination<ProductStatisticsDetailsDto>>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.GoodsGetting));
            }
            var count = await _productStatisticsRepository.GetCountGoodsSellDetails(pagination);
            return new ApiResponse<Pagination<ProductStatisticsDetailsDto>>(ResponseStatusEnum.Success, new Pagination<ProductStatisticsDetailsDto>(count, data), _ms.MessageService(Message.Successfull));       
                       
        }
    }
}