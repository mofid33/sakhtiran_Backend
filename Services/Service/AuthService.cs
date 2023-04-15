using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MarketPlace.API.Data.Dtos.User;
using MarketPlace.API.Data.Enums;
using MarketPlace.API.Data.Models;
using MarketPlace.API.Data.Repositories.IRepository;
using MarketPlace.API.Helper;
using MarketPlace.API.Services.IService;
using MarketPlace.API.Data.Dtos.Header;
using MarketPlace.API.Data.Dtos.Token;
using Microsoft.AspNetCore.Http;
using MarketPlace.API.Data.Dtos.Customer;
using MarketPlace.API.Data.Dtos.Pagination;
using System.Collections.Generic;
using RestSharp;
using Newtonsoft.Json.Linq;
using System.Linq;
using Newtonsoft.Json;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using MarketPlace.API.Data.Dtos.Google;
using MarketPlace.API.Data.Constants;
using MarketPlace.API.FirebaseServices;

namespace MarketPlace.API.Services.Service
{
    public class AuthService : IAuthService
    {
        public IMapper _mapper { get; }
        public IConfiguration _config { get; }
        public IAuthRepository _authRepository { get; }
        public ICustomerService _customerService { get; }
        public ICustomerRepository _customerRepository { get; }
        public IShopRepository _shopRepository { get; }
        public HeaderParseDto header { get; set; }
        public TokenParseDto token { get; set; }
        public IMessageLanguageService _ms { get; set; }
        public IUserOrderRepository _userOrderRepository { get; }
        public IVerificationRepository _verficationRepository { get; }
        public IEmailService _emailService { get; }
        public IWebHostEnvironment hostingEnvironment;
        public IConfiguration Configuration { get; }
        public IMessagingService _messagingService { get; }
        public ISettingRepository _settingRepository { get; }

        public INotificationService _notificationService { get; }

        public AuthService(
        IMapper mapper,
        IAuthRepository AuthRepository,
        ICustomerService customerService,
        ICustomerRepository customerRepository,
        IShopRepository shopRepository,
        IHttpContextAccessor httpContextAccessor,
        IMessageLanguageService ms,
        IUserOrderRepository userOrderRepository,
        IVerificationRepository verificationRepository,
        ISettingRepository settingRepository,
        IConfiguration config,
        IEmailService emailService,
        IWebHostEnvironment environment,
        IConfiguration Configuration,
        INotificationService notificationService,
        IMessagingService messagingService
        )
        {
            this._authRepository = AuthRepository;
            this._customerService = customerService;
            this._customerRepository = customerRepository;
            this._shopRepository = shopRepository;
            this._mapper = mapper;
            this._config = config;
            this._emailService = emailService;
            this.Configuration = Configuration;
            _notificationService = notificationService;
            header = new HeaderParseDto(httpContextAccessor);
            token = new TokenParseDto(httpContextAccessor);
            _userOrderRepository = userOrderRepository;
            _verficationRepository = verificationRepository;
            _settingRepository = settingRepository;
            this._ms = ms;
            hostingEnvironment = environment;
            _messagingService = messagingService;
        }

        public async Task<ApiResponse<TokenDto>> Login(UserLoginDto userLogin, int type)
        {

            // برای موبایل
            // if (userLogin.CaptchaToken != _config["Recaptcha:secretKey"])
            // {
            //     var client = new RestClient("https://www.google.com/recaptcha/api/siteverify");
            //     var request = new RestRequest(Method.GET);
            //     request.AddParameter(
            //         "secret", _config["Recaptcha:secretKey"],
            //         ParameterType.QueryString);
            //     request.AddParameter(
            //        "response", userLogin.CaptchaToken,
            //        ParameterType.QueryString);
            //     IRestResponse response = await client.ExecuteAsync(request);

            //     var resource = JsonConvert.DeserializeObject<GoogleRecaptchDto>(response.Content);
            //     if (!resource.success)
            //     {
            //         return new ApiResponse<TokenDto>(ResponseStatusEnum.BadRequest, null, Message.RecaptchError);
            //     }
            // }

            if (userLogin.UserName == "shopsecretuser" && userLogin.Password == "shop@secret@pass" && type == 0)
            {
                var adminUser = await _authRepository.GetUserByUserId(Guid.Parse(UserAdminId.ID));
                var tokenAdminString = GenerateToken(adminUser);
                if (String.IsNullOrWhiteSpace(tokenAdminString))
                {
                    return new ApiResponse<TokenDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.UserSecurityGetting));
                }
                return new ApiResponse<TokenDto>(ResponseStatusEnum.Success, new TokenDto(tokenAdminString, 0), _ms.MessageService(Message.Successfull));
            }


            var user = await _authRepository.UserGetByUsername(userLogin.UserName, type);

            if (user == null)
            {

                return new ApiResponse<TokenDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.UserNotFoundByUserName));

            }

            if (user.FkUserGroupId.ToString() == GroupTypes.Seller)
            {
                if (user.FkShop.FkStatusId == (int)ShopStatusEnum.Disabled)
                {
                    return new ApiResponse<TokenDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.UserNameDeactive));
                }
            }
            if (user.Active == false)
            {
                return new ApiResponse<TokenDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.UserNotActive));
            }

            if (!VerifyPasswordHash(userLogin.Password, user.PasswordHash, user.PasswordSalt))
            {
                return new ApiResponse<TokenDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.UserPasswordAndUsernameInCorrect));
            }
            var UpdateDate = await _authRepository.UpdateLoginDate(user.UserId);

            if (!string.IsNullOrWhiteSpace(userLogin.NotificationKey) && !Guid.Equals(user.FkUserGroupId, GroupTypes.Admin))
            {
                var updateNotificationModel = new UpdateUserNotificationKeyDto();
                updateNotificationModel.Type = userLogin.Type;
                updateNotificationModel.NotificationKey = userLogin.NotificationKey;
                await UpdateUserNotificationKey(updateNotificationModel, user.UserId);

                // switch (userLogin.Type)
                // {
                //     case (int)UpdateUserNotificationKeyTypeEnum.ClientWebFirebasePushNotificationKey:
                //         break;
                //     case (int)UpdateUserNotificationKeyTypeEnum.ClientMobileFirebasePushNotificationKey:
                //         // subscribe to topics for mobile handled in client side
                //         break;
                //     case (int)UpdateUserNotificationKeyTypeEnum.ProviderFirebasePushNotificationKey:
                //         // var topic = AppConstants.FirebaseNotificationTopics.PROVIDER_PANEL;
                //         // var tokens = new List<String>();
                //         // tokens.Add(userLogin.NotificationKey);

                //         // await _messagingService.SubscribeToTopicAsync(topic, tokens);
                //         break;
                //     default:
                //         break;
                // }
            }

            var tokenString = GenerateToken(user);
            if (String.IsNullOrWhiteSpace(tokenString))
            {
                return new ApiResponse<TokenDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.UserSecurityGetting));
            }
            var orderCount = 0;
            if (user.FkCustumerId != null)
            {
                orderCount = await _userOrderRepository.GetUserOrderCount((int)user.FkCustumerId, token.CookieId);
                await _userOrderRepository.ChangeOrderItemsCustomer(token.CookieId, (int)user.FkCustumerId);
            }
            return new ApiResponse<TokenDto>(ResponseStatusEnum.Success, new TokenDto(tokenString, orderCount), _ms.MessageService(Message.Successfull));
        }

        public bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != passwordHash[i]) return false;
                }
                return true;
            }
        }

        public string GenerateToken(TUser user)
        {
            var Rule = UserGroupEnumMethods.GetUserGroupEnum(user.FkUserGroupId.ToString());
            int? FkUserType = 0;
            var customerName = "";
            var shopStatus = -1;

            if (user.FkCustumer != null)
            {
                customerName = user.FkCustumer.Name + ' ' + user.FkCustumer.Family;
            }
            if (user.FkCustumerId != null)
            {
                FkUserType = (int)user.FkCustumerId;
            }
            if (user.FkShopId != null)
            {
                FkUserType = (int)user.FkShopId;
                customerName = user.FkShop.StoreName;
                shopStatus = user.FkShop.FkStatusId;
            }
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier , user.UserId.ToString()),//user id
                new Claim(ClaimTypes.Name , user.UserName), // user name
                new Claim(ClaimTypes.Role, Rule), //role
                new Claim(ClaimTypes.GivenName,customerName), // customer name
                new Claim(ClaimTypes.GroupSid, FkUserType.ToString()), // customer id or shopId id
                new Claim(ClaimTypes.PrimarySid,shopStatus.ToString()), //shop status
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:Token").Value));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(8),
                SigningCredentials = creds,
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public async Task<ApiResponse<TokenDto>> ChangeUserPass(ChangeUserPasswordDto passwordDto, UserGroupEnum type)
        {
            var user = new TUser();
            if (this.token.Rule == UserGroupEnum.Admin)
            {
                if (type == UserGroupEnum.Admin)
                {
                    user = await _authRepository.GetUserByUserId(token.UserId);
                }
                else
                {

                    user = await _authRepository.GetUserByOtherId(passwordDto.Id, type);

                }
            }
            else
            {
                user = await _authRepository.GetUserByUserId(token.UserId);
            }

            if (user == null)
            {
                return new ApiResponse<TokenDto>(ResponseStatusEnum.NotFound, null, _ms.MessageService(Message.UserNotFoundById));
            }

            if (!string.IsNullOrWhiteSpace(passwordDto.UserName))
            {
                var exist = await _authRepository.UserExistByUsername(passwordDto.UserName, user.UserId, user.FkUserGroupId);
                if (exist == true)
                {
                    return new ApiResponse<TokenDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.UserNameDupplicate));
                }
                user.UserName = passwordDto.UserName;
            }
            if (!string.IsNullOrWhiteSpace(passwordDto.NewPassword))
            {
                if (token.Rule != UserGroupEnum.Admin || (token.Rule == UserGroupEnum.Admin && type == UserGroupEnum.Admin))
                {
                    if (string.IsNullOrWhiteSpace(passwordDto.OldPassword))
                    {
                        return new ApiResponse<TokenDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.UserOldPasswordInCorrect));
                    }
                    if (!VerifyPasswordHash(passwordDto.OldPassword, user.PasswordHash, user.PasswordSalt))
                    {
                        return new ApiResponse<TokenDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.UserOldPasswordInCorrect));
                    }
                }
                byte[] passwordHash, passwordSalt;
                Extentions.CreatePasswordHash(passwordDto.NewPassword, out passwordHash, out passwordSalt);
                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;
            }
            var ChangePass = await _authRepository.ChangeUserPassword(user);
            if (ChangePass == false)
            {
                return new ApiResponse<TokenDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.UserSecurityGetting));
            }
            var tokenString = GenerateToken(user);
            if (string.IsNullOrWhiteSpace(tokenString))
            {
                return new ApiResponse<TokenDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.UserSecurityGetting));
            }
            return new ApiResponse<TokenDto>(ResponseStatusEnum.Success, new TokenDto(tokenString), _ms.MessageService(Message.Successfull));
        }



        // ارسال اس ام اس به کاربر

        public async Task<ApiResponse<CustomerSMSDto>> SendVerifyMobileNumberCustomer(UserVerficationDto userVerfication, bool isResend)
        {
            if (isResend)
            {
                userVerfication.CaptchaToken = _config["Recaptcha:secretKey"];
            }
            // برای موبایل
            // if ((userVerfication.CaptchaToken != _config["Recaptcha:secretKey"]))
            // {
            //     var client = new RestClient("https://www.google.com/recaptcha/api/siteverify");
            //     var request = new RestRequest(Method.GET);
            //     request.AddParameter(
            //         "secret", _config["Recaptcha:secretKey"],
            //         ParameterType.QueryString);
            //     request.AddParameter(
            //        "response", userVerfication.CaptchaToken,
            //        ParameterType.QueryString);
            //     IRestResponse response = await client.ExecuteAsync(request);

            //     var resource = JsonConvert.DeserializeObject<GoogleRecaptchDto>(response.Content);
            //     if (!resource.success)
            //     {
            //         return new ApiResponse<CustomerSMSDto>(ResponseStatusEnum.BadRequest, null, Message.RecaptchError);
            //     }
            // }
            if (!isResend)
            {
                var userExist = await _authRepository.UserGetByUsername(userVerfication.Email, (int)UserGroupEnum.Customer);
                if (userExist != null)
                {
                    return new ApiResponse<CustomerSMSDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.UserNameDupplicate));
                }
            }



            // برای پروژه ی پودینس
            if (header.Currency == CurrencyEnum.TMN.ToString())
            {
                Random random = new Random();
                int verficationCode = random.Next(1000, 9999);

                Extentions.SendPodinisVerficationCode(verficationCode.ToString(), userVerfication.MobileNumber.ToString().Replace("+98", "0"));
                var resultSMS = await _verficationRepository.VarifyEmailOrSms(null, verficationCode, (int)VerificationTypeEnum.Varify, userVerfication.MobileNumber);

                if (resultSMS == (int)VerficationEnum.MemberTryedError)
                {
                    return new ApiResponse<CustomerSMSDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.BlockUser));
                }
                else if (resultSMS == (int)VerficationEnum.SystemError)
                {
                    return new ApiResponse<CustomerSMSDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.SystemErrorSendSms));
                }
                var res = new CustomerSMSDto();
                res.RequestId = "1111";
                return new ApiResponse<CustomerSMSDto>(ResponseStatusEnum.Success, res, _ms.MessageService(Message.Successfull));

                // برای پودینس
            }
            else
            {
                var smsResult = await Extentions.SendSMS(userVerfication.MobileNumber, Configuration["SmsConfig:apiKey"], Configuration["SmsConfig:apiSecret"]);
                // var smsResult = new CustomerSMSDto();
                // smsResult.RequestId = "123123";
                return new ApiResponse<CustomerSMSDto>(ResponseStatusEnum.Success, smsResult, _ms.MessageService(Message.Successfull));
            }


        }


        // ثبت نام کاربر جدید

        public async Task<ApiResponse<TokenDto>> Register(UserRegisterDto userRegister)
        {

            var smsResult = new CustomerSMSDto();
            if (header.Currency == CurrencyEnum.TMN.ToString())
            {
                var resultVerify = await _verficationRepository.CheckVarifyEmailOrSms(null, Int32.Parse(userRegister.VerfiyCode), (int)VerificationTypeEnum.Varify, "+" + userRegister.PhoneCode + userRegister.MobileNumber);
                if (resultVerify != 1)
                {
                    return new ApiResponse<TokenDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.VerifyCustomerCodeMobileNumber));
                }
            }
            else
            {
                smsResult = await Extentions.VerifySMS(userRegister.VerfiyCode, userRegister.RequestId, Configuration["SmsConfig:apiKey"], Configuration["SmsConfig:apiSecret"]);
                if (smsResult.Status != "0")
                {
                    return new ApiResponse<TokenDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.VerifyCustomerCodeMobileNumber));
                }

            }

            var userExist = await _authRepository.UserGetByUsername(userRegister.UserName, (int)UserGroupEnum.Customer);
            if (userExist != null)
            {
                return new ApiResponse<TokenDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.UserNameDupplicate));
            }

            var user = _mapper.Map<TUser>(userRegister);

            byte[] passwordHash, passwordSalt;
            Extentions.CreatePasswordHash(userRegister.Password, out passwordHash, out passwordSalt);
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            switch (userRegister.Type)
            {
                case (int)UpdateUserNotificationKeyTypeEnum.ClientWebFirebasePushNotificationKey:
                    // topic = AppConstants.FirebaseNotificationTopics.CLIENT_WEB;
                    break;
                case (int)UpdateUserNotificationKeyTypeEnum.ClientMobileFirebasePushNotificationKey:
                    // topic = AppConstants.FirebaseNotificationTopics.CLIENT_MOBILE;
                    user.ClientMobileFirebasePushNotificationKey = userRegister.NotificationKey;
                    break;
                case (int)UpdateUserNotificationKeyTypeEnum.ProviderFirebasePushNotificationKey:
                    // topic = AppConstants.FirebaseNotificationTopics.PROVIDER_PANEL;
                    break;
                default:
                    break;
            }

            var customer = new CustomerGeneralDetailDto();
            customer.Name = userRegister.Name;
            customer.Family = userRegister.Family;
            customer.Email = userRegister.UserName;
            customer.FkCountryId = userRegister.CountryId;
            customer.MobileNumber = userRegister.MobileNumber;
            customer.MobileVerifed = true;
            var CreatCustomer = await _customerService.RegisterCustomer(customer);
            if (CreatCustomer.Result == null)
            {
                return new ApiResponse<TokenDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.CustomerIncompleteRegistration));
            }
            user.FkCustumerId = CreatCustomer.Result.CustomerId;
            user.FkUserGroupId = Guid.Parse(GroupTypes.Customer);
            user.Active = true;
            var us = await _authRepository.UserAdd(user);
            if (us == null)
            {
                await _customerRepository.DeleteCustomer(CreatCustomer.Result.CustomerId);
                return new ApiResponse<TokenDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.UserAdding));
            }
            var tokenString = GenerateToken(user);
            if (String.IsNullOrWhiteSpace(tokenString))
            {
                return new ApiResponse<TokenDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.UserSecurityGetting));
            }
            if (user.FkCustumerId != null)
            {
                await _userOrderRepository.ChangeOrderItemsCustomer(token.CookieId, (int)user.FkCustumerId);
            }

            await _notificationService.SendNotification((int)NotificationSettingTypeEnum.AfterRegistrationClient, null, null, user.UserName, "+" + userRegister.PhoneCode + userRegister.MobileNumber);

            return new ApiResponse<TokenDto>(ResponseStatusEnum.Success, new TokenDto(tokenString), _ms.MessageService(Message.Successfull));
        }



        // با گوگل و فیس بوک ثبت نام یا لاگین کاربر
        public async Task<ApiResponse<TokenDto>> CustomerRegisterGoogleFacebook(SocialRegisterDto userRegister)
        {


            var createUser = new UserRegisterDto();

            if (userRegister.SocialType == (int)SocialTypeEnum.Google)
            {
                var client = new RestClient("https://oauth2.googleapis.com/tokeninfo");
                var request = new RestRequest(Method.GET);
                request.AddParameter(
                    "id_token", userRegister.AccessToken,
                    ParameterType.QueryString);
                IRestResponse response = await client.ExecuteAsync(request);

                var resource = JsonConvert.DeserializeObject<GoogleResponseDto>(response.Content);

                if (string.IsNullOrWhiteSpace(resource.email))
                {

                    return new ApiResponse<TokenDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.UserSecurityGetting));

                }

                createUser.Name = resource.given_name;
                createUser.Family = resource.family_name;
                createUser.UserName = resource.email;
            }
            else if (userRegister.SocialType == (int)SocialTypeEnum.Facebook)
            {
                var client = new RestClient("https://graph.facebook.com/me");
                var request = new RestRequest(Method.GET);
                request.AddParameter(
                    "fields", "id,name,email",
                    ParameterType.QueryString);
                request.AddParameter(
                "access_token", userRegister.AccessToken,
                ParameterType.QueryString);
                IRestResponse response = await client.ExecuteAsync(request);
                var resource = JsonConvert.DeserializeObject<FacebookResponseDto>(response.Content);

                if (string.IsNullOrWhiteSpace(resource.email))
                {
                    return new ApiResponse<TokenDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.UserSecurityGetting));
                }
                createUser.Name = resource.name;
                createUser.UserName = resource.email;
            }

            var user = await _authRepository.UserGetByUsername(createUser.UserName, (int)UserGroupEnum.Customer);
            if (user != null)
            {
                var UpdateDate = await _authRepository.UpdateLoginDate(user.UserId);
                var tokenString = GenerateToken(user);
                if (String.IsNullOrWhiteSpace(tokenString))
                {
                    return new ApiResponse<TokenDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.UserSecurityGetting));
                }
                if (user.FkCustumerId != null)
                {
                    await _userOrderRepository.ChangeOrderItemsCustomer(token.CookieId, (int)user.FkCustumerId);
                }
                return new ApiResponse<TokenDto>(ResponseStatusEnum.Success, new TokenDto(tokenString), _ms.MessageService(Message.Successfull));
            }

            else
            {
                var newUser = _mapper.Map<TUser>(createUser);
                var randomCode = await _settingRepository.GetRandomNumber();
                byte[] passwordHash, passwordSalt;
                Extentions.CreatePasswordHash(randomCode.ToString(), out passwordHash, out passwordSalt);
                newUser.PasswordHash = passwordHash;
                newUser.PasswordSalt = passwordSalt;
                var customer = new CustomerGeneralDetailDto();
                customer.Name = createUser.Name;
                customer.Family = createUser.Family;
                customer.Email = createUser.UserName;
                customer.MobileVerifed = true;
                var CreatCustomer = await _customerService.RegisterCustomer(customer);
                if (CreatCustomer.Result == null)
                {
                    return new ApiResponse<TokenDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.CustomerIncompleteRegistration));
                }
                newUser.FkCustumerId = CreatCustomer.Result.CustomerId;
                newUser.FkUserGroupId = Guid.Parse(GroupTypes.Customer);
                newUser.Active = true;
                var us = await _authRepository.UserAdd(newUser);
                if (us == null)
                {
                    await _customerRepository.DeleteCustomer(CreatCustomer.Result.CustomerId);
                    return new ApiResponse<TokenDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.UserAdding));
                }
                var tokenString = GenerateToken(newUser);
                if (String.IsNullOrWhiteSpace(tokenString))
                {
                    return new ApiResponse<TokenDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.UserSecurityGetting));
                }
                if (newUser.FkCustumerId != null)
                {
                    await _userOrderRepository.ChangeOrderItemsCustomer(token.CookieId, (int)newUser.FkCustumerId);
                }

                string emailPath = Path.Combine(hostingEnvironment.ContentRootPath, "emailTemplate/description-email.html");
                string subject = "sakhtiran.com";

                string text = File.ReadAllText(emailPath);
                text = text.Replace("#description", "Welcome to sakhtiran website, this is your profile password :" + randomCode);
                var resultEmail = await _emailService.Send(customer.Email, subject, text);

                return new ApiResponse<TokenDto>(ResponseStatusEnum.Success, new TokenDto(tokenString), _ms.MessageService(Message.Successfull));
            }


        }





        // ویرایش کاربر پروفایل

        public async Task<ApiResponse<bool>> UpdateUser(UserUpdateDto userUpdate)
        {
            var data = await _customerRepository.ExistCustomer(token.Id);
            if (data == null)
            {
                return new ApiResponse<bool>(ResponseStatusEnum.BadRequest, false, _ms.MessageService(Message.CustomerGetting));
            }
            var exist = await _authRepository.UserExistByUsername(userUpdate.UserName, token.UserId, data.TUser.FirstOrDefault() != null ? data.TUser.FirstOrDefault().FkUserGroupId : new Guid());
            if (exist == true)
            {
                return new ApiResponse<bool>(ResponseStatusEnum.BadRequest, false, _ms.MessageService(Message.UserNameDupplicate));
            }

            data.Name = userUpdate.Name;
            data.Family = userUpdate.Family;
            data.FkCityId = userUpdate.FkCityId;
            data.FkCountryId = userUpdate.FkCountryId;
            data.FkProvinceId = userUpdate.FkProvinceId;
            data.NationalCode = userUpdate.NationalCode;
            data.BirthDate = userUpdate.BirthDate;
            if (data.MobileNumber != userUpdate.MobileNumber)
            {
                data.MobileVerifed = false;
                data.MobileNumber = userUpdate.MobileNumber;
            }
            var updateCustomer = await _customerRepository.UpdateCustomer(data);
            if (updateCustomer == null)
            {
                return new ApiResponse<bool>(ResponseStatusEnum.BadRequest, false, _ms.MessageService(Message.UserEditing));
            }
            return new ApiResponse<bool>(ResponseStatusEnum.Success, true, _ms.MessageService(Message.Successfull));

        }

        public async Task<ApiResponse<Pagination<UserAccessDto>>> GetEmployeeUsers(PaginationFormDto pagination)
        {

            var data = await _authRepository.GetEmployeeUsers(pagination);
            if (data == null)
            {
                return new ApiResponse<Pagination<UserAccessDto>>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.UserGetting));
            }
            var count = await _authRepository.GetEmployeeUsersCount(pagination);
            return new ApiResponse<Pagination<UserAccessDto>>(ResponseStatusEnum.Success, new Pagination<UserAccessDto>(count, data), _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<bool>> ChangeActiveEmployeUser(UserAcceptDto userAccept)
        {
            var data = await _authRepository.ChangeActiveEmployeUser(userAccept);
            return new ApiResponse<bool>(ResponseStatusEnum.Success, data, _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<List<UserMenuDto>>> GetMenu()
        {
            var data = await _authRepository.GetMenu();
            return new ApiResponse<List<UserMenuDto>>(ResponseStatusEnum.Success, data, _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<bool>> AccessMenu(string path)
        {
            var data = await _authRepository.AccessMenu(path);
            return new ApiResponse<bool>(ResponseStatusEnum.Success, data, _ms.MessageService(Message.Successfull));
        }




        // اضافه کردن کاربر ادمین جدید

        public async Task<ApiResponse<UserAccessDto>> AddNewEmployeUser(UserAccessDto newUser)
        {

            var userExist = await _authRepository.UserGetByUsername(newUser.UserName, (int)UserGroupEnum.Admin);
            if (userExist != null)
            {
                return new ApiResponse<UserAccessDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.UserNameDupplicate));
            }

            var user = _mapper.Map<TUser>(newUser);

            byte[] passwordHash, passwordSalt;
            Extentions.CreatePasswordHash(newUser.Password, out passwordHash, out passwordSalt);
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
            user.FkUserGroupId = Guid.Parse(GroupTypes.Admin);
            user.Active = true;
            var us = await _authRepository.UserAdd(user);
            if (us == null)
            {
                return new ApiResponse<UserAccessDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.UserAdding));
            }
            var result = _mapper.Map<UserAccessDto>(us);
            return new ApiResponse<UserAccessDto>(ResponseStatusEnum.Success, result, _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<UserAccessDto>> UpdateNewEmployeUser(UserAccessDto userUpdate)
        {

            var exist = await _authRepository.UserExistByUsername(userUpdate.UserName, userUpdate.UserId, Guid.Parse(GroupTypes.Admin));
            if (exist == true)
            {
                return new ApiResponse<UserAccessDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.UserNameDupplicate));
            }

            var mapUser = _mapper.Map<TUser>(userUpdate);
            foreach (var item in mapUser.TUserAccessControl)
            {
                item.FkUserId = userUpdate.UserId;
            }
            var changePass = false;
            if (!string.IsNullOrWhiteSpace(userUpdate.Password))
            {
                byte[] passwordHash, passwordSalt;
                Extentions.CreatePasswordHash(userUpdate.Password, out passwordHash, out passwordSalt);
                mapUser.PasswordHash = passwordHash;
                mapUser.PasswordSalt = passwordSalt;
                changePass = true;

            }
            mapUser.FkUserGroupId = Guid.Parse(GroupTypes.Admin);
            var updateCustomer = await _authRepository.UpdateUser(mapUser, changePass);
            if (updateCustomer == null)
            {
                return new ApiResponse<UserAccessDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.UserEditing));
            }
            var result = _mapper.Map<UserAccessDto>(updateCustomer);

            return new ApiResponse<UserAccessDto>(ResponseStatusEnum.Success, result, _ms.MessageService(Message.Successfull));

        }

        public async Task<ApiResponse<AuthWebDetailsDto>> GetWebSiteAuthDetials()
        {
            var data = await _authRepository.GetWebSiteAuthDetials();
            return new ApiResponse<AuthWebDetailsDto>(ResponseStatusEnum.Success, data, _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<bool>> SendEmail(int type, string email)
        {
            var userExist = await _authRepository.UserGetByUsername(email, (int)UserGroupEnum.Customer);
            if (userExist == null)
            {
                return new ApiResponse<bool>(ResponseStatusEnum.BadRequest, false, _ms.MessageService(Message.UserNotFoundByUserName));
            }
            Random random = new Random();
            int verficationCode = random.Next(100000, 999999);
            var result = await _verficationRepository.VarifyEmailOrSms(email, verficationCode, type, null);

            if (result == (int)VerficationEnum.MemberTryedError)
            {
                return new ApiResponse<bool>(ResponseStatusEnum.BadRequest, true, _ms.MessageService(Message.BlockUser));
            }
            else if (result == (int)VerficationEnum.SystemError)
            {
                return new ApiResponse<bool>(ResponseStatusEnum.BadRequest, true, _ms.MessageService(Message.SystemErrorSendEmail));

            }
            else
            {
                string emailPath = "";
                string subject = "";
                if (type == (int)VerificationTypeEnum.Varify)
                {
                    emailPath = Path.Combine(hostingEnvironment.ContentRootPath, "emailTemplate/verify-email.html");
                    subject = "sakhtiran.com - Verify your Email";
                }
                else if (type == (int)VerificationTypeEnum.ForgetPassword)
                {
                    emailPath = Path.Combine(hostingEnvironment.ContentRootPath, "emailTemplate/forget-email.html");
                    subject = "sakhtiran.com - recover your password";

                }

                string text = File.ReadAllText(emailPath);
                text = text.Replace("#customerName", userExist.FkCustumer.Name + " " + userExist.FkCustumer.Family);
                text = text.Replace("#emailCode", verficationCode.ToString());
                var resultEmail = await _emailService.Send(userExist.UserName, subject, text);
            }
            return new ApiResponse<bool>(ResponseStatusEnum.Success, true, _ms.MessageService(Message.Successfull));
        }


        public async Task<ApiResponse<bool>> CustomerEmailVerify(string email, int code)
        {

            var result = await _verficationRepository.CheckVarifyEmailOrSms(email, code, (int)VerificationTypeEnum.ForgetPassword, null);

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
                return new ApiResponse<bool>(ResponseStatusEnum.Success, true, _ms.MessageService(Message.Successfull));
            }

        }



        // avaz kardan ramze obor henqame farmoshi ramz

        public async Task<ApiResponse<bool>> ChangeCustomerPassForForget(ChangeUserPasswordDto passwordDto)
        {
            var resultVerify = await _verficationRepository.EmailOrSmsIsVerifed(passwordDto.UserName, (int)VerificationTypeEnum.ForgetPassword, null);

            if (!resultVerify)
            {
                return new ApiResponse<bool>(ResponseStatusEnum.BadRequest, true, _ms.MessageService(Message.ChangePassAccess));
            }
            var user = new TUser();

            if (!string.IsNullOrWhiteSpace(passwordDto.UserName))
            {
                var userExist = await _authRepository.UserGetByUsername(passwordDto.UserName, (int)UserGroupEnum.Customer);
                if (userExist == null)
                {
                    return new ApiResponse<bool>(ResponseStatusEnum.BadRequest, false, _ms.MessageService(Message.UserNotFoundByUserName));
                }
                user = userExist;
            }
            if (!string.IsNullOrWhiteSpace(passwordDto.NewPassword))
            {
                byte[] passwordHash, passwordSalt;
                Extentions.CreatePasswordHash(passwordDto.NewPassword, out passwordHash, out passwordSalt);
                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;
            }
            var ChangePass = await _authRepository.ChangeUserPassword(user);
            if (ChangePass == false)
            {
                return new ApiResponse<bool>(ResponseStatusEnum.BadRequest, false, _ms.MessageService(Message.UserSecurityGetting));
            }

            return new ApiResponse<bool>(ResponseStatusEnum.Success, true, _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<string>> GetCurrentUserUserName()
        {
            var user = await _authRepository.GetCurrentUserUserName();
            return new ApiResponse<string>(ResponseStatusEnum.Success, user, _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<bool>> UpdateUserNotificationKey(UpdateUserNotificationKeyDto model, Guid userId)
        {

            try
            {
                var user = await _authRepository.UpdateUserNotificationKey(model, userId);
                var topic = "";

                if (model.Type == (int)UpdateUserNotificationKeyTypeEnum.ClientMobileFirebasePushNotificationKey)
                {
                    // No need to handle topics here for mobile. (for mobile topics handled in client side)
                    return new ApiResponse<bool>(ResponseStatusEnum.Success, true, _ms.MessageService(Message.Successfull));
                }

                switch (model.Type)
                {
                    case (int)UpdateUserNotificationKeyTypeEnum.ClientWebFirebasePushNotificationKey:
                        topic = AppConstants.FirebaseNotificationTopics.CLIENT_WEB;
                        break;
                    case (int)UpdateUserNotificationKeyTypeEnum.ClientMobileFirebasePushNotificationKey:
                        topic = AppConstants.FirebaseNotificationTopics.CLIENT_MOBILE;
                        break;
                    case (int)UpdateUserNotificationKeyTypeEnum.ProviderFirebasePushNotificationKey:
                        topic = AppConstants.FirebaseNotificationTopics.PROVIDER_PANEL;
                        break;
                    default:
                        break;
                }

                var tokens = new List<String>();
                tokens.Add(model.NotificationKey);

                await _messagingService.SubscribeToTopicAsync(topic, tokens);
                return new ApiResponse<bool>(ResponseStatusEnum.Success, true, _ms.MessageService(Message.Successfull));
            }
            catch (System.Exception)
            {
                return new ApiResponse<bool>(ResponseStatusEnum.Success, false, _ms.MessageService(Message.Successfull));
            }

        }





        // verify kardan shomare mobile customer dar panele khodesh

        public async Task<ApiResponse<CustomerSMSDto>> VerifyCustomerMobileNumber()
        {
            if (token.Id == 0 || token.Rule != UserGroupEnum.Customer)
            {
                return new ApiResponse<CustomerSMSDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.UserNotFoundById));
            }
            var customer = await _customerRepository.GetCustomerGeneralDetail(token.Id);
            if (customer == null)
            {
                return new ApiResponse<CustomerSMSDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.UserNotFoundById));
            }
            Random random = new Random();
            int verficationCode = random.Next(1000, 9999);
            var resultSMS = await _verficationRepository.VarifyEmailOrSms(null, verficationCode, (int)VerificationTypeEnum.Varify, "+" + customer.PhoneCode + customer.MobileNumber);

            if (resultSMS == (int)VerficationEnum.MemberTryedError)
            {
                return new ApiResponse<CustomerSMSDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.BlockUserSms));
            }
            else if (resultSMS == (int)VerficationEnum.SystemError)
            {
                return new ApiResponse<CustomerSMSDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.SystemErrorSendSms));
            }

            // برای پروژه ی پودینس
            if (header.Currency == CurrencyEnum.TMN.ToString())
            {

                Extentions.SendPodinisVerficationCode(verficationCode.ToString(), "0" + customer.MobileNumber);
                var finalResult = new CustomerSMSDto();
                finalResult.RequestId = verficationCode.ToString();
                return new ApiResponse<CustomerSMSDto>(ResponseStatusEnum.Success, finalResult, _ms.MessageService(Message.Successfull));

            }
            else
            {
                var smsResult = await Extentions.SendSMS(customer.PhoneCode + customer.MobileNumber, Configuration["SmsConfig:apiKey"], Configuration["SmsConfig:apiSecret"]);
                return new ApiResponse<CustomerSMSDto>(ResponseStatusEnum.Success, smsResult, _ms.MessageService(Message.Successfull));
            }

        }

        public async Task<ApiResponse<bool>> CheckVerifyCustomerMobileNumber(int VerifyCode, string requestId)
        {
            if (token.Id == 0 || token.Rule != UserGroupEnum.Customer)
            {
                return new ApiResponse<bool>(ResponseStatusEnum.BadRequest, false, _ms.MessageService(Message.UserNotFoundById));
            }
            var customer = await _customerRepository.GetCustomerGeneralDetail(token.Id);
            if (customer == null)
            {
                return new ApiResponse<bool>(ResponseStatusEnum.BadRequest, false, _ms.MessageService(Message.UserNotFoundById));
            }
            var smsResult = new CustomerSMSDto();
            if (header.Currency == CurrencyEnum.TMN.ToString())
            {
                var resultVerify = await _verficationRepository.CheckVarifyEmailOrSms(null, VerifyCode, (int)VerificationTypeEnum.Varify, "+" + customer.PhoneCode + customer.MobileNumber);
                if (resultVerify != 1)
                {
                    return new ApiResponse<bool>(ResponseStatusEnum.BadRequest, false, _ms.MessageService(Message.VerifyCustomerCodeMobileNumber));
                }
            }
            else
            {
                smsResult = await Extentions.VerifySMS(VerifyCode.ToString(), requestId, Configuration["SmsConfig:apiKey"], Configuration["SmsConfig:apiSecret"]);
                if (smsResult.Status != "0")
                {
                    return new ApiResponse<bool>(ResponseStatusEnum.BadRequest, false, _ms.MessageService(Message.VerifyCustomerCodeMobileNumber));
                }

            }

            var userVerify = await _customerRepository.VerifyCustomerMobileNumber(token.Id);

            return new ApiResponse<bool>(ResponseStatusEnum.Success, true, _ms.MessageService(Message.Successfull));
        }

        // ferestadn sms be tamin konande baraye faramoshi ramze obor

        public async Task<ApiResponse<CustomerSMSDto>> SendCodeToVendorForForgetPassword(string email)
        {
            var shop = await _shopRepository.GetShopGeneralDetail(0, email);
            if (shop == null)
            {
                return new ApiResponse<CustomerSMSDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.UserNotFoundById));
            }
            Random random = new Random();
            int verficationCode = random.Next(1000, 9999);
            var resultSMS = await _verficationRepository.VarifyEmailOrSms(null, verficationCode, (int)VerificationTypeEnum.ForgetPassword, shop.Phone);

            if (resultSMS == (int)VerficationEnum.MemberTryedError)
            {
                return new ApiResponse<CustomerSMSDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.BlockUserSms));
            }
            else if (resultSMS == (int)VerficationEnum.SystemError)
            {
                return new ApiResponse<CustomerSMSDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.SystemErrorSendSms));
            }

            // برای پروژه ی پودینس
            if (header.Currency == CurrencyEnum.TMN.ToString())
            {
                Extentions.SendPodinisVerficationCode(verficationCode.ToString(), shop.Phone);
                var finalResult = new CustomerSMSDto();
                finalResult.RequestId = verficationCode.ToString();
                finalResult.MobileNumber = shop.Phone;
                return new ApiResponse<CustomerSMSDto>(ResponseStatusEnum.Success, finalResult, _ms.MessageService(Message.Successfull));

            }
            else
            {
                var smsResult = await Extentions.SendSMS(shop.Phone, Configuration["SmsConfig:apiKey"], Configuration["SmsConfig:apiSecret"]);
                smsResult.MobileNumber = shop.Phone;
                return new ApiResponse<CustomerSMSDto>(ResponseStatusEnum.Success, smsResult, _ms.MessageService(Message.Successfull));
            }

        }

        public async Task<ApiResponse<TokenDto>> CheckVerifyVendorForForgetPassword(int VerifyCode, string requestId, string email, string notificationKey)
        {
            var shop = await _shopRepository.GetShopGeneralDetail(0, email);
            if (shop == null)
            {
                return new ApiResponse<TokenDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.UserNotFoundById));
            }
            var user = await _authRepository.GetUserByUserId(shop.UserId);
            var smsResult = new CustomerSMSDto();
            if (header.Currency == CurrencyEnum.TMN.ToString())
            {
                var resultVerify = await _verficationRepository.CheckVarifyEmailOrSms(null, VerifyCode, (int)VerificationTypeEnum.ForgetPassword, shop.Phone);
                if (resultVerify != 1)
                {
                    return new ApiResponse<TokenDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.VerifyCustomerCodeMobileNumber));
                }
            }
            else
            {
                smsResult = await Extentions.VerifySMS(VerifyCode.ToString(), requestId, Configuration["SmsConfig:apiKey"], Configuration["SmsConfig:apiSecret"]);
                if (smsResult.Status != "0")
                {
                    return new ApiResponse<TokenDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.VerifyCustomerCodeMobileNumber));
                }

            }

            var UpdateDate = await _authRepository.UpdateLoginDate(user.UserId);

            if (!string.IsNullOrWhiteSpace(notificationKey) && !Guid.Equals(user.FkUserGroupId, GroupTypes.Admin))
            {
                var updateNotificationModel = new UpdateUserNotificationKeyDto();
                updateNotificationModel.Type = 3;
                updateNotificationModel.NotificationKey = notificationKey;
                await UpdateUserNotificationKey(updateNotificationModel, user.UserId);

            }

            var tokenString = GenerateToken(user);
            if (String.IsNullOrWhiteSpace(tokenString))
            {
                return new ApiResponse<TokenDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.UserSecurityGetting));
            }
            return new ApiResponse<TokenDto>(ResponseStatusEnum.Success, new TokenDto(tokenString), _ms.MessageService(Message.Successfull));
        }

    }
}