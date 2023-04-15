using System.Collections.Generic;
using System.Threading.Tasks;
using MarketPlace.API.Data.Dtos.Header;
using MarketPlace.API.Data.Dtos.Order;
using MarketPlace.API.Data.Dtos.Token;
using MarketPlace.API.Data.Enums;
using MarketPlace.API.Data.Repositories.IRepository;
using MarketPlace.API.Helper;
using MarketPlace.API.Services.IService;
using Microsoft.AspNetCore.Http;

namespace MarketPlace.API.Services.Service
{
    public class OrderService : IOrderService
    {
        public HeaderParseDto header { get; set; }
        public TokenParseDto token { get; set; }
        public IMessageLanguageService _ms { get; set; }
        public IOrderRepository _orderRepository { get; }
        public ISettingRepository _settingRepository { get; }
        public OrderService(IHttpContextAccessor httpContextAccessor,
        IMessageLanguageService ms,
        ISettingRepository settingRepository,
        IOrderRepository orderRepository)
        {
            this._orderRepository = orderRepository;
            header = new HeaderParseDto(httpContextAccessor);
            this._ms = ms;
            this._settingRepository = settingRepository;
            token = new TokenParseDto(httpContextAccessor);
        }
        public async Task<ApiResponse<OrderListGetDto>> GetOrderList(OrderListPaginationDto pagination)
        {
            var data = new OrderListGetDto();
            pagination.Rate = (decimal) 1.0;
            pagination.Rate = await _settingRepository.GetCurrencyRate();
            data.Order = await _orderRepository.GetOrderList(pagination);
            data.OrderCount = await _orderRepository.GetOrderListCount(pagination);
            data.Discount = await _orderRepository.GetOrderListDiscount(pagination);
            data.ComissionPrice = await _orderRepository.GetOrderListComissionPrice(pagination);
            data.ShipingCost = await _orderRepository.GetOrderListShipingCost(pagination);
            data.VatAmount = await _orderRepository.GetOrderListVatAmount(pagination);
            data.FinalPrice = await _orderRepository.GetOrderListFinalPrice(pagination);
            data.Count = await _orderRepository.GetOrderListItemCount(pagination);
            if (data.Order == null)
            {
                return new ApiResponse<OrderListGetDto>(ResponseStatusEnum.BadRequest, data, _ms.MessageService(Message.OrderGetting));
            }
            return new ApiResponse<OrderListGetDto>(ResponseStatusEnum.Success, data, _ms.MessageService(Message.Successfull));
        }
        public async Task<ApiResponse<ShopOrderListGetDto>> GetShopOrderList(OrderListPaginationDto pagination)
        {
            var data = new ShopOrderListGetDto();
            pagination.Rate = (decimal) 1.0;
            pagination.ShopId = token.Id;
            pagination.Rate = await _settingRepository.GetCurrencyRate();

            data.Order = await _orderRepository.GetShopOrderList(pagination);
            data.OrderCount = await _orderRepository.GetShopOrderListCount(pagination);
            data.Discount = await _orderRepository.GetShopOrderListDiscount(pagination);
            data.ShipingCost = await _orderRepository.GetShopOrderListShipingCost(pagination);
            data.VatAmount = await _orderRepository.GetShopOrderListVatAmount(pagination);
            data.FinalPrice = await _orderRepository.GetShopOrderListFinalPrice(pagination);
            data.Count = await _orderRepository.GetShopOrderListItemCount(pagination);
            if (data.Order == null)
            {
                return new ApiResponse<ShopOrderListGetDto>(ResponseStatusEnum.BadRequest, data, _ms.MessageService(Message.OrderGetting));
            }
            return new ApiResponse<ShopOrderListGetDto>(ResponseStatusEnum.Success, data, _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<OrderDetailGetDto>> GetOrderDetail(long orderId)
        {
            var shopId = 0 ;
             if(token.Rule == UserGroupEnum.Seller)
            {
              shopId = token.Id;
            }           
            var rate = await _settingRepository.GetCurrencyRate();
            var data = await _orderRepository.GetOrderDetail(orderId, rate , shopId);
            if (data == null)
            {
                return new ApiResponse<OrderDetailGetDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.OrderGetting));
            }
            return new ApiResponse<OrderDetailGetDto>(ResponseStatusEnum.Success, data, _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<List<OrderLogDto>>> GetOrderLog(long orderId)
        {
            var data = await _orderRepository.GetOrderLog(orderId);
            if (data == null)
            {
                return new ApiResponse<List<OrderLogDto>>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.OrderLogGetting));
            }
            return new ApiResponse<List<OrderLogDto>>(ResponseStatusEnum.Success, data, _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<Pagination<LiveCartListDto>>> GetLiveCartList(LiveCartListPaginationDto pagination)
        {
            pagination.Rate = await _settingRepository.GetCurrencyRate();
            var data = await _orderRepository.GetLiveCartList(pagination);
            if (data == null)
            {
                return new ApiResponse<Pagination<LiveCartListDto>>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.OrderGetting));
            }
            var count = await _orderRepository.GetLiveCartListCount(pagination);
            return new ApiResponse<Pagination<LiveCartListDto>>(ResponseStatusEnum.Success, new Pagination<LiveCartListDto>(count, data), _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<LiveCartDetailGetDto>> GetLiveCartDetail(long orderId)
        {
            var rate = await _settingRepository.GetCurrencyRate();
            var data = await _orderRepository.GetLiveCartDetail(orderId, rate);
            if (data == null)
            {
                return new ApiResponse<LiveCartDetailGetDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.OrderGetting));
            }
            return new ApiResponse<LiveCartDetailGetDto>(ResponseStatusEnum.Success, data, _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<bool>> DeleteLiveCart(long orderId)
        {
            var result = await _orderRepository.DeleteLiveCart(orderId);
            if (result == false)
            {
                return new ApiResponse<bool>(ResponseStatusEnum.BadRequest, false, _ms.MessageService(Message.LiveCartDeleting));
            }
            return new ApiResponse<bool>(ResponseStatusEnum.Success, true, _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<Pagination<ShippmentListDto>>> GetShippmentList(ShippmentPaginationDto pagination)
        {
            if(token.Rule == UserGroupEnum.Seller)
            {
                pagination.ShopId = token.Id;
            }
            var data = await _orderRepository.GetShippmentList(pagination);
            if (data == null)
            {
                return new ApiResponse<Pagination<ShippmentListDto>>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.OrderGetting));
            }
            return new ApiResponse<Pagination<ShippmentListDto>>(ResponseStatusEnum.Success, new Pagination<ShippmentListDto>(data.Count, data.ShippmentList), _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<List<ShippmentDetailDto>>> GetShippmentDetail(int shopId , int customerId)
        {
            var data = await _orderRepository.GetShippmentDetail(shopId , customerId);
            if (data == null)
            {
                return new ApiResponse<List<ShippmentDetailDto>>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.ShippmentDetailGetting));
            }
            return new ApiResponse<List<ShippmentDetailDto>>(ResponseStatusEnum.Success, data, _ms.MessageService(Message.Successfull));
        }      

        public async Task<ApiResponse<SalesDetailDto>> GetSalesDetail(long itemId)
        {
            var data = await _orderRepository.GetSalesDetail(itemId);
            if (data == null)
            {
                return new ApiResponse<SalesDetailDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.SalesDetailGetting));
            }
            return new ApiResponse<SalesDetailDto>(ResponseStatusEnum.Success, data, _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<int>> ChangeStauts(long orderItemId, int statusId)
        {
            var data = await _orderRepository.ChangeStauts(orderItemId, statusId);
            if (data.Result == false)
            {
                return new ApiResponse<int>(ResponseStatusEnum.BadRequest, data.Data, _ms.MessageService(data.Message));
            }
            return new ApiResponse<int>(ResponseStatusEnum.Success, data.Data, _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<Pagination<SalesListDto>>> GeSalesList(SalesListPaginationDto pagination)
        {
            if(token.Rule == UserGroupEnum.Seller)
            {
                pagination.ShopId = token.Id;
            }
            var data = await _orderRepository.GeSalesList(pagination);
            if (data == null)
            {
                return new ApiResponse<Pagination<SalesListDto>>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.OrderGetting));
            }
            var count = await _orderRepository.GeSalesListCount(pagination);
            return new ApiResponse<Pagination<SalesListDto>>(ResponseStatusEnum.Success, new Pagination<SalesListDto>(count, data), _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<Pagination<OrderCallRequestDto>>> GetOrderCallRequest(OrderCallRequestPaginationDto pagination)
        {
            if(token.Rule == UserGroupEnum.Seller)
            {
                pagination.ShopId = token.Id;
            }
            var data = await _orderRepository.GetOrderCallRequest(pagination);
            if (data == null)
            {
                return new ApiResponse<Pagination<OrderCallRequestDto>>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.OrderItemNotFound));
            }
            var count = await _orderRepository.GetOrderCallRequestCount(pagination);
            return new ApiResponse<Pagination<OrderCallRequestDto>>(ResponseStatusEnum.Success, new Pagination<OrderCallRequestDto>(count, data), _ms.MessageService(Message.Successfull));        
            
            }

        public async Task<ApiResponse<bool>> ChangeCallRequestStatus(long callRequestId, int status)
        {
            var data = await _orderRepository.ChangeCallRequestStatus(callRequestId , status);
            if (data == false)
            {
                return new ApiResponse<bool>(ResponseStatusEnum.BadRequest, false, _ms.MessageService(Message.OrderItemNotFound));
            }
            return new ApiResponse<bool>(ResponseStatusEnum.Success, data, _ms.MessageService(Message.Successfull));        
        }
    }
}