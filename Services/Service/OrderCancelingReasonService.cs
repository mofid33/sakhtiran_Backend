using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using MarketPlace.API.Data.Dtos.OrderCancelingReason;
using MarketPlace.API.Data.Models;
using MarketPlace.API.Data.Repositories.IRepository;
using MarketPlace.API.Helper;
using MarketPlace.API.Services.IService;
using MarketPlace.API.Data.Enums;
using MarketPlace.API.Data.Dtos.Pagination;
using MarketPlace.API.Data.Dtos.Header;
using MarketPlace.API.Data.Dtos.Token;
using Microsoft.AspNetCore.Http;
using MarketPlace.API.Data.Dtos.Accept;

namespace MarketPlace.API.Services.Service
{
    public class OrderCancelingReasonService : IOrderCancelingReasonService
    {
        public IMapper _mapper { get; }
        public IOrderCancelingReasonRepository _orderCancelingReasonRepository { get; }
        public HeaderParseDto header { get; set; }
        public TokenParseDto token { get; set; }
        public IMessageLanguageService _ms { get; set; }

        public OrderCancelingReasonService(IMapper mapper,
                IHttpContextAccessor httpContextAccessor,
        IMessageLanguageService ms, IOrderCancelingReasonRepository OrderCancelingReasonRepository)
        {
            this._orderCancelingReasonRepository = OrderCancelingReasonRepository;
            this._mapper = mapper;
            header = new HeaderParseDto(httpContextAccessor);
            this._ms = ms;
            token = new TokenParseDto(httpContextAccessor);
        }
        public async Task<ApiResponse<OrderCancelingReasonDto>> OrderCancelingReasonAdd(OrderCancelingReasonDto OrderCancelingReason)
        {
            var mapOrderCancelingReason = _mapper.Map<TOrderCancelingReason>(OrderCancelingReason);
            var craetedOrderCancelingReason = await _orderCancelingReasonRepository.OrderCancelingReasonAdd(mapOrderCancelingReason);
            if (craetedOrderCancelingReason == null)
            {
                return new ApiResponse<OrderCancelingReasonDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.OrderCancelingReasonAdding));
            }
            var mapCraetedOrderCancelingReason = _mapper.Map<OrderCancelingReasonDto>(craetedOrderCancelingReason);
            return new ApiResponse<OrderCancelingReasonDto>(ResponseStatusEnum.Success, mapCraetedOrderCancelingReason, _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<bool>> OrderCancelingReasonDelete(int id)
        {
            var exist = await this.OrderCancelingReasonExist(id);
            if (exist.Status == (int)ResponseStatusEnum.NotFound)
            {
                return new ApiResponse<bool>((ResponseStatusEnum)exist.Status, false, _ms.MessageService(exist.Message));
            }
            var result = await _orderCancelingReasonRepository.OrderCancelingReasonDelete(id);
            if (result.Result == false)
            {
                return new ApiResponse<bool>(ResponseStatusEnum.BadRequest, result.Result, _ms.MessageService(result.Message));
            }
            else
            {
                return new ApiResponse<bool>(ResponseStatusEnum.Success, result.Result, _ms.MessageService(result.Message));
            }
        }

        public async Task<ApiResponse<OrderCancelingReasonDto>> OrderCancelingReasonEdit(OrderCancelingReasonDto OrderCancelingReason)
        {
            var exist = await this.OrderCancelingReasonExist(OrderCancelingReason.ReasonId);
            if (exist.Status == (int)ResponseStatusEnum.NotFound)
            {
                return new ApiResponse<OrderCancelingReasonDto>((ResponseStatusEnum)exist.Status, null, _ms.MessageService(exist.Message));
            }
            var mapOrderCancelingReason = _mapper.Map<TOrderCancelingReason>(OrderCancelingReason);
            var editedOrderCancelingReason = await _orderCancelingReasonRepository.OrderCancelingReasonEdit(mapOrderCancelingReason);
            if (editedOrderCancelingReason == null)
            {
                return new ApiResponse<OrderCancelingReasonDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.OrderCancelingReasonEditing));
            }
            var mapEditedOrderCancelingReason = _mapper.Map<OrderCancelingReasonDto>(editedOrderCancelingReason);
            return new ApiResponse<OrderCancelingReasonDto>(ResponseStatusEnum.Success, mapEditedOrderCancelingReason, _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<bool>> OrderCancelingReasonExist(int id)
        {
            var result = await _orderCancelingReasonRepository.OrderCancelingReasonExist(id);
            if (result == false)
            {
                return new ApiResponse<bool>(ResponseStatusEnum.NotFound, result, _ms.MessageService(Message.OrderCancelingReasonNotFoundById));
            }
            else
            {
                return new ApiResponse<bool>(ResponseStatusEnum.Success, result, _ms.MessageService(Message.Successfull));
            }
        }

        public async Task<ApiResponse<Pagination<OrderCancelingReasonDto>>> OrderCancelingReasonGetAll(PaginationDto pagination)
        {
            var data = await _orderCancelingReasonRepository.OrderCancelingReasonGetAll(pagination);
            if (data == null)
            {
                return new ApiResponse<Pagination<OrderCancelingReasonDto>>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.OrderCancelingReasonGetting));
            }
            var count = await _orderCancelingReasonRepository.OrderCancelingReasonGetAllCount(pagination);
            return new ApiResponse<Pagination<OrderCancelingReasonDto>>(ResponseStatusEnum.Success, new Pagination<OrderCancelingReasonDto>(count, data), _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<List<OrderCancelingReasonDto>>> GetOrderCancelingReason()
        {
            var data = await _orderCancelingReasonRepository.OrderCancelingReasonGetAllForm();
            if (data == null)
            {
                return new ApiResponse<List<OrderCancelingReasonDto>>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.OrderCancelingReasonGetting));
            }
            return new ApiResponse<List<OrderCancelingReasonDto>>(ResponseStatusEnum.Success, data, _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<bool>> ChangeAccept(AcceptDto accept)
        {
            var data = await _orderCancelingReasonRepository.ChangeAccept(accept);
            if (data == false)
            {
                return new ApiResponse<bool>(ResponseStatusEnum.BadRequest, false, _ms.MessageService(Message.OrderCancelingReasonChangeStatus));
            }
            return new ApiResponse<bool>(ResponseStatusEnum.Success, true, _ms.MessageService(Message.Successfull));
        }
    }
}