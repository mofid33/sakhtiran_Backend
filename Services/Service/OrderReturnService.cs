using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using MarketPlace.API.Data.Dtos.Header;
using MarketPlace.API.Data.Dtos.OrderReturning;
using MarketPlace.API.Data.Dtos.Token;
using MarketPlace.API.Data.Enums;
using MarketPlace.API.Data.Repositories.IRepository;
using MarketPlace.API.Helper;
using MarketPlace.API.Services.IService;
using Microsoft.AspNetCore.Http;

namespace MarketPlace.API.Services.Service
{
    public class OrderReturnService : IOrderReturnService
    {
        public IMapper _mapper { get; }
        public IOrderReturnRepository _orderReturnRepository { get; }
        public HeaderParseDto header { get; set; }
        public TokenParseDto token { get; set; }
        public IMessageLanguageService _ms { get; set; }

        public OrderReturnService(IMapper mapper,
        IHttpContextAccessor httpContextAccessor,
        IMessageLanguageService ms, IOrderReturnRepository orderReturnRepository)
        {
            this._orderReturnRepository = orderReturnRepository;
            this._mapper = mapper;
            header = new HeaderParseDto(httpContextAccessor);
            this._ms = ms;
            token = new TokenParseDto(httpContextAccessor);
        }
        public async Task<ApiResponse<Pagination<OrderReturningListDto>>> GetOrderReturningList(OrderReturningPaginationDto pagination)
        {
            if (token.Rule == UserGroupEnum.Seller)
            {
                pagination.ShopId = token.Id;
            }
            var data = await _orderReturnRepository.GetOrderReturningList(pagination);
            if (data == null)
            {
                return new ApiResponse<Pagination<OrderReturningListDto>>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.OrderReturningGetting));
            }
            var count = await _orderReturnRepository.GetOrderReturningListCount(pagination);
            return new ApiResponse<Pagination<OrderReturningListDto>>(ResponseStatusEnum.Success, new Pagination<OrderReturningListDto>(count, data), _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<List<OrderReturningLogDto>>> GetOrderReturningLog(int returnId)
        {
            var data = await _orderReturnRepository.GetOrderReturningLog(returnId);
            if (data == null)
            {
                return new ApiResponse<List<OrderReturningLogDto>>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.OrderReturningLogGetting));
            }
            return new ApiResponse<List<OrderReturningLogDto>>(ResponseStatusEnum.Success, data, _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<OrderReturningDetailDto>> GetOrderReturningDetail(int returnId)
        {
            var data = await _orderReturnRepository.GetOrderReturningDetail(returnId);
            if (data == null)
            {
                return new ApiResponse<OrderReturningDetailDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.OrderReturningDetailGetting));
            }
            return new ApiResponse<OrderReturningDetailDto>(ResponseStatusEnum.Success, data, _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<bool>> EditOrderReturning(OrderReturningChangeDto orderReturning)
        {
            var data = await _orderReturnRepository.EditOrderReturning(orderReturning);
            if (data.Result == false)
            {
                return new ApiResponse<bool>(ResponseStatusEnum.BadRequest, false, _ms.MessageService(data.Message));
            }
            return new ApiResponse<bool>(ResponseStatusEnum.Success, true, _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<bool>> BlockAmountOrderReturning(AmountDto amount)
        {
            var data = await _orderReturnRepository.BlockAmountOrderReturning(amount);
            if (data.Result == false)
            {
                return new ApiResponse<bool>(ResponseStatusEnum.BadRequest, false, _ms.MessageService(data.Message));
            }
            return new ApiResponse<bool>(ResponseStatusEnum.Success, true, _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<bool>> RefoundAmountOrderReturning(AmountDto amount)
        {
            var data = await _orderReturnRepository.RefoundAmountOrderReturning(amount);
            if (data.Result == false)
            {
                return new ApiResponse<bool>(ResponseStatusEnum.BadRequest, false, _ms.MessageService(data.Message));
            }
            return new ApiResponse<bool>(ResponseStatusEnum.Success, true, _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<AmountDto>> GetOrderReturningAmount(int returnId )
        {
            var data = await _orderReturnRepository.GetOrderReturningAmount(returnId);
            if (data == null)
            {
                return new ApiResponse<AmountDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.OrderReturningAmountGetting));
            }
            return new ApiResponse<AmountDto>(ResponseStatusEnum.Success, data, _ms.MessageService(Message.Successfull));
        }
        public async Task<ApiResponse<bool>> SendEmailAndSMS(int returnId, string msg)
        {
            var data = await _orderReturnRepository.SendEmailAndSMS(returnId,msg);
            return new ApiResponse<bool>(ResponseStatusEnum.Success, true, _ms.MessageService(Message.Successfull));
        }
    }
}