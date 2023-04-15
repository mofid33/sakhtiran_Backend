using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MarketPlace.API.Data.Models;
using MarketPlace.API.Data.Repositories.IRepository;
using MarketPlace.API.Helper;
using MarketPlace.API.Services.IService;
using MarketPlace.API.Data.Enums;
using MarketPlace.API.Data.Dtos.Survey;
using MarketPlace.API.Data.Dtos.Header;
using MarketPlace.API.Data.Dtos.Token;
using Microsoft.AspNetCore.Http;
using MarketPlace.API.Data.Dtos.Shop;
using MarketPlace.API.Data.Dtos.Customer;
using RestSharp;
using Newtonsoft.Json.Linq;
using MarketPlace.API.Data.Dtos.Transaction;
using MarketPlace.API.Data.Dtos.Pagination;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using MarketPlace.API.Data.Dtos.Google;

namespace MarketPlace.API.Services.Service
{
    public class UserActivityService : IUserActivityService
    {
        public IMapper _mapper { get; }
        public IUserActivityRepository _userActivityRepository { get; }
        public IGoodsRepository _gooodsRepository { get; }
        public IShopRepository _shopRepository { get; }
        public IAuthRepository _authRepository { get; }
        public ICategoryRepository _categoryRepository { get; }
        public IDocumentTypeRepository _documentTypeRepository { get; }
        public IShopSurveyQuestionsRepository _shopSurveyQuestionsRepository { get; }
        public HeaderParseDto header { get; set; }
        public TokenParseDto token { get; set; }
        public IMessageLanguageService _ms { get; set; }
        public IFileUploadService _fileUploadService { get; }
        public IUserOrderRepository _userOrderRepository { get; }
        public IVerificationRepository _verficationRepository { get; }
        public INotificationService _notificationService { get; }
        public IConfiguration Configuration { get; }
        public IMessageRepository _messageRepository { get; }

        public UserActivityService(
        IMapper mapper,
        ICategoryRepository categoryRepository,
        IUserActivityRepository userActivityRepository,
        IUserOrderRepository userOrderRepository
        , IGoodsRepository gooodsRepository
        , IDocumentTypeRepository documentTypeRepository
        , IShopSurveyQuestionsRepository shopSurveyQuestionsRepository,
        IMessageLanguageService ms,
        IFileUploadService fileUploadService,
        IShopRepository shopRepository,
        IAuthRepository authRepository,
        IConfiguration Configuration,
        IVerificationRepository verificationRepository,
        INotificationService notificationService,
        IMessageRepository messageRepository,
        IHttpContextAccessor httpContextAccessor)
        {
            this._categoryRepository = categoryRepository;
            this._shopSurveyQuestionsRepository = shopSurveyQuestionsRepository;
            this._gooodsRepository = gooodsRepository;
            this._documentTypeRepository = documentTypeRepository;
            this._userActivityRepository = userActivityRepository;
            this._shopRepository = shopRepository;
            this._authRepository = authRepository;
            this._mapper = mapper;
            this._fileUploadService = fileUploadService;
            this.Configuration = Configuration;
            _verficationRepository = verificationRepository;
            _notificationService = notificationService;
            this._messageRepository = messageRepository;
            header = new HeaderParseDto(httpContextAccessor);
            token = new TokenParseDto(httpContextAccessor);
            _userOrderRepository = userOrderRepository;
            _ms = ms;
        }
        public async Task<ApiResponse<bool>> LikeGoodsAdd(int goodsId)
        {

            var result = await _userActivityRepository.LikeGoodsAdd(token.Id, goodsId);
            if (result == false)
            {
                return new ApiResponse<bool>(ResponseStatusEnum.BadRequest, result, _ms.MessageService(Message.LikeGoods));
            }
            return new ApiResponse<bool>(ResponseStatusEnum.Success, result, _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<bool>> ViewGoodsAdd(int goodsId, string ipAddress)
        {
            var result = await _userActivityRepository.ViewGoodsAdd(token.Id, goodsId, ipAddress);
            if (result == false)
            {
                return new ApiResponse<bool>(ResponseStatusEnum.BadRequest, result, _ms.MessageService(Message.ViewGoods));
            }
            return new ApiResponse<bool>(ResponseStatusEnum.Success, result, _ms.MessageService(Message.Successfull));
        }

        ///// ثبت نظر کاربر
        public async Task<ApiResponse<GoodsCommentAddDto>> GoodsCommentAdd(GoodsCommentAddDto goodsComment)
        {
            goodsComment.CustomerId = token.Id;
            var result = await _userActivityRepository.GoodsCommentAdd(goodsComment);
            if (result == null)
            {
                return new ApiResponse<GoodsCommentAddDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.CommentAdd));
            }
            var mapResultCustommerComment = _mapper.Map<GoodsCommentAddDto>(result);
            return new ApiResponse<GoodsCommentAddDto>(ResponseStatusEnum.Success, mapResultCustommerComment, _ms.MessageService(Message.Successfull));
        }


        public async Task<ApiResponse<UserGoodsCommentDto>> GetCustomerGoodsComment(int goodsId, long orderItemId)
        {
            var data = new UserGoodsCommentDto();
            var comments = await _userActivityRepository.GetCustomerGoodsComment(orderItemId, token.Id);
            if (comments == null)
            {
                data.Comment = null;
            }
            else
            {
                data.Comment = comments;
            }
            data.Goods = await _gooodsRepository.GetGoodsBaseDetailDto(goodsId);
            data.Goods.ShopName = comments.ShopName;
            var questions = await _shopSurveyQuestionsRepository.ShopSurveyQuestionsGetAll(true);
            if (questions.Count > 0)
            {
                data.Questions = questions;
            }

            return new ApiResponse<UserGoodsCommentDto>(ResponseStatusEnum.Success, data, _ms.MessageService(Message.Successfull));
        }


        // ثبت نام تامین کننده

        public async Task<ApiResponse<ShopRegisterDto>> RegisterProvider(ShopRegisterSerializeDto shopDto)
        {

            var shopObj = Extentions.Deserialize<ShopRegisterDto>(shopDto.Shop);
            if (shopObj == null)
            {
                return new ApiResponse<ShopRegisterDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.ShopDeserialize));
            }


            // if (shopDto.FilesDto == null)
            // {
            //     return new ApiResponse<ShopRegisterDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.UploadFile));
            // }
            var userExist = await _authRepository.UserGetByUsername(shopObj.Email, (int)UserGroupEnum.Seller);
            if (userExist != null)
            {
                return new ApiResponse<ShopRegisterDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.UserNameDupplicate));
            }

            if (shopDto.FilesDto != null)
            {

                if (shopDto.FilesDto.Count != shopObj.TShopFiles.Count)
                {
                    return new ApiResponse<ShopRegisterDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.UploadFile));
                }
                for (int i = 0; i < shopDto.FilesDto.Count; i++)
                {
                    shopObj.TShopFiles[i].FileUrl = _fileUploadService.UploadImage(shopDto.FilesDto[i], Pathes.Shop + "/" + Pathes.DocsTemp);
                    if (shopObj.TShopFiles[i].FileUrl == null)
                    {
                        return new ApiResponse<ShopRegisterDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.UploadFile));
                    }
                }
            }

            var mapProvider = _mapper.Map<TShop>(shopObj);
            mapProvider.Phone = "+" + shopObj.PhoneCode + shopObj.Phone;
            var addData = await _shopRepository.RegisterShop(mapProvider);

            if (addData.Data == null)
            {
                foreach (var item in shopObj.TShopFiles)
                {
                    _fileUploadService.DeleteImage(item.FileUrl, Pathes.Shop + "/" + Pathes.DocsTemp);
                }
                return new ApiResponse<ShopRegisterDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(addData.Message));
            }
            else
            {

                for (int i = 0; i < shopObj.TShopFiles.Count; i++)
                {
                    var isMoved = _fileUploadService.ChangeDestOfFile(shopObj.TShopFiles[i].FileUrl, Pathes.Shop + "/" + Pathes.DocsTemp, Pathes.Shop + addData.Data.ShopId + "/" + Pathes.Docs);
                    if (!isMoved)
                    {
                        _fileUploadService.DeleteImage(shopObj.TShopFiles[i].FileUrl, Pathes.Shop + "/" + Pathes.DocsTemp);
                        return new ApiResponse<ShopRegisterDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.UploadFile));
                    }
                }
            }
            var user = new TUser();
            user.FkShopId = addData.Data.ShopId;
            user.UserName = addData.Data.Email;
            user.FkUserGroupId = Guid.Parse(GroupTypes.Seller);
            user.Active = true;
            byte[] passwordHash, passwordSalt;
            Extentions.CreatePasswordHash(shopObj.Password, out passwordHash, out passwordSalt);
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
            var us = await _authRepository.UserAdd(user);
            if (us == null)
            {
                await _shopRepository.DeleteShop(addData.Data.ShopId);
                return new ApiResponse<ShopRegisterDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.CustomerIncompleteRegistration));
            }
            if (token.Rule != UserGroupEnum.Admin)
            {
              await  _notificationService.SendNotification((int)NotificationSettingTypeEnum.AfterRegisterProvider, null, null, user.UserName, mapProvider.Phone , us.UserId);
                // ارسال پیام به مدیر وقتی فروشنده کالای جدید ثبت میکنند
                await _messageRepository.SendMessageToAdmin(shopObj.StoreName + " - ثبت نام تامین کننده ی جدید", shopObj.StoreName + " - تامین کننده ی جدید");

            }
            return new ApiResponse<ShopRegisterDto>(ResponseStatusEnum.Success, null, _ms.MessageService(addData.Message));

        }


        public async Task<ApiResponse<CustomerSMSDto>> CheckShopEmail(string email, string mobileNumber, bool CheckMobileNumber, string captchaToken)
        {

            // برای موبایل
            // if (captchaToken != Configuration["Recaptcha:secretKey"])
            // {
            //     var client = new RestClient("https://www.google.com/recaptcha/api/siteverify");
            //     var request = new RestRequest(Method.GET);
            //     request.AddParameter(
            //         "secret", Configuration["Recaptcha:secretKey"],
            //         ParameterType.QueryString);
            //     request.AddParameter(
            //        "response", captchaToken,
            //        ParameterType.QueryString);
            //     IRestResponse response = await client.ExecuteAsync(request);

            //     var resource = JsonConvert.DeserializeObject<GoogleRecaptchDto>(response.Content);
            //     if (!resource.success)
            //     {
            //         return new ApiResponse<CustomerSMSDto>(ResponseStatusEnum.BadRequest, null, Message.RecaptchError);
            //     }
            // }
            var userExist = await _authRepository.UserGetByUsername(email, (int)UserGroupEnum.Seller);
            if (userExist != null)
            {
                return new ApiResponse<CustomerSMSDto>(ResponseStatusEnum.Success, null, _ms.MessageService(Message.UserNameDupplicate));
            }
            else if (CheckMobileNumber)
            {
                Random random = new Random();
                int verficationCode = random.Next(1000, 9999);
                var resultSMS = await _verficationRepository.VarifyEmailOrSms(null, verficationCode, (int)VerificationTypeEnum.Varify, mobileNumber);

                if (resultSMS == (int)VerficationEnum.MemberTryedError)
                {
                    return new ApiResponse<CustomerSMSDto>(ResponseStatusEnum.Success, null, _ms.MessageService(Message.BlockUserSms));
                }
                else if (resultSMS == (int)VerficationEnum.SystemError)
                {
                    return new ApiResponse<CustomerSMSDto>(ResponseStatusEnum.Success, null, _ms.MessageService(Message.SystemErrorSendSms));
                }
                var smsResult = new CustomerSMSDto();
                smsResult.RequestId = "123123";
                if (header.Currency == CurrencyEnum.TMN.ToString())
                {
                    Extentions.SendPodinisVerficationCode(verficationCode.ToString(), mobileNumber);
                }
                else
                {
                    smsResult = await Extentions.SendSMS(mobileNumber, Configuration["SmsConfig:apiKey"], Configuration["SmsConfig:apiSecret"]);
                }
                return new ApiResponse<CustomerSMSDto>(ResponseStatusEnum.Success, smsResult, _ms.MessageService(Message.Successfull));
            }
            else
            {
                return new ApiResponse<CustomerSMSDto>(ResponseStatusEnum.Success, null, _ms.MessageService(Message.Successfull));
            }

        }


        public async Task<ApiResponse<CustomerAddressDto>> UpdateCustomerAddress(CustomerAddressDto customerAddress, bool forOrder)
        {
            var mapCustomerAddress = _mapper.Map<TCustomerAddress>(customerAddress);
            mapCustomerAddress.FkCustomerId = token.Id;
            var updateAddress = await _userActivityRepository.UpdateCustomerAddress(mapCustomerAddress, forOrder);
            if (updateAddress == null)
            {
                return new ApiResponse<CustomerAddressDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.CustomerAddressEditing));
            }
            return new ApiResponse<CustomerAddressDto>(ResponseStatusEnum.Success, updateAddress, _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<List<CustomerAddressDto>>> GetCustomerAddress(string type)
        {
            var data = await _userActivityRepository.GetCustomerAddress(type);
            if (data == null)
            {
                return new ApiResponse<List<CustomerAddressDto>>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.CustomerAddressGetting));
            }
            return new ApiResponse<List<CustomerAddressDto>>(ResponseStatusEnum.Success, data, _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<CustomerSMSDto>> AddCustomerAddress(CustomerAddressDto customerAddress, bool forCart, long? orderId)
        {
            var mapCustomerAddress = _mapper.Map<TCustomerAddress>(customerAddress);
            mapCustomerAddress.FkCustomerId = token.Id;
            var result = await _userActivityRepository.AddCustomerAddress(mapCustomerAddress, forCart, orderId);
            if (result == null)
            {
                return new ApiResponse<CustomerSMSDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.CustomerAddressAdding));
            }
            if (orderId != null)
            {
                var changeResult = await _userOrderRepository.ChangeDestination((long)orderId, forCart, result.AddressId);
                return new ApiResponse<CustomerSMSDto>(ResponseStatusEnum.Success, null, changeResult.Message);
            }
            var addressPhoneCode = await _userActivityRepository.GetPhoneCodeWithCustomerAddress(result.AddressId);

            // برای پروژه ی پودینس
            if (header.Currency == CurrencyEnum.TMN.ToString())
            {
                Random random = new Random();
                int verficationCode = random.Next(1000, 9999);
                Extentions.SendPodinisVerficationCode(verficationCode.ToString(), "0" + mapCustomerAddress.TransfereeMobile);
                var resultSMS = await _verficationRepository.VarifyEmailOrSms(null, verficationCode, (int)VerificationTypeEnum.Varify, addressPhoneCode + mapCustomerAddress.TransfereeMobile);
                var finalResult = new CustomerSMSDto();
                finalResult.AddressId = result.AddressId;
                finalResult.RequestId = verficationCode.ToString();
                return new ApiResponse<CustomerSMSDto>(ResponseStatusEnum.Success, finalResult, _ms.MessageService(Message.Successfull));

                // برای پودینس
            }
            else
            {
                var smsResult = await Extentions.SendSMS(addressPhoneCode + mapCustomerAddress.TransfereeMobile, Configuration["SmsConfig:apiKey"], Configuration["SmsConfig:apiSecret"]);
                smsResult.AddressId = result.AddressId;
                return new ApiResponse<CustomerSMSDto>(ResponseStatusEnum.Success, smsResult, _ms.MessageService(Message.Successfull));
            }


        }

        public async Task<ApiResponse<bool>> DeleteCustomerAddress(int addressId)
        {
            var result = await _userActivityRepository.DeleteCustomerAddress(addressId, token.Id);
            if (result == false)
            {
                return new ApiResponse<bool>(ResponseStatusEnum.BadRequest, result, _ms.MessageService(Message.CustomerAddressDeleteing));
            }
            return new ApiResponse<bool>(ResponseStatusEnum.Success, result, _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<CustomerSMSDto>> AddProfileCustomerAddress(CustomerAddressDto customerAddress)
        {
            customerAddress.IsDefualt = false;
            var mapCustomerAddress = _mapper.Map<TCustomerAddress>(customerAddress);
            mapCustomerAddress.FkCustomerId = token.Id;
            var result = await _userActivityRepository.AddProfileCustomerAddress(mapCustomerAddress);
            if (result == null)
            {
                return new ApiResponse<CustomerSMSDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.CustomerAddressAdding));
            }
            var addressPhoneCode = await _userActivityRepository.GetPhoneCodeWithCustomerAddress(result.AddressId);
            // برای پروژه ی پودینس
            if (header.Currency == CurrencyEnum.TMN.ToString())
            {
                Random random = new Random();
                int verficationCode = random.Next(1000, 9999);
                Extentions.SendPodinisVerficationCode(verficationCode.ToString(), "0" + mapCustomerAddress.TransfereeMobile);
                var resultSMS = await _verficationRepository.VarifyEmailOrSms(null, verficationCode, (int)VerificationTypeEnum.Varify, addressPhoneCode + mapCustomerAddress.TransfereeMobile);
                var finalResult = new CustomerSMSDto();
                finalResult.AddressId = result.AddressId;
                finalResult.RequestId = verficationCode.ToString();
                return new ApiResponse<CustomerSMSDto>(ResponseStatusEnum.Success, finalResult, _ms.MessageService(Message.Successfull));

                // برای پودینس
            }
            else
            {
                var smsResult = await Extentions.SendSMS(addressPhoneCode + mapCustomerAddress.TransfereeMobile, Configuration["SmsConfig:apiKey"], Configuration["SmsConfig:apiSecret"]);
                smsResult.AddressId = result.AddressId;

                return new ApiResponse<CustomerSMSDto>(ResponseStatusEnum.Success, smsResult, _ms.MessageService(Message.Successfull));


            }

        }


        // چک کردن اینکه که شهر انتخابی با سفارش یکی است یا نه
        public async Task<ApiResponse<CustomerAddressDto>> CheckAddressArea(CustomerMapAddressDto customerAddress)
        {

            var orderData = await _userOrderRepository.GetOrderCountryAndCity();
            var client = new RestClient("https://maps.googleapis.com/maps/api/geocode/json");
            var request = new RestRequest(Method.GET);
            request.AddParameter(
                "latlng", customerAddress.Lat + "," + customerAddress.Long,
                ParameterType.QueryString);
            request.AddParameter(
               "key", "AIzaSyDA1IUurSPV52x4PbyUM3aeVA3AAIEtAwo",
               ParameterType.QueryString);
            request.AddParameter(
            "language", header.LanguageNum.ToString(),
            ParameterType.QueryString);
            IRestResponse response = await client.ExecuteAsync(request);

            var resource = JObject.Parse(response.Content);
            JArray datass = (JArray)resource["results"][1]["address_components"];
            IList<CustomerDecodeAddressDto> locationDetils = datass.ToObject<IList<CustomerDecodeAddressDto>>();
            var cityObject = locationDetils.FirstOrDefault(x => x.types[0] == "locality");
            var cityTitle = "";
            if (cityObject != null)
            {
                cityTitle = cityObject.long_name.Trim();
            }
            else
            {
                return new ApiResponse<CustomerAddressDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.OrderCantBeSentToThisAddress));
            }
            var countryObject = locationDetils.FirstOrDefault(x => x.types[0] == "country");
            var countryTitle = "";
            if (countryObject != null)
            {
                countryTitle = countryObject.long_name.Trim();
            }
            else
            {
                return new ApiResponse<CustomerAddressDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.OrderCantBeSentToThisAddress));
            }
            string address = (string)resource["results"][0]["formatted_address"];
            var country = orderData.Split(",")[0].Trim();
            var city = orderData.Split(",")[1].Trim();
            var phoneCode = orderData.Split(",")[2].Trim();
            var iso = orderData.Split(",")[3].Trim();
            if (cityTitle == city && country == countryTitle)
            {
                var customerRess = new CustomerAddressDto();
                customerRess.LocationX = customerAddress.Lat;
                customerRess.LocationY = customerAddress.Long;
                customerRess.Address = address;
                customerRess.CityName = cityTitle;
                customerRess.CountryName = countryTitle;
                customerRess.PhoneCode = phoneCode;
                customerRess.Iso = iso;
                return new ApiResponse<CustomerAddressDto>(ResponseStatusEnum.Success, customerRess, _ms.MessageService(Message.Successfull));

            }
            else
            {
                return new ApiResponse<CustomerAddressDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.OrderCantBeSentToThisAddress));
            }

        }


        // ajyal credit
        public async Task<ApiResponse<UserTransactionWebGetDto>> GetProfileAjyalCredit(PaginationFormDto pagination)
        {
            if (token.Rule == UserGroupEnum.Customer)
            {
                pagination.Id = token.Id;
            }
            var data = await _userActivityRepository.GetProfileAjyalCredit(pagination);
            if (data == null)
            {
                return new ApiResponse<UserTransactionWebGetDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.TransactionTypeGetting));
            }
            data.Count = await _userActivityRepository.GetProfileAjyalCreditCount(pagination);
            return new ApiResponse<UserTransactionWebGetDto>(ResponseStatusEnum.Success,
            data, _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<bool>> SetDefualtAddress(int addressId)
        {
            if (token.Id == 0 || token.Rule != UserGroupEnum.Customer)
            {
                return new ApiResponse<bool>(ResponseStatusEnum.BadRequest, false, _ms.MessageService(Message.UserNotFoundById));
            }

            var result = await _userActivityRepository.SetDefualtAddress(addressId);
            if (!result)
            {
                return new ApiResponse<bool>(ResponseStatusEnum.BadRequest, false, _ms.MessageService(Message.SetDefualtAddress));
            }
            return new ApiResponse<bool>(ResponseStatusEnum.Success, true, Message.Successfull);
        }

        public async Task<ApiResponse<CustomerSMSDto>> ChangeMobileNumberAddress(int addressId, string mobileNumber)
        {
            if (token.Id == 0 || token.Rule != UserGroupEnum.Customer)
            {
                return new ApiResponse<CustomerSMSDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.UserNotFoundById));
            }

            var result = await _userActivityRepository.ChangeMobileNumberAddress(addressId, mobileNumber);
            if (!result)
            {
                return new ApiResponse<CustomerSMSDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.ChangeCustomerMobileNumber));
            }
            var addressPhoneCode = await _userActivityRepository.GetPhoneCodeWithCustomerAddress(addressId);
            // برای پروژه ی پودینس
            if (header.Currency == CurrencyEnum.TMN.ToString())
            {
                Random random = new Random();
                int verficationCode = random.Next(1000, 9999);
                Extentions.SendPodinisVerficationCode(verficationCode.ToString(), "0" + mobileNumber);
                var resultSMS = await _verficationRepository.VarifyEmailOrSms(null, verficationCode, (int)VerificationTypeEnum.Varify, addressPhoneCode + mobileNumber);
                var finalResult = new CustomerSMSDto();
                finalResult.RequestId = verficationCode.ToString();
                return new ApiResponse<CustomerSMSDto>(ResponseStatusEnum.Success, finalResult, _ms.MessageService(Message.Successfull));

                // برای پودینس
            }
            else
            {
                var smsResult = await Extentions.SendSMS(addressPhoneCode + mobileNumber, Configuration["SmsConfig:apiKey"], Configuration["SmsConfig:apiSecret"]);

                return new ApiResponse<CustomerSMSDto>(ResponseStatusEnum.Success, smsResult, _ms.MessageService(Message.Successfull));

            }

        }

        public async Task<ApiResponse<CustomerSMSDto>> CheckVerifyMobileNumber(int addressId, string code, string requestId, bool forRegisterProvider, string mobileNumber)
        {
            if ((token.Id == 0 || token.Rule != UserGroupEnum.Customer) && forRegisterProvider == false)
            {
                return new ApiResponse<CustomerSMSDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.UserNotFoundById));
            }
            var smsResult = new CustomerSMSDto();
            if (header.Currency == CurrencyEnum.TMN.ToString())
            {
                var addressMobileNumber = "";
                var addressMobileNumberPhoneCode = "";
                if (forRegisterProvider == false)
                {
                    addressMobileNumber = await _userActivityRepository.GetMobileNumberWithCustomerAddress(addressId);
                    addressMobileNumberPhoneCode = await _userActivityRepository.GetPhoneCodeWithCustomerAddress(addressId);
                }

                var resultVerify = await _verficationRepository.CheckVarifyEmailOrSms(null, Int32.Parse(code), (int)VerificationTypeEnum.Varify, forRegisterProvider == false ? addressMobileNumberPhoneCode+addressMobileNumber : mobileNumber);
                if (resultVerify != 1)
                {
                    return new ApiResponse<CustomerSMSDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.VerifyCustomerCodeMobileNumber));
                }
            }
            else
            {
                smsResult = await Extentions.VerifySMS(code, requestId, Configuration["SmsConfig:apiKey"], Configuration["SmsConfig:apiSecret"]);
                if (smsResult.Status != "0")
                {
                    return new ApiResponse<CustomerSMSDto>(ResponseStatusEnum.BadRequest, smsResult, _ms.MessageService(Message.VerifyCustomerCodeMobileNumber));
                }

            }

            // important: sms change

            if (forRegisterProvider == false)
            {
                var result = await _userActivityRepository.SetCustomerMobileVerify(addressId);
                if (!result)
                {
                    return new ApiResponse<CustomerSMSDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.VerifyCustomerMobileNumber));
                }

            }

            return new ApiResponse<CustomerSMSDto>(ResponseStatusEnum.Success, smsResult, _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<bool>> CustomerEmailVerify(string email, int code)
        {
            if (token.Id == 0 || token.Rule != UserGroupEnum.Customer)
            {
                return new ApiResponse<bool>(ResponseStatusEnum.BadRequest, false, _ms.MessageService(Message.UserNotFoundById));
            }
            var result = await _verficationRepository.CheckVarifyEmailOrSms(email, code, (int)VerificationTypeEnum.Varify, null);

            if (result == (int)VerficationEnum.expired)
            {
                return new ApiResponse<bool>(ResponseStatusEnum.BadRequest, false, _ms.MessageService(Message.Expired));
            }
            else if (result == (int)VerficationEnum.NotVerifyed)
            {
                return new ApiResponse<bool>(ResponseStatusEnum.BadRequest, false, _ms.MessageService(Message.NotVerifyed));
            }

            else if (result == (int)VerficationEnum.SystemError)
            {
                return new ApiResponse<bool>(ResponseStatusEnum.BadRequest, true, _ms.MessageService(Message.SystemErrorSendEmail));

            }
            else
            {
                var verify = await _userActivityRepository.CustomerEmailVerify(email);
                return new ApiResponse<bool>(ResponseStatusEnum.Success, true, _ms.MessageService(Message.Successfull));

            }

        }

        public async Task<ApiResponse<ShopGeneralDto>> CallRequestGoodsAdd(int goodsId, int providerId)
        {
            if (token.Id == 0 || token.Rule != UserGroupEnum.Customer)
            {
                return new ApiResponse<ShopGeneralDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.UserNotFoundById));
            }

            var result = await _userActivityRepository.CallRequestGoodsAdd(token.Id, goodsId, providerId);
            return new ApiResponse<ShopGeneralDto>(ResponseStatusEnum.Success, result, _ms.MessageService(Message.Successfull));

        }


    }
}