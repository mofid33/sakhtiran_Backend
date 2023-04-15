using System;
using System.Net.Http;
using System.Threading.Tasks;
using MarketPlace.API.Data.Enums;
using MarketPlace.API.Data.Repositories.IRepository;
using MarketPlace.API.Helper;
using MarketPlace.API.Services.IService;
using AutoMapper;
using MarketPlace.API.Data.Dtos.UserOrder;
using MarketPlace.API.Data.Dtos.Header;
using MarketPlace.API.Data.Dtos.Token;
using Microsoft.AspNetCore.Http;
using MarketPlace.API.Data.Dtos.Pagination;
using MarketPlace.API.Data.Models;
using System.Collections.Generic;
using System.Linq;
using MarketPlace.API.Data.Dtos.Customer;
using Microsoft.Extensions.Configuration;
using MarketPlace.API.PaymentGateway.PaypalHelper;
using RestSharp;
using System.Globalization;
using FSS.Pipe;
using MarketPlace.API.PaymentGateway.CredimaxHelper;

namespace MarketPlace.API.Services.Service
{
    public class UserOrderService : IUserOrderService
    {
        public IMapper _mapper { get; }
        public IUserOrderRepository _userOrderRepository { get; }
        public IGoodsRepository _goodsRepository { get; }
        public ISettingRepository _settingRepository { get; }
        public ICustomerRepository _customerRepository { get; }
        public IMessageLanguageService _ms { get; }
        public HeaderParseDto header { get; set; }
        public TokenParseDto token { get; set; }
        public IConfiguration Configuration { get; }
        public UserOrderService(
        IMapper mapper,
        IGoodsRepository goodsRepository,
        ISettingRepository settingRepository,
        ICustomerRepository customerRepository,
        IMessageLanguageService ms,
        IConfiguration Configuration,
        IHttpContextAccessor httpContextAccessor,
        IUserOrderRepository userOrderRepository)
        {
            this._goodsRepository = goodsRepository;
            this._userOrderRepository = userOrderRepository;
            this._settingRepository = settingRepository;
            this._customerRepository = customerRepository;
            this._mapper = mapper;
            _ms = ms;
            this.Configuration = Configuration;
            header = new HeaderParseDto(httpContextAccessor);
            token = new TokenParseDto(httpContextAccessor);
        }

        public async Task<ApiResponse<OrderCountDto>> AddOrder(OrderAddDto orderDto)
        {
            if (token.Rule != UserGroupEnum.Customer)
            {
                return new ApiResponse<OrderCountDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.UserNotFoundById));
            }
            var customerId = token.Id;
            var cookieId = customerId == 0 ? (token.CookieId == Guid.Empty ? Guid.NewGuid() : token.CookieId) : (Guid?)null;
            if (token.Id == 0)
            {
                customerId = (int)CustomerTypeEnum.Unknown;
                orderDto.OneClick = false;
            }
            var data = await _userOrderRepository.AddOrder(orderDto, customerId, cookieId);
            var orderCount = await _userOrderRepository.GetUserOrderCount(token.Id, cookieId);
            if (data.Result == false)
            {
                return new ApiResponse<OrderCountDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(data.Message));
            }
            else
            {
                return new ApiResponse<OrderCountDto>(ResponseStatusEnum.Success, new OrderCountDto(orderCount, cookieId, data.Data.SetOneClick), _ms.MessageService(data.Message));
            }
        }

        public async Task<ApiResponse<OrderCountDto>> IncreaseOrderItem(OrderAddDto orderDto)
        {
            if ((token.Id == 0 && token.CookieId == Guid.Empty) || token.Rule != UserGroupEnum.Customer)
            {
                return new ApiResponse<OrderCountDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.UserNotFoundById));
            }
            var data = await _userOrderRepository.IncreaseOrderItem(orderDto);
            var orderCount = await _userOrderRepository.GetUserOrderCount(token.Id, token.CookieId);
            if (data.Result == false)
            {
                return new ApiResponse<OrderCountDto>(ResponseStatusEnum.BadRequest, new OrderCountDto(orderCount, token.CookieId, false), _ms.MessageService(data.Message));
            }
            else
            {
                return new ApiResponse<OrderCountDto>(ResponseStatusEnum.Success, new OrderCountDto(orderCount, token.CookieId, false), _ms.MessageService(data.Message));
            }
        }

        public async Task<ApiResponse<OrderCountDto>> DeleteOrderItem(long orderItemId)
        {
            if ((token.Id == 0 && token.CookieId == Guid.Empty) || token.Rule != UserGroupEnum.Customer)
            {
                return new ApiResponse<OrderCountDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.UserNotFoundById));
            }
            var data = await _userOrderRepository.DeleteOrderItem(orderItemId);
            var orderCount = await _userOrderRepository.GetUserOrderCount(token.Id, token.CookieId);
            if (data.Result == false)
            {
                return new ApiResponse<OrderCountDto>(ResponseStatusEnum.BadRequest, new OrderCountDto(orderCount, token.CookieId, false), _ms.MessageService(data.Message));
            }
            else
            {
                return new ApiResponse<OrderCountDto>(ResponseStatusEnum.Success, new OrderCountDto(orderCount, token.CookieId, false), _ms.MessageService(data.Message));
            }
        }

        public async Task<ApiResponse<WebsiteOrderGetDto>> GetOrderDetail(string code, int? cityId, int? countryId, int? provinceId, bool forPay)
        {
            if ((token.Id == 0 && token.CookieId == Guid.Empty) || token.Rule != UserGroupEnum.Customer)
            {
                return new ApiResponse<WebsiteOrderGetDto>(ResponseStatusEnum.Success, null, _ms.MessageService(Message.UserNotFoundById));
            }
            if (cityId != null && countryId != null && provinceId != null)
            {
                var changeArea = await _userOrderRepository.ChangeAreaOrder((int)cityId, (int)countryId, (int)provinceId);
                if (changeArea == false)
                {
                    return new ApiResponse<WebsiteOrderGetDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.EditChangeAreaOrder));
                }
            }
            var order = await _userOrderRepository.GetOrderDetail(code, forPay);
            if (order.Result == false)
            {
                return new ApiResponse<WebsiteOrderGetDto>(ResponseStatusEnum.BadRequest, order.Data, _ms.MessageService(order.Message));
            }
            else
            {
                return new ApiResponse<WebsiteOrderGetDto>(ResponseStatusEnum.Success, order.Data, _ms.MessageService(order.Message));
            }
        }

        public async Task<ApiResponse<WebsiteOrderGetDto>> OrderCityCountryDetail()
        {
            if ((token.Id == 0 && token.CookieId == Guid.Empty) || token.Rule != UserGroupEnum.Customer)
            {
                return new ApiResponse<WebsiteOrderGetDto>(ResponseStatusEnum.Success, null, _ms.MessageService(Message.UserNotFoundById));
            }

            var order = await _userOrderRepository.OrderCityCountryDetail();
            if (order.Result == false)
            {
                return new ApiResponse<WebsiteOrderGetDto>(ResponseStatusEnum.BadRequest, order.Data, _ms.MessageService(order.Message));
            }
            else
            {
                return new ApiResponse<WebsiteOrderGetDto>(ResponseStatusEnum.Success, order.Data, _ms.MessageService(order.Message));
            }
        }

        public async Task<ApiResponse<bool>> ChangeDestination(int addressId)
        {
            var result = await _userOrderRepository.ChangeDestination(0, true, addressId);
            if (result.Result == false)
            {
                return new ApiResponse<bool>(ResponseStatusEnum.BadRequest, false, _ms.MessageService(result.Message));
            }
            else
            {
                return new ApiResponse<bool>(ResponseStatusEnum.Success, true, _ms.MessageService(Message.Successfull));
            }
        }

        public async Task<ApiResponse<string>> InitOrderPayment(PayOrderDto PayDto)
        {

            var transactionCode = await _settingRepository.GetRandomNumber();

            // beh pardakht melat

            if (PayDto.PaymentType == (int)PaymentMethodEnum.Mellat)
            {

                var orderDetail = await _userOrderRepository.GetOrderDetail(PayDto.Code, true, (int)CurrencyEnum.TMN);
                // agar qeymate kalayi 100 darsad takhfif khorde bashad
                if ((double)orderDetail.Data.Total <= 0)
                {

                    PayDto.OrderId = orderDetail.Data.OrderId;
                    PayDto.Token = transactionCode.ToString();
                    PayDto.PaymentId = transactionCode.ToString();
                    PayDto.CurrencyId = (int)CurrencyEnum.TMN;
                    PayDto.PaymentType = (int)PaymentMethodEnum.Mellat;
                    var result = await _userOrderRepository.InitOrderPayment(PayDto);
                    var payOrder = new PayOrderDto();
                    payOrder.PaymentId = transactionCode.ToString();
                    payOrder.PaymentType = (int)PaymentMethodEnum.Mellat;

                    var PayOrder = await _userOrderRepository.PayOrder(payOrder);
                    if (PayOrder.Result == false)
                    {
                        return new ApiResponse<string>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(PayOrder.Message));
                    }
                    else
                    {
                        return new ApiResponse<string>(ResponseStatusEnum.Success, "https://sakhtiran.com/tmn-fa/order?paymentId=" + transactionCode + "&token=paymentGateway&PayerID=sakhtiran", _ms.MessageService(PayOrder.Message));
                    }

                }

                DateTime date = DateTime.Now;

                CultureInfo english = CultureInfo.GetCultureInfo("en-US");
                var shortDate = date.ToString("yyyyMMdd", english);
                var shortTime = date.ToString("hhmmss");

                var client = new RestClient(Configuration["Mellat:RequestUrl"]);
                var request = new RestRequest(Method.GET);
                request.AddParameter(
                "terminalId", Configuration["Mellat:TerminalId"],
                ParameterType.QueryString);
                request.AddParameter(
                "userName", Configuration["Mellat:UserName"],
                ParameterType.QueryString);
                request.AddParameter(
                "userPassword", Configuration["Mellat:Password"],
                ParameterType.QueryString);
                request.AddParameter(
                "orderId", transactionCode,
                ParameterType.QueryString);
                request.AddParameter(
               "amount", (double)orderDetail.Data.Total * 10,
               ParameterType.QueryString);
                request.AddParameter(
               "localDate", shortDate,
               ParameterType.QueryString);
                request.AddParameter(
                 "localTime", shortTime,
                 ParameterType.QueryString);
                request.AddParameter(
                "additionalData", "sakhtiran",
                ParameterType.QueryString);
                request.AddParameter(
                "callBackUrl", Configuration["RedirectUrl:returnUrl"],
                ParameterType.QueryString);
                request.AddParameter(
                "payerId", orderDetail.Data.PayerId,
                ParameterType.QueryString);
                try
                {
                    IRestResponse response = await client.ExecuteAsync(request);
                    if (response.IsSuccessful)
                    {
                        string data = Extentions.getBetweenXmlFile(response.Content, "<return>", "</return>");
                        var transactionOrderId = data.Split(",");
                        // window.location.href = 'https://bpm.shaparak.ir/pgwchannel/startpay.mellat?RefId=' + divided[1];
                        if (string.IsNullOrWhiteSpace(transactionOrderId[1]))
                        {
                            return new ApiResponse<string>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.OrderGetting));
                        }
                        PayDto.OrderId = orderDetail.Data.OrderId;
                        PayDto.Token = transactionCode.ToString();
                        PayDto.PaymentId = transactionOrderId[1];
                        PayDto.CurrencyId = (int)CurrencyEnum.TMN;
                        PayDto.PaymentType = (int)PaymentMethodEnum.Mellat;
                        var result = await _userOrderRepository.InitOrderPayment(PayDto);
                        if (result)
                        {
                            return new ApiResponse<string>(ResponseStatusEnum.Success, Configuration["Mellat:PaymentUrl"] + transactionOrderId[1], _ms.MessageService(Message.Successfull));
                        }
                        else
                        {
                            return new ApiResponse<string>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.OrderGetting));
                        }
                    }
                    else
                    {
                        return new ApiResponse<string>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.OrderGetting));
                    }
                }
                catch (System.Exception)
                {
                    return new ApiResponse<string>(ResponseStatusEnum.BadRequest, transactionCode.ToString(), _ms.MessageService(Message.OrderGetting));

                }

            }

            else
            {
                return new ApiResponse<string>(ResponseStatusEnum.Success, null, _ms.MessageService(Message.Successfull));
            }

        }

        // چک کردن سفارش درگاه ملت

        public async Task<ApiResponse<WebsiteOrderGetDto>> PayOrderMellatCheck(string refId, string resCode, string saleOrderId, string saleReferenceId)
        {



            var client = new RestClient(Configuration["Mellat:RequestCheckUrl"]);
            var request = new RestRequest(Method.GET);
            request.AddParameter(
            "terminalId", Configuration["Mellat:TerminalId"],
            ParameterType.QueryString);
            request.AddParameter(
            "userName", Configuration["Mellat:UserName"],
            ParameterType.QueryString);
            request.AddParameter(
            "userPassword", Configuration["Mellat:Password"],
            ParameterType.QueryString);
            request.AddParameter(
            "orderId", saleOrderId,
            ParameterType.QueryString);
            request.AddParameter(
           "saleOrderId", saleOrderId,
           ParameterType.QueryString);
            request.AddParameter(
           "saleReferenceId", saleReferenceId,
           ParameterType.QueryString);
            var payOrder = new PayOrderDto();
            payOrder.PaymentId = refId;
            payOrder.PaymentType = (int)PaymentMethodEnum.Mellat;
            var PayOrder = await _userOrderRepository.PayOrder(payOrder);
            if (PayOrder.Result == false)
            {
                return new ApiResponse<WebsiteOrderGetDto>(ResponseStatusEnum.BadRequest, PayOrder.Data, _ms.MessageService(PayOrder.Message));
            }
            else
            {
                IRestResponse response = await client.ExecuteAsync(request);

                if (response.IsSuccessful)
                {
                    string resultCode = Extentions.getBetweenXmlFile(response.Content, "<return>", "</return>");

                    if (resultCode == "0")
                    {
                        return new ApiResponse<WebsiteOrderGetDto>(ResponseStatusEnum.Success, PayOrder.Data, _ms.MessageService(PayOrder.Message));

                    }
                    else
                    {
                        return new ApiResponse<WebsiteOrderGetDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.OrderGetting));
                    }
                }
                else
                {
                    return new ApiResponse<WebsiteOrderGetDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.OrderGetting));
                }


            }




        }



        public async Task<ApiResponse<WebsiteOrderGetDto>> PayOrder(PayOrderDto PayDto)
        {
            var paymentTransaction = await _userOrderRepository.GetPaymentTransaction(PayDto.PaymentId);
            if (paymentTransaction == null)
            {
                return new ApiResponse<WebsiteOrderGetDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.OrderGetting));
            }
            PayDto.PaymentType = paymentTransaction.FkPaymentMethodId;
            var orderDetail = await _userOrderRepository.GetOrderWithPaymentID(PayDto.PaymentId);
            if (paymentTransaction.Status == true)
            {
                return new ApiResponse<WebsiteOrderGetDto>(ResponseStatusEnum.Success, orderDetail, _ms.MessageService(Message.Successfull));
            }

            return new ApiResponse<WebsiteOrderGetDto>(ResponseStatusEnum.BadRequest, orderDetail, _ms.MessageService(Message.Successfull));

        }

        // سفارشات پروفایل کاربر
        public async Task<ApiResponse<Pagination<ProfileOrderGetDto>>> GetProfileOrderDetail(PaginationFormDto pagination)
        {

            if (token.Id == 0 || token.Rule != UserGroupEnum.Customer)
            {
                return new ApiResponse<Pagination<ProfileOrderGetDto>>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.UserNotFoundById));
            }

            pagination.Id = token.Id;

            var order = await _userOrderRepository.GetProfileOrderDetail(pagination);
            if (order == null)
            {
                return new ApiResponse<Pagination<ProfileOrderGetDto>>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.OrderGetting));
            }
            else
            {
                var count = await _userOrderRepository.GetProfileOrderDetailCount(pagination);
                return new ApiResponse<Pagination<ProfileOrderGetDto>>(ResponseStatusEnum.Success, new Pagination<ProfileOrderGetDto>(count, order), _ms.MessageService(Message.Successfull));
            }
        }

        public async Task<ApiResponse<List<ProfileOrderItemGetDto>>> CancelOrder(List<OrderCancelingAddDto> orderCanceling)
        {
            if (orderCanceling.Select(x => x.FkOrderId).Distinct().Count() > 1)
            {
                return new ApiResponse<List<ProfileOrderItemGetDto>>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.ThisItemsIsNotFromOneOrder));
            }
            if (orderCanceling.Select(x => x.FkOrderItemId).Distinct().Count() != orderCanceling.Select(x => x.FkOrderItemId).Count())
            {
                return new ApiResponse<List<ProfileOrderItemGetDto>>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.ItemsAreDuplicates));
            }
            var mapCancel = _mapper.Map<List<TOrderCanceling>>(orderCanceling);
            foreach (var item in mapCancel)
            {
                item.CancelDate = DateTime.Now;
            }
            var result = await _userOrderRepository.CancelOrder(mapCancel);
            if (result != null && result.Result == false)
            {
                return new ApiResponse<List<ProfileOrderItemGetDto>>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(result.Message));
            }
            else
            {
                return new ApiResponse<List<ProfileOrderItemGetDto>>(ResponseStatusEnum.Success, result.Data, _ms.MessageService(Message.Successfull));
            }
        }

        public async Task<ApiResponse<bool>> ReturnOrder(OrderReturningAddDto orderReturning)
        {
            var mapReturn = _mapper.Map<TOrderReturning>(orderReturning);
            mapReturn.RegisterDateTime = DateTime.Now;
            var result = await _userOrderRepository.ReturnOrder(mapReturn);
            if (result.Result == false)
            {
                return new ApiResponse<bool>(ResponseStatusEnum.BadRequest, false, _ms.MessageService(result.Message));
            }
            else
            {
                return new ApiResponse<bool>(ResponseStatusEnum.Success, true, _ms.MessageService(Message.Successfull));
            }
        }

        public async Task<ApiResponse<ProfileOrderGetDto>> GetProfileOrderItem(int orderId)
        {
            if (token.Id == 0 || token.Rule != UserGroupEnum.Customer)
            {
                return new ApiResponse<ProfileOrderGetDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.UserNotFoundById));
            }

            var result = await _userOrderRepository.GetProfileOrderItem(orderId);
            if (result == null)
            {
                return new ApiResponse<ProfileOrderGetDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.OrderGetting));
            }
            else
            {
                return new ApiResponse<ProfileOrderGetDto>(ResponseStatusEnum.Success, result, _ms.MessageService(Message.Successfull));
            }
        }
        public async Task<ApiResponse<List<ProfileOrderItemGetDto>>> ProfileOrdersItemReturned()
        {
            if (token.Id == 0 || token.Rule != UserGroupEnum.Customer)
            {
                return new ApiResponse<List<ProfileOrderItemGetDto>>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.UserNotFoundById));
            }

            var result = await _userOrderRepository.ProfileOrdersItemReturned();
            if (result == null)
            {
                return new ApiResponse<List<ProfileOrderItemGetDto>>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.OrderGetting));
            }
            else
            {
                return new ApiResponse<List<ProfileOrderItemGetDto>>(ResponseStatusEnum.Success, result, _ms.MessageService(Message.Successfull));
            }
        }

        public async Task<ApiResponse<Pagination<ProfileOrderItemReturnGetDto>>> GetProfileReturnRequested(PaginationFormDto pagination, int type)
        {
            if (token.Id == 0 || token.Rule != UserGroupEnum.Customer)
            {
                return new ApiResponse<Pagination<ProfileOrderItemReturnGetDto>>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.UserNotFoundById));
            }

            pagination.Id = token.Id;

            var order = await _userOrderRepository.GetProfileReturnRequested(pagination, type);
            if (order == null)
            {
                return new ApiResponse<Pagination<ProfileOrderItemReturnGetDto>>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.OrderGetting));
            }
            else
            {
                var count = await _userOrderRepository.GetProfileReturnRequestedCount(pagination, type);
                return new ApiResponse<Pagination<ProfileOrderItemReturnGetDto>>(ResponseStatusEnum.Success, new Pagination<ProfileOrderItemReturnGetDto>(count, order), _ms.MessageService(Message.Successfull));
            }
        }

        public async Task<ApiResponse<CustomerOrderCountDto>> GetCustomerOrderCount()
        {
            if (token.Id == 0 || token.Rule != UserGroupEnum.Customer)
            {
                return new ApiResponse<CustomerOrderCountDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.UserNotFoundById));
            }

            var result = await _userOrderRepository.GetCustomerOrderCount();
            if (result == null)
            {
                return new ApiResponse<CustomerOrderCountDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.OrderGetting));
            }
            else
            {
                return new ApiResponse<CustomerOrderCountDto>(ResponseStatusEnum.Success, result, _ms.MessageService(Message.Successfull));
            }
        }

        public async Task<ApiResponse<ProfileOrderItemGetDto>> ProfileProductReturned(int ItemId)
        {
            if (token.Id == 0 || token.Rule != UserGroupEnum.Customer)
            {
                return new ApiResponse<ProfileOrderItemGetDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.UserNotFoundById));
            }

            var result = await _userOrderRepository.ProfileProductReturned(ItemId);
            if (result == null)
            {
                return new ApiResponse<ProfileOrderItemGetDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.OrderGetting));
            }
            else
            {
                return new ApiResponse<ProfileOrderItemGetDto>(ResponseStatusEnum.Success, result, _ms.MessageService(Message.Successfull));
            }
        }

        public async Task<ApiResponse<PayOrderDto>> CheckPayOrder(long OrderId)
        {
            if (token.Id == 0 || token.Rule != UserGroupEnum.Customer)
            {
                return new ApiResponse<PayOrderDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.UserNotFoundById));
            }
            var paymentTransaction = await _userOrderRepository.GetPaymentTransactionWithOrderId(OrderId);

            if (paymentTransaction == null)
            {
                return new ApiResponse<PayOrderDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.OrderGetting));
            }
            if (paymentTransaction.Status != true)
            {
                return new ApiResponse<PayOrderDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.OrderGetting));
            }
            var payInfo = new PayOrderDto();
            payInfo.OrderId = (long)paymentTransaction.FkOrderId;
            payInfo.PayerID = paymentTransaction.PayerId;
            payInfo.Token = paymentTransaction.PaymentToken;
            payInfo.PaymentId = paymentTransaction.PaymentId;

            return new ApiResponse<PayOrderDto>(ResponseStatusEnum.Success, payInfo, _ms.MessageService(Message.Successfull));

        }



        // allowed goods list for cancel

        public async Task<ApiResponse<List<ProfileOrderItemGetDto>>> ProfileOrdersItemCanceled(long orderId)
        {
            if (token.Id == 0 || token.Rule != UserGroupEnum.Customer)
            {
                return new ApiResponse<List<ProfileOrderItemGetDto>>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.UserNotFoundById));
            }

            var result = await _userOrderRepository.ProfileOrdersItemCanceled(orderId);
            if (result == null)
            {
                return new ApiResponse<List<ProfileOrderItemGetDto>>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.OrderGetting));
            }
            else
            {
                return new ApiResponse<List<ProfileOrderItemGetDto>>(ResponseStatusEnum.Success, result, _ms.MessageService(Message.Successfull));
            }
        }



    }
}