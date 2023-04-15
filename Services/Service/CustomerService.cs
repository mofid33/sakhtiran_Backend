using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using MarketPlace.API.Data.Dtos.Customer;
using MarketPlace.API.Data.Dtos.CustomerBankCards;
using MarketPlace.API.Data.Dtos.Header;
using MarketPlace.API.Data.Dtos.Home;
using MarketPlace.API.Data.Dtos.Order;
using MarketPlace.API.Data.Dtos.Token;
using MarketPlace.API.Data.Enums;
using MarketPlace.API.Data.Models;
using MarketPlace.API.Data.Repositories.IRepository;
using MarketPlace.API.Helper;
using MarketPlace.API.Services.IService;
using Microsoft.AspNetCore.Http;

namespace MarketPlace.API.Services.Service
{
    public class CustomerService : ICustomerService
    {
        public IMapper _mapper { get; }
        public ICustomerRepository _customerRepository { get; }
        public IGoodsCommentRepository _goodsCommentRepository { get; }
        public HeaderParseDto header { get; set; }
        public TokenParseDto token { get; set; }
        public IMessageLanguageService _ms { get; set; }
        public IOrderService _orderService { get; set; }


        public CustomerService(
        IMapper mapper,
        ICustomerRepository customerRepository,
        IHttpContextAccessor httpContextAccessor,
        IOrderService orderService,
        IGoodsCommentRepository goodsCommentRepository,
        IMessageLanguageService ms)
        {
            header = new HeaderParseDto(httpContextAccessor);
            token = new TokenParseDto(httpContextAccessor);
            this._customerRepository = customerRepository;
            this._mapper = mapper;
            _orderService = orderService;
            _goodsCommentRepository = goodsCommentRepository;
            _ms = ms;
        }

        public async Task<ApiResponse<Pagination<CustomerListDto>>> GetCustomerList(CustomerListPaginationDto pagination)
        {
            var data = await _customerRepository.GetCustomerList(pagination);
            if (data == null)
            {
                return new ApiResponse<Pagination<CustomerListDto>>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.CustomerGetting));
            }
            var count = await _customerRepository.GetCustomerListCount(pagination);
            return new ApiResponse<Pagination<CustomerListDto>>(ResponseStatusEnum.Success, new Pagination<CustomerListDto>(count, data), _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<CustomerGeneralDetailDto>> GetCustomerGeneralDetail(int customerId)
        {
            if (token.Id == 0)
            {
                token.Id = customerId;
            }
            var data = await _customerRepository.GetCustomerGeneralDetail(token.Id);
            if (data == null)
            {
                return new ApiResponse<CustomerGeneralDetailDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.CustomerGetting));
            }
            return new ApiResponse<CustomerGeneralDetailDto>(ResponseStatusEnum.Success, data, _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<List<CustomerAddressDto>>> GetCustomerAddress(int customerId)
        {
            var data = await _customerRepository.GetCustomerAddress(customerId);
            if (data == null)
            {
                return new ApiResponse<List<CustomerAddressDto>>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.CustomerAddressGetting));
            }
            return new ApiResponse<List<CustomerAddressDto>>(ResponseStatusEnum.Success, data, _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<Pagination<CustomerWishListViewDateDto>>> GetCustomerWishList(CustomerPaginationDto pagination)
        {
            var data = await _customerRepository.GetCustomerWishList(pagination);
            if (data == null)
            {
                return new ApiResponse<Pagination<CustomerWishListViewDateDto>>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.CustomerWishListGetting));
            }
            var count = await _customerRepository.GetCustomerWishListCount(pagination);
            return new ApiResponse<Pagination<CustomerWishListViewDateDto>>(ResponseStatusEnum.Success, new Pagination<CustomerWishListViewDateDto>(count, data), _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<Pagination<CustomerWishListViewDateDto>>> GetCustomerViewList(CustomerPaginationDto pagination)
        {
            var data = await _customerRepository.GetCustomerViewList(pagination);
            if (data == null)
            {
                return new ApiResponse<Pagination<CustomerWishListViewDateDto>>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.CustomerViewGetting));
            }
            var count = await _customerRepository.GetCustomerViewListCount(pagination);
            return new ApiResponse<Pagination<CustomerWishListViewDateDto>>(ResponseStatusEnum.Success, new Pagination<CustomerWishListViewDateDto>(count, data), _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<Pagination<CustomerCommentDto>>> GetCustomerCommentList(CustomerPaginationDto pagination)
        {
            var data = await _customerRepository.GetCustomerCommentList(pagination);
            if (data == null)
            {
                return new ApiResponse<Pagination<CustomerCommentDto>>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.CustomerAddressGetting));
            }
            var count = await _customerRepository.GetCustomerCommentListCount(pagination);
            return new ApiResponse<Pagination<CustomerCommentDto>>(ResponseStatusEnum.Success, new Pagination<CustomerCommentDto>(count, data), _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<Pagination<LiveCartListDto>>> GetCustomerLiveCartList(CustomerPaginationDto pagination)
        {
            var orderPagination = new LiveCartListPaginationDto();
            orderPagination.CustomerId = pagination.CustomerId;
            orderPagination.PageNumber = pagination.PageNumber;
            orderPagination.PageSize = pagination.PageSize;
            return await _orderService.GetLiveCartList(orderPagination);

        }

        public async Task<ApiResponse<OrderListGetDto>> GetCustomerOrderList(CustomerPaginationDto pagination)
        {
            var orderPagination = new OrderListPaginationDto();
            orderPagination.CustomerId = pagination.CustomerId;
            orderPagination.PageNumber = pagination.PageNumber;
            orderPagination.PageSize = pagination.PageSize;
            return await _orderService.GetOrderList(orderPagination);
        }

        public async Task<ApiResponse<CustomerBalanceGetDto>> GetCustomerBalance(CustomerPaginationDto pagination)
        {
            var data = new CustomerBalanceGetDto();
            data.AvailableBalance = await _customerRepository.GetAvailableBalance(pagination.CustomerId);
            var Balance = await _customerRepository.GetCustomerBalance(pagination);
            var count = await _customerRepository.GetCustomerBalanceCount(pagination);
            data.CustomerBalance = new Pagination<CustomerBalanceDto>(count, Balance);
            return new ApiResponse<CustomerBalanceGetDto>(ResponseStatusEnum.Success, data, _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<Pagination<CustomerCommentDto>>> GetAllCustomerCommentList(CustomerCommentPaginationDto pagination)
        {
            if (token.Rule == UserGroupEnum.Seller)
            {
                pagination.VendorId = token.Id;
            }
            var data = await _customerRepository.GetAllCustomerCommentList(pagination);
            if (data == null)
            {
                return new ApiResponse<Pagination<CustomerCommentDto>>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.CustomerAddressGetting));
            }
            var count = await _customerRepository.GetAllCustomerCommentListCount(pagination);
            return new ApiResponse<Pagination<CustomerCommentDto>>(ResponseStatusEnum.Success, new Pagination<CustomerCommentDto>(count, data), _ms.MessageService(Message.Successfull));      
        }

        public async Task<ApiResponse<string>> GetCustomerUserName(int customerId)
        {
            var data = await _customerRepository.GetCustomerUserName(customerId);
            if (data == null)
            {
                return new ApiResponse<string>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.UserNameGetting));
            }
            return new ApiResponse<string>(ResponseStatusEnum.Success, data, _ms.MessageService(Message.Successfull));
        }


        // ثبت کردن مشتری
        public async Task<ApiResponse<CustomerGeneralDetailDto>>  RegisterCustomer(CustomerGeneralDetailDto customer)
        {
            var mapCustomer = _mapper.Map<TCustomer>(customer);
            mapCustomer.RegisteryDate = DateTime.Now;
            mapCustomer.RefundPreference = 1 ;
            var CreatCustomer = await _customerRepository.RegisterCustomer(mapCustomer);
            if(CreatCustomer == null)
            {
                return new ApiResponse<CustomerGeneralDetailDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.CustomerIncompleteRegistration));
            }
            var mapCreateCustomer = _mapper.Map<CustomerGeneralDetailDto>(CreatCustomer);
            return new ApiResponse<CustomerGeneralDetailDto>(ResponseStatusEnum.Success, mapCreateCustomer, _ms.MessageService(Message.Successfull));

        }


        
        public async Task<ApiResponse<bool>> CustomerDelete(int customerId)
        {
         
            var result = await _customerRepository.CustomerDelete(customerId);
            if (result.Result == false)
            {
                return new ApiResponse<bool>(ResponseStatusEnum.BadRequest, result.Result, _ms.MessageService(result.Message));
            }
            else
            {
                return new ApiResponse<bool>(ResponseStatusEnum.Success, result.Result, _ms.MessageService(result.Message));
            }
        }

        public async Task<ApiResponse<int>> CustomerRefundPreference()
        {
            if (token.Id == 0 || token.Rule != UserGroupEnum.Customer)
            {
                return new ApiResponse<int>(ResponseStatusEnum.BadRequest, 0, _ms.MessageService(Message.UserNotFoundById));
            }       

            var result = await _customerRepository.CustomerRefundPreference(token.Id);
            return new ApiResponse<int>(ResponseStatusEnum.Success, result, _ms.MessageService(Message.Successfull));

            
        }

        public async Task<ApiResponse<bool>> SetCustomerRefundPreference(int refundPreference)
        {
            if (token.Id == 0 || token.Rule != UserGroupEnum.Customer)
            {
                return new ApiResponse<bool>(ResponseStatusEnum.BadRequest, false, _ms.MessageService(Message.UserNotFoundById));
            }       

            var result = await _customerRepository.SetCustomerRefundPreference(token.Id , refundPreference);
            return new ApiResponse<bool>(ResponseStatusEnum.Success, result, _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<List<CustomerBankCardGetDto>>> GetCustomerBankCards()
        {
            if (token.Id == 0 || token.Rule != UserGroupEnum.Customer)
            {
                return new ApiResponse<List<CustomerBankCardGetDto>>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.UserNotFoundById));
            }       

            var result = await _customerRepository.GetCustomerBankCards(token.Id);
            return new ApiResponse<List<CustomerBankCardGetDto>>(ResponseStatusEnum.Success, result, _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<bool>> RemoveCustomerBankCard(int bankCartId)
        {
            var result = await _customerRepository.RemoveCustomerBankCard(bankCartId);
            return new ApiResponse<bool>(ResponseStatusEnum.Success, result, _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<bool>> ChangeGoodsCommentIsAccept(int commentId, bool? isAccept)
        {
            var result = await _goodsCommentRepository.ChangeIsAccept(commentId, isAccept);
            return new ApiResponse<bool>(ResponseStatusEnum.Success, result, _ms.MessageService(Message.Successfull));
        }
    }
}