using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using MarketPlace.API.Data.Dtos.Dashboard;
using MarketPlace.API.Data.Dtos.Header;
using MarketPlace.API.Data.Dtos.Token;
using MarketPlace.API.Data.Enums;
using MarketPlace.API.Data.Repositories.IRepository;
using MarketPlace.API.Helper;
using MarketPlace.API.Services.IService;
using Microsoft.AspNetCore.Http;

namespace MarketPlace.API.Services.Service
{
    public class DashboardService : IDashboardService
    {
        public  IMapper _mapper { get; }
        public  IDashboardRepository _dashboardRepository { get; }
        public  ISettingRepository _settingRepository { get; }
        public  HeaderParseDto header { get; set; }
        public  TokenParseDto token { get; set; }
        public  IMessageLanguageService _ms { get; set; }

        public DashboardService(
        IMapper mapper,
        IDashboardRepository dashboardRepository,
        ISettingRepository settingRepository,
        IHttpContextAccessor httpContextAccessor,
        IOrderService orderService,
        IMessageLanguageService ms)
        {
            header = new HeaderParseDto(httpContextAccessor);
            token = new TokenParseDto(httpContextAccessor);
            this._dashboardRepository = dashboardRepository;
            this._mapper = mapper;
            _settingRepository = settingRepository;
            _ms = ms;
        }

        public async Task<ApiResponse<DashboardDto>> GetDashboard()
        {
            var itemCount = await _dashboardRepository.GetSettingItemCount();
            var rate = (decimal) await _settingRepository.GetCurrencyRate();
            var data = new DashboardDto();
            data.Category = await _dashboardRepository.GetCategoryCount();
            data.Customer = await _dashboardRepository.GetCustomerCount();
            data.OutOfStock = await _dashboardRepository.GetOutOfStockCount();
            data.OrderReturningStatus = await _dashboardRepository.GetOrderReturningStatus();
            data.OrderStatus = await _dashboardRepository.GetOrderStatus();
            data.Goods = await _dashboardRepository.GetGoodsCount();
            data.Orders = await _dashboardRepository.GetOrdersCount();
            data.TodayOrders = await _dashboardRepository.GetTodayOrdersCount();
            data.Discount = await _dashboardRepository.GetDiscount(rate);
            data.Tax = await _dashboardRepository.GetTax(rate);
            data.Total = await _dashboardRepository.GetTotal(rate);
            data.TodayTotal = await _dashboardRepository.GetTodayTotal(rate);
            data.RecentOrder = await _dashboardRepository.GetDashboardOrder((int)OrderStatusEnum.Cart,itemCount,rate);
            data.RecentReturnOrder = await _dashboardRepository.GetDashboardReturnOrder((int)ReturningStatusEnum.Processing,itemCount);
            data.Chart = await _dashboardRepository.GetDashboardChart();
            data.Promotions = await _dashboardRepository.GetPromotionsCount();
            if (token.Rule == UserGroupEnum.Admin)
            {
                data.Shop = await _dashboardRepository.GetShopCount();
                data.ShopRequest = await _dashboardRepository.GetDashboardShopRequest(itemCount);
                data.ApproveRequest = await _dashboardRepository.GetApproveRequest();
                data.Income = await _dashboardRepository.GetIncome(rate);
                data.TodayIncome = await _dashboardRepository.GetTodayIncome(rate);
            }
            return new ApiResponse<DashboardDto>(ResponseStatusEnum.Success, data, _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<List<RecentOrderDto>>> GetDashboardOrder(int statusId)
        {
            var itemCount = await _dashboardRepository.GetSettingItemCount();
            var rate = (decimal) await _settingRepository.GetCurrencyRate();
            var data = await _dashboardRepository.GetDashboardOrder(statusId,itemCount,rate);
            if (data == null)
            {
                return new ApiResponse<List<RecentOrderDto>>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.OrderGetting));
            }
            return new ApiResponse<List<RecentOrderDto>>(ResponseStatusEnum.Success, data, _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<List<RecentReturnOrderDto>>> GetDashboardReturnOrder(int statusId)
        {
            var itemCount = await _dashboardRepository.GetSettingItemCount();
            var data = await _dashboardRepository.GetDashboardReturnOrder(statusId,itemCount);
            if (data == null)
            {
                return new ApiResponse<List<RecentReturnOrderDto>>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.OrderReturningGetting));
            }
            return new ApiResponse<List<RecentReturnOrderDto>>(ResponseStatusEnum.Success, data, _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<List<ShopRequestDto>>> GetDashboardShopRequest()
        {
            var itemCount = await _dashboardRepository.GetSettingItemCount();
            var data = await _dashboardRepository.GetDashboardShopRequest(itemCount);
            if (data == null)
            {
                return new ApiResponse<List<ShopRequestDto>>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.ShopGetting));
            }
            return new ApiResponse<List<ShopRequestDto>>(ResponseStatusEnum.Success, data, _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<List<GoodsRequestDto>>> GetDashboardGoodsRequest()
        {
            var itemCount = await _dashboardRepository.GetSettingItemCount();
            var data = await _dashboardRepository.GetDashboardGoodsRequest(itemCount);
            if (data == null)
            {
                return new ApiResponse<List<GoodsRequestDto>>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.GoodsGetting));
            }
            return new ApiResponse<List<GoodsRequestDto>>(ResponseStatusEnum.Success, data, _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<List<BrandRequestDto>>> GetDashboardBrandRequest()
        {
            var itemCount = await _dashboardRepository.GetSettingItemCount();
            var data = await _dashboardRepository.GetDashboardBrandRequest(itemCount);
            if (data == null)
            {
                return new ApiResponse<List<BrandRequestDto>>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.BrandGetting));
            }
            return new ApiResponse<List<BrandRequestDto>>(ResponseStatusEnum.Success, data, _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<List<GarenteeRequestDto>>> GetDashboardGarenteeRequest()
        {
            var itemCount = await _dashboardRepository.GetSettingItemCount();
            var data = await _dashboardRepository.GetDashboardGarenteeRequest(itemCount);
            if (data == null)
            {
                return new ApiResponse<List<GarenteeRequestDto>>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.GuaranteeGetting));
            }
            return new ApiResponse<List<GarenteeRequestDto>>(ResponseStatusEnum.Success, data, _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<List<WithDrawalRequestDto>>> GetDashboardWithDrawalRequest()
        {
            var itemCount = await _dashboardRepository.GetSettingItemCount();
            var rate = (decimal) await _settingRepository.GetCurrencyRate();
            var data = await _dashboardRepository.GetDashboardWithDrawalRequest(itemCount,rate);
            if (data == null)
            {
                return new ApiResponse<List<WithDrawalRequestDto>>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.ShopWithdrawalRequestGetting));
            }
            return new ApiResponse<List<WithDrawalRequestDto>>(ResponseStatusEnum.Success, data, _ms.MessageService(Message.Successfull));
        }
    }
}