using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using MarketPlace.API.Data.Dtos.Setting;
using MarketPlace.API.Data.Models;
using MarketPlace.API.Data.Repositories.IRepository;
using MarketPlace.API.Services.IService;
using MarketPlace.API.Helper;
using MarketPlace.API.Data.Enums;
using MarketPlace.API.Data.Dtos.ShopPlan;
using MarketPlace.API.Data.Dtos.Accept;
using MarketPlace.API.Data.Dtos.PaymentMethod;
using MarketPlace.API.Data.Dtos.ShippingMethod;

namespace MarketPlace.API.Services.Service
{
    public class SettingService : ISettingService
    {
        public IMapper _mapper { get; }
        public ISettingRepository _settingRepository { get; }
        public ICategoryRepository _categoryRepository { get; }
        public IFileUploadService _fileUploadService { get; }
        public IMessageLanguageService _ms { get; }
        public SettingService(IMapper mapper, ISettingRepository settingRepository,
        IMessageLanguageService ms,
        ICategoryRepository categoryRepository, IFileUploadService fileUploadService)
        {
            this._fileUploadService = fileUploadService;
            this._categoryRepository = categoryRepository;
            this._settingRepository = settingRepository;
            this._mapper = mapper;
            _ms = ms;
        }

        public async Task<ApiResponse<SettingLogoDto>> EditSettingLogo(SettingSerializeDto settingDto)
        {
            var settingObj = new SettingLogoDto();
            var HeaderLogoImageName = "";
            if (settingDto.ShopHeaderLogoUrl != null)
            {
                HeaderLogoImageName = _fileUploadService.UploadImageWithName(settingDto.ShopHeaderLogoUrl, Pathes.LogoTemp, "LogoHeader");
                if (HeaderLogoImageName == null)
                {
                    return new ApiResponse<SettingLogoDto>(ResponseStatusEnum.BadRequest, null,_ms.MessageService(Message.UploadFile));
                }
                settingObj.LogoUrlShopHeader = HeaderLogoImageName;
            }
            var LogoFotterImageName = "";
            if (settingDto.ShopFooterLogoUrl != null)
            {
                LogoFotterImageName = _fileUploadService.UploadImageWithName(settingDto.ShopFooterLogoUrl, Pathes.LogoTemp, "LogoFooter");
                if (LogoFotterImageName == null)
                {
                    return new ApiResponse<SettingLogoDto>(ResponseStatusEnum.BadRequest, null,_ms.MessageService(Message.UploadFile));
                }
                settingObj.LogoUrlShopFooter = LogoFotterImageName;
            }
            var LoginPageLogoUrl = "";
            if (settingDto.LoginPageLogoUrl != null)
            {
                LoginPageLogoUrl = _fileUploadService.UploadImageWithName(settingDto.LoginPageLogoUrl, Pathes.LogoTemp, "LogoLogin");
                if (LoginPageLogoUrl == null)
                {
                    return new ApiResponse<SettingLogoDto>(ResponseStatusEnum.BadRequest, null,_ms.MessageService(Message.UploadFile));
                }
                settingObj.LogoUrlLoginPage = LoginPageLogoUrl;
            }
            var LoginBkUrl = "";
            if (settingDto.WebLoginBkUrl != null)
            {
                LoginBkUrl = _fileUploadService.UploadImageWithName(settingDto.WebLoginBkUrl, Pathes.LogoTemp, "BkLogin");
                if (LoginBkUrl == null)
                {
                    return new ApiResponse<SettingLogoDto>(ResponseStatusEnum.BadRequest, null,_ms.MessageService(Message.UploadFile));
                }
                settingObj.CustomerLoginPageBackgroundImage = LoginBkUrl;
            }
            var HelpBkUrl = "";
            if (settingDto.WebHelpBkUrl != null)
            {
                HelpBkUrl = _fileUploadService.UploadImageWithName(settingDto.WebHelpBkUrl, Pathes.LogoTemp, "BkHelp");
                if (HelpBkUrl == null)
                {
                    return new ApiResponse<SettingLogoDto>(ResponseStatusEnum.BadRequest, null,_ms.MessageService(Message.UploadFile));
                }
                settingObj.HelpPageBackgroundImage = HelpBkUrl;
            }
            var shopDefaultBannerImageUrl = "";
            if (settingDto.ShopDefaultBanner != null)
            {
                shopDefaultBannerImageUrl = _fileUploadService.UploadImageWithName(settingDto.ShopDefaultBanner, Pathes.LogoTemp, "ShopDefaultBanner");
                if (shopDefaultBannerImageUrl == null)
                {
                    return new ApiResponse<SettingLogoDto>(ResponseStatusEnum.BadRequest, null,_ms.MessageService(Message.UploadFile));
                }
                settingObj.ShopDefaultBannerImageUrl = shopDefaultBannerImageUrl;
            }

            var data = await _settingRepository.EditSettingLogo(settingObj);
            if (data == null)
            {
                if (settingDto.ShopHeaderLogoUrl != null)
                {
                    _fileUploadService.DeleteImage(HeaderLogoImageName, Pathes.LogoTemp);
                }
                if (settingDto.ShopFooterLogoUrl != null)
                {
                    _fileUploadService.DeleteImage(LogoFotterImageName, Pathes.LogoTemp);
                }
                if (settingDto.LoginPageLogoUrl != null)
                {
                    _fileUploadService.DeleteImage(LoginPageLogoUrl, Pathes.LogoTemp);
                }                
                if (settingDto.WebLoginBkUrl != null)
                {
                    _fileUploadService.DeleteImage(LoginBkUrl, Pathes.LogoTemp);
                }                
                if (settingDto.WebHelpBkUrl != null)
                {
                    _fileUploadService.DeleteImage(HelpBkUrl, Pathes.LogoTemp);
                }                
                if (settingDto.ShopDefaultBanner != null)
                {
                    _fileUploadService.DeleteImage(shopDefaultBannerImageUrl, Pathes.LogoTemp);
                }
                return new ApiResponse<SettingLogoDto>(ResponseStatusEnum.BadRequest, null,_ms.MessageService(Message.SettingEdit));
            }
            if (settingDto.ShopHeaderLogoUrl != null)
            {
                var isMoved = _fileUploadService.ChangeDestOfFile(HeaderLogoImageName, Pathes.LogoTemp, Pathes.Logo);
                if (!isMoved)
                {
                    _fileUploadService.DeleteImage(HeaderLogoImageName, Pathes.LogoTemp);
                }
            }
            if (settingDto.ShopFooterLogoUrl != null)
            {
                var isMoved = _fileUploadService.ChangeDestOfFile(LogoFotterImageName, Pathes.LogoTemp, Pathes.Logo);
                if (!isMoved)
                {
                    _fileUploadService.DeleteImage(LogoFotterImageName, Pathes.LogoTemp);
                }
            }
            if (settingDto.LoginPageLogoUrl != null)
            {
                var isMoved = _fileUploadService.ChangeDestOfFile(LoginPageLogoUrl, Pathes.LogoTemp, Pathes.Logo);
                if (!isMoved)
                {
                    _fileUploadService.DeleteImage(LoginPageLogoUrl, Pathes.LogoTemp);
                }
            }
            if (settingDto.WebLoginBkUrl != null)
            {
                var isMoved = _fileUploadService.ChangeDestOfFile(LoginBkUrl, Pathes.LogoTemp, Pathes.Logo);
                if (!isMoved)
                {
                    _fileUploadService.DeleteImage(LoginBkUrl, Pathes.LogoTemp);
                }
            }
            if (settingDto.WebHelpBkUrl != null)
            {
                var isMoved = _fileUploadService.ChangeDestOfFile(HelpBkUrl, Pathes.LogoTemp, Pathes.Logo);
                if (!isMoved)
                {
                    _fileUploadService.DeleteImage(HelpBkUrl, Pathes.LogoTemp);
                }
            }
            if (settingDto.ShopDefaultBanner != null)
            {
                var isMoved = _fileUploadService.ChangeDestOfFile(shopDefaultBannerImageUrl, Pathes.LogoTemp, Pathes.Logo);
                if (!isMoved)
                {
                    _fileUploadService.DeleteImage(shopDefaultBannerImageUrl, Pathes.LogoTemp);
                }
            }

            return new ApiResponse<SettingLogoDto>(ResponseStatusEnum.Success, data,_ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<SettingLogoDto>> GetSettingLogo()
        {
            var data = await _settingRepository.GetSettingLogo();
            if (data == null)
            {
                return new ApiResponse<SettingLogoDto>(ResponseStatusEnum.BadRequest, null,_ms.MessageService(Message.SettingGetting));
            }
            return new ApiResponse<SettingLogoDto>(ResponseStatusEnum.Success, data,_ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<SettingCompanyDto>> EditSettingCompany(SettingCompanyDto settingDto)
        {
            var data = await _settingRepository.EditSettingCompany(settingDto);
            if (data == null)
            {
                return new ApiResponse<SettingCompanyDto>(ResponseStatusEnum.BadRequest, null,_ms.MessageService(Message.SettingEdit));
            }
            return new ApiResponse<SettingCompanyDto>(ResponseStatusEnum.Success, data,_ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<SettingCompanyDto>> GetSettingCompany()
        {
            var data = await _settingRepository.GetSettingCompany();
            if (data == null)
            {
                return new ApiResponse<SettingCompanyDto>(ResponseStatusEnum.BadRequest, null,_ms.MessageService(Message.SettingGetting));
            }
            return new ApiResponse<SettingCompanyDto>(ResponseStatusEnum.Success, data,_ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<SettingSeoDto>> EditSettingSeo(SettingSeoDto settingDto)
        {
            var data = await _settingRepository.EditSettingSeo(settingDto);
            if (data == null)
            {
                return new ApiResponse<SettingSeoDto>(ResponseStatusEnum.BadRequest, null,_ms.MessageService(Message.SettingEdit));
            }
            return new ApiResponse<SettingSeoDto>(ResponseStatusEnum.Success, data,_ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<SettingSeoDto>> GetSettingSeo()
        {
            var data = await _settingRepository.GetSettingSeo();
            if (data == null)
            {
                return new ApiResponse<SettingSeoDto>(ResponseStatusEnum.BadRequest, null,_ms.MessageService(Message.SettingGetting));
            }
            return new ApiResponse<SettingSeoDto>(ResponseStatusEnum.Success, data,_ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<SettingSocialDto>> EditSettingSocial(SettingSocialDto settingDto)
        {
            var data = await _settingRepository.EditSettingSocial(settingDto);
            if (data == null)
            {
                return new ApiResponse<SettingSocialDto>(ResponseStatusEnum.BadRequest, null,_ms.MessageService(Message.SettingEdit));
            }
            return new ApiResponse<SettingSocialDto>(ResponseStatusEnum.Success, data,_ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<SettingSocialDto>> GetSettingSocial()
        {
            var data = await _settingRepository.GetSettingSocial();
            if (data == null)
            {
                return new ApiResponse<SettingSocialDto>(ResponseStatusEnum.BadRequest, null,_ms.MessageService(Message.SettingGetting));
            }
            return new ApiResponse<SettingSocialDto>(ResponseStatusEnum.Success, data,_ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<SettingGeneralDto>> EditSettingGeneral(SettingGeneralDto settingDto)
        {
            var data = await _settingRepository.EditSettingGeneral(settingDto);
            if (data == null)
            {
                return new ApiResponse<SettingGeneralDto>(ResponseStatusEnum.BadRequest, null,_ms.MessageService(Message.SettingEdit));
            }
            return new ApiResponse<SettingGeneralDto>(ResponseStatusEnum.Success, data,_ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<SettingGeneralDto>> GetSettingGeneral()
        {
            var data = await _settingRepository.GetSettingGeneral();
            if (data == null)
            {
                return new ApiResponse<SettingGeneralDto>(ResponseStatusEnum.BadRequest, null,_ms.MessageService(Message.SettingGetting));
            }
            return new ApiResponse<SettingGeneralDto>(ResponseStatusEnum.Success, data,_ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<SettingDescriptionDto>> EditSettingAboutUs(SettingDescriptionDto settingDto)
        {
            var data = await _settingRepository.EditSettingAboutUs(settingDto);
            if (data == null)
            {
                return new ApiResponse<SettingDescriptionDto>(ResponseStatusEnum.BadRequest, null,_ms.MessageService(Message.SettingEdit));
            }
            return new ApiResponse<SettingDescriptionDto>(ResponseStatusEnum.Success, data,_ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<SettingDescriptionDto>> GetSettingAboutUs()
        {
            var data = await _settingRepository.GetSettingAboutUs();
            if (data == null)
            {
                return new ApiResponse<SettingDescriptionDto>(ResponseStatusEnum.BadRequest, null,_ms.MessageService(Message.SettingGetting));
            }
            return new ApiResponse<SettingDescriptionDto>(ResponseStatusEnum.Success, data,_ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<SettingDescriptionDto>> EditSettingShortDescription(SettingDescriptionDto settingDto)
        {
            var data = await _settingRepository.EditSettingShortDescription(settingDto);
            if (data == null)
            {
                return new ApiResponse<SettingDescriptionDto>(ResponseStatusEnum.BadRequest, null,_ms.MessageService(Message.SettingEdit));
            }
            return new ApiResponse<SettingDescriptionDto>(ResponseStatusEnum.Success, data,_ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<SettingDescriptionDto>> GetSettingShortDescription()
        {
            var data = await _settingRepository.GetSettingShortDescription();
            if (data == null)
            {
                return new ApiResponse<SettingDescriptionDto>(ResponseStatusEnum.BadRequest, null,_ms.MessageService(Message.SettingGetting));
            }
            return new ApiResponse<SettingDescriptionDto>(ResponseStatusEnum.Success, data,_ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<SettingDescriptionDto>> EditSettingWarrantyPolicy(SettingDescriptionDto settingDto)
        {
            var data = await _settingRepository.EditSettingWarrantyPolicy(settingDto);
            if (data == null)
            {
                return new ApiResponse<SettingDescriptionDto>(ResponseStatusEnum.BadRequest, null,_ms.MessageService(Message.SettingEdit));
            }
            return new ApiResponse<SettingDescriptionDto>(ResponseStatusEnum.Success, data,_ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<SettingDescriptionDto>> GetSettingWarrantyPolicy()
        {
            var data = await _settingRepository.GetSettingWarrantyPolicy();
            if (data == null)
            {
                return new ApiResponse<SettingDescriptionDto>(ResponseStatusEnum.BadRequest, null,_ms.MessageService(Message.SettingGetting));
            }
            return new ApiResponse<SettingDescriptionDto>(ResponseStatusEnum.Success, data,_ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<SettingDescriptionDto>> EditSettingTermOfUser(SettingDescriptionDto settingDto)
        {
            var data = await _settingRepository.EditSettingTermOfUser(settingDto);
            if (data == null)
            {
                return new ApiResponse<SettingDescriptionDto>(ResponseStatusEnum.BadRequest, null,_ms.MessageService(Message.SettingEdit));
            }
            return new ApiResponse<SettingDescriptionDto>(ResponseStatusEnum.Success, data,_ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<SettingDescriptionDto>> GetSettingTermOfUser()
        {
            var data = await _settingRepository.GetSettingTermOfUser();
            if (data == null)
            {
                return new ApiResponse<SettingDescriptionDto>(ResponseStatusEnum.BadRequest, null,_ms.MessageService(Message.SettingGetting));
            }
            return new ApiResponse<SettingDescriptionDto>(ResponseStatusEnum.Success, data,_ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<SettingDescriptionDto>> EditSettingTermOfSale(SettingDescriptionDto settingDto)
        {
            var data = await _settingRepository.EditSettingTermOfSale(settingDto);
            if (data == null)
            {
                return new ApiResponse<SettingDescriptionDto>(ResponseStatusEnum.BadRequest, null,_ms.MessageService(Message.SettingEdit));
            }
            return new ApiResponse<SettingDescriptionDto>(ResponseStatusEnum.Success, data,_ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<SettingDescriptionDto>> GetSettingTermOfSale()
        {
            var data = await _settingRepository.GetSettingTermOfSale();
            if (data == null)
            {
                return new ApiResponse<SettingDescriptionDto>(ResponseStatusEnum.BadRequest, null,_ms.MessageService(Message.SettingGetting));
            }
            return new ApiResponse<SettingDescriptionDto>(ResponseStatusEnum.Success, data,_ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<SettingDescriptionDto>> EditSettingPrivacyPolicy(SettingDescriptionDto settingDto)
        {
            var data = await _settingRepository.EditSettingPrivacyPolicy(settingDto);
            if (data == null)
            {
                return new ApiResponse<SettingDescriptionDto>(ResponseStatusEnum.BadRequest, null,_ms.MessageService(Message.SettingEdit));
            }
            return new ApiResponse<SettingDescriptionDto>(ResponseStatusEnum.Success, data,_ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<SettingDescriptionDto>> GetSettingPrivacyPolicy()
        {
            var data = await _settingRepository.GetSettingPrivacyPolicy();
            if (data == null)
            {
                return new ApiResponse<SettingDescriptionDto>(ResponseStatusEnum.BadRequest, null,_ms.MessageService(Message.SettingGetting));
            }
            return new ApiResponse<SettingDescriptionDto>(ResponseStatusEnum.Success, data,_ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<SettingDescriptionDto>> EditSettingCustomerRights(SettingDescriptionDto settingDto)
        {
            var data = await _settingRepository.EditSettingCustomerRights(settingDto);
            if (data == null)
            {
                return new ApiResponse<SettingDescriptionDto>(ResponseStatusEnum.BadRequest, null,_ms.MessageService(Message.SettingEdit));
            }
            return new ApiResponse<SettingDescriptionDto>(ResponseStatusEnum.Success, data,_ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<SettingDescriptionDto>> GetSettingCustomerRights()
        {
            var data = await _settingRepository.GetSettingCustomerRights();
            if (data == null)
            {
                return new ApiResponse<SettingDescriptionDto>(ResponseStatusEnum.BadRequest, null,_ms.MessageService(Message.SettingGetting));
            }
            return new ApiResponse<SettingDescriptionDto>(ResponseStatusEnum.Success, data,_ms.MessageService(Message.Successfull));
        }


        
        public async Task<ApiResponse<SettingDescriptionDto>> EditSettingInactiveShopMessage(SettingDescriptionDto settingDto)
        {
            var data = await _settingRepository.EditInactiveShopMessage(settingDto);
            if (data == null)
            {
                return new ApiResponse<SettingDescriptionDto>(ResponseStatusEnum.BadRequest, null,_ms.MessageService(Message.SettingEdit));
            }
            return new ApiResponse<SettingDescriptionDto>(ResponseStatusEnum.Success, data,_ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<SettingDescriptionDto>> GetSettingInactiveShopMessage()
        {
            var data = await _settingRepository.GetInactiveShopMessage();
            if (data == null)
            {
                return new ApiResponse<SettingDescriptionDto>(ResponseStatusEnum.BadRequest, null,_ms.MessageService(Message.SettingGetting));
            }
            return new ApiResponse<SettingDescriptionDto>(ResponseStatusEnum.Success, data,_ms.MessageService(Message.Successfull));
        }
        
        public async Task<ApiResponse<SettingDescriptionDto>> EditSettingShopWelcomeMessage(SettingDescriptionDto settingDto)
        {
            var data = await _settingRepository.EditShopWelcomeMessage(settingDto);
            if (data == null)
            {
                return new ApiResponse<SettingDescriptionDto>(ResponseStatusEnum.BadRequest, null,_ms.MessageService(Message.SettingEdit));
            }
            return new ApiResponse<SettingDescriptionDto>(ResponseStatusEnum.Success, data,_ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<SettingDescriptionDto>> GetSettingShopWelcomeMessage()
        {
            var data = await _settingRepository.GetShopWelcomeMessage();
            if (data == null)
            {
                return new ApiResponse<SettingDescriptionDto>(ResponseStatusEnum.BadRequest, null,_ms.MessageService(Message.SettingGetting));
            }
            return new ApiResponse<SettingDescriptionDto>(ResponseStatusEnum.Success, data,_ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<SettingEmailDto>> EditSettingEmail(SettingEmailDto settingDto)
        {
            var data = await _settingRepository.EditSettingEmail(settingDto);
            if (data == null)
            {
                return new ApiResponse<SettingEmailDto>(ResponseStatusEnum.BadRequest, null,_ms.MessageService(Message.SettingEdit));
            }
            return new ApiResponse<SettingEmailDto>(ResponseStatusEnum.Success, data,_ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<SettingEmailDto>> GetSettingEmail()
        {
            var data = await _settingRepository.GetSettingEmail();
            if (data == null)
            {
                return new ApiResponse<SettingEmailDto>(ResponseStatusEnum.BadRequest, null,_ms.MessageService(Message.SettingGetting));
            }
            return new ApiResponse<SettingEmailDto>(ResponseStatusEnum.Success, data,_ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<bool>> AddShopPlan(ShopPlanDto shopPlan)
        {

            if (shopPlan.PercentOfCommission > 100 || shopPlan.PercentOfCommission < 0)
            {
                return new ApiResponse<bool>(ResponseStatusEnum.BadRequest, false, _ms.MessageService(Message.ShopPlanAdd));
            }
            var mapPlan = _mapper.Map<TShopPlan>(shopPlan);
            var data = await _settingRepository.AddShopPlan(mapPlan);
            if (data == false)
            {
                return new ApiResponse<bool>(ResponseStatusEnum.BadRequest, false,_ms.MessageService(Message.ShopPlanAdd));
            }
            return new ApiResponse<bool>(ResponseStatusEnum.Success, true,_ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<bool>> EditShopPlan(ShopPlanDto shopPlan)
        {

            if (shopPlan.PercentOfCommission > 100 || shopPlan.PercentOfCommission < 0)
            {
                return new ApiResponse<bool>(ResponseStatusEnum.BadRequest, false, _ms.MessageService(Message.ShopPlanAdd));
            }
            var mapPlan = _mapper.Map<TShopPlan>(shopPlan);
            var data = await _settingRepository.EditShopPlan(mapPlan);
            if (data == false)
            {
                return new ApiResponse<bool>(ResponseStatusEnum.BadRequest, false,_ms.MessageService(Message.ShopPlanEdit));
            }
            return new ApiResponse<bool>(ResponseStatusEnum.Success, true,_ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<bool>> ChangeStatusShopPlan(AcceptDto accept)
        {
            var data = await _settingRepository.ChangeStatusShopPlan(accept);
            if (data == false)
            {
                return new ApiResponse<bool>(ResponseStatusEnum.BadRequest, false,_ms.MessageService(Message.ShopPlanEdit));
            }
            return new ApiResponse<bool>(ResponseStatusEnum.Success, true,_ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<List<ShopPlanDto>>> GetShopPlans()
        {
            var data = await _settingRepository.GetShopPlans();
            if (data == null)
            {
                return new ApiResponse<List<ShopPlanDto>>(ResponseStatusEnum.BadRequest, null,_ms.MessageService(Message.ShopPlanGetting));
            }
            return new ApiResponse<List<ShopPlanDto>>(ResponseStatusEnum.Success, data,_ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<ShopPlanDto>> GetShopPlansOne(int planId)
        {
            var data = await _settingRepository.GetShopPlansOne(planId);
            if (data == null)
            {
                return new ApiResponse<ShopPlanDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.ShopPlanGetting));
            }
            return new ApiResponse<ShopPlanDto>(ResponseStatusEnum.Success, data, _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<bool>> EditPaymentMethod(AcceptDto accept)
        {
            var data = await _settingRepository.EditPaymentMethod(accept);
            if (data == false)
            {
                return new ApiResponse<bool>(ResponseStatusEnum.BadRequest, false,_ms.MessageService(Message.PaymentMethodEdit));
            }
            return new ApiResponse<bool>(ResponseStatusEnum.Success, true,_ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<List<PaymentMethodDto>>> GetPaymentMethod()
        {
            var data = await _settingRepository.GetPaymentMethod();
            if (data == null)
            {
                return new ApiResponse<List<PaymentMethodDto>>(ResponseStatusEnum.BadRequest, null,_ms.MessageService(Message.PaymentMethodGetting));
            }
            return new ApiResponse<List<PaymentMethodDto>>(ResponseStatusEnum.Success, data,_ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<bool>> ChangeAcceptShipingMethod(AcceptDto accept)
        {
            var data = await _settingRepository.ChangeAcceptShipingMethod(accept);
            if (data == false)
            {
                return new ApiResponse<bool>(ResponseStatusEnum.BadRequest, false,_ms.MessageService(Message.ShippingMethodCanNotChange));
            }
            return new ApiResponse<bool>(ResponseStatusEnum.Success, true,_ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<bool>> EditShippingMethod(ShippingMethodDto shippingMethod)
        {
            foreach (var item in shippingMethod.TShippingOnCountry)
            {
                item.FkShippingMethodId = shippingMethod.Id;
                item.Id = 0;
            }
            var data = await _settingRepository.EditShippingMethod(_mapper.Map<TShippingMethod>(shippingMethod));
            if (data == false)
            {
                return new ApiResponse<bool>(ResponseStatusEnum.BadRequest, false,_ms.MessageService(Message.ShippingMethodEdit));
            }
            return new ApiResponse<bool>(ResponseStatusEnum.Success, true,_ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<bool>> EditShippingMethodDesc(ShippingMethodDto shippingMethod)
        {
          
            var data = await _settingRepository.EditShippingMethodDesc(_mapper.Map<TShippingMethod>(shippingMethod));
            if (data == false)
            {
                return new ApiResponse<bool>(ResponseStatusEnum.BadRequest, false,_ms.MessageService(Message.ShippingMethodEdit));
            }
            return new ApiResponse<bool>(ResponseStatusEnum.Success, true,_ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<bool>> EditShippingMethodCountryCost(List<ShippingOnCountryDto> shippingMethodOnCountry, int shippingMethodId)
        {
            foreach (var item in shippingMethodOnCountry)
            {
                item.FkShippingMethodId = shippingMethodId;
            }
            var data = await _settingRepository.EditShippingMethodCountryCost(_mapper.Map<List<TShippingOnCountry>>(shippingMethodOnCountry) , shippingMethodId);
            if (data == false)
            {
                return new ApiResponse<bool>(ResponseStatusEnum.BadRequest, false,_ms.MessageService(Message.ShippingMethodEdit));
            }
            return new ApiResponse<bool>(ResponseStatusEnum.Success, true,_ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<bool>> EditShippingMethodCityCost(List<ShippingOnCityDto> shippingMethodOnCity, int shippingMethodId)
        {
            foreach (var item in shippingMethodOnCity)
            {
                item.FkShippingMethodId = shippingMethodId;
            }
            var data = await _settingRepository.EditShippingMethodCityCost(_mapper.Map<List<TShippingOnCity>>(shippingMethodOnCity) , shippingMethodId);
            if (data == false)
            {
                return new ApiResponse<bool>(ResponseStatusEnum.BadRequest, false,_ms.MessageService(Message.ShippingMethodEdit));
            }
            return new ApiResponse<bool>(ResponseStatusEnum.Success, true,_ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<List<ShippingMethodDto>>> GetShippingMethod()
        {
            var data = await _settingRepository.GetShippingMethod();
            if (data == null)
            {
                return new ApiResponse<List<ShippingMethodDto>>(ResponseStatusEnum.BadRequest, null,_ms.MessageService(Message.ShippingMethodGetting));
            }
            return new ApiResponse<List<ShippingMethodDto>>(ResponseStatusEnum.Success, data,_ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<List<ShippingOnCityDto>>> GetShippingMethodCityCost(int shippingMethodId , int provinceId)
        {
            var data = await _settingRepository.GetShippingMethodCityCost(shippingMethodId , provinceId);
            if (data == null)
            {
                return new ApiResponse<List<ShippingOnCityDto>>(ResponseStatusEnum.BadRequest, null,_ms.MessageService(Message.ShippingMethodGetting));
            }
            return new ApiResponse<List<ShippingOnCityDto>>(ResponseStatusEnum.Success, data,_ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<List<ShippingOnCityDto>>> GetShippingMethodProvinceCost(int shippingMethodId , int countryId)
        {
            var data = await _settingRepository.GetShippingMethodProvinceCost(shippingMethodId , countryId);
            if (data == null)
            {
                return new ApiResponse<List<ShippingOnCityDto>>(ResponseStatusEnum.BadRequest, null,_ms.MessageService(Message.ShippingMethodGetting));
            }
            return new ApiResponse<List<ShippingOnCityDto>>(ResponseStatusEnum.Success, data,_ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<bool>> ChangeShippingMethod(ShippingMethodChangeDto shippingMethod)
        {
            var data = await _settingRepository.ChangeShippingMethod(shippingMethod);
            if (data == false)
            {
                return new ApiResponse<bool>(ResponseStatusEnum.BadRequest, false,_ms.MessageService(Message.ShippingMethodCanNotChange));
            }
            return new ApiResponse<bool>(ResponseStatusEnum.Success, true,_ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<int>> GeneralMinimumInventory()
        {
            var data = await _settingRepository.GeneralMinimumInventory();
 
            return new ApiResponse<int>(ResponseStatusEnum.Success, data ,_ms.MessageService(Message.Successfull));        }

        public async Task<ApiResponse<SettingDescriptionDto>> EditSettingRegistrationFinalMessage(SettingDescriptionDto settingDto)
        {
            var data = await _settingRepository.EditSettingRegistrationFinalMessage(settingDto);
            if (data == null)
            {
                return new ApiResponse<SettingDescriptionDto>(ResponseStatusEnum.BadRequest, null,_ms.MessageService(Message.SettingEdit));
            }
            return new ApiResponse<SettingDescriptionDto>(ResponseStatusEnum.Success, data,_ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<SettingDescriptionDto>> GetSettingRegistrationFinalMessage()
        {
            var data = await _settingRepository.GetSettingRegistrationFinalMessage();
            if (data == null)
            {
                return new ApiResponse<SettingDescriptionDto>(ResponseStatusEnum.BadRequest, null,_ms.MessageService(Message.SettingGetting));
            }
            return new ApiResponse<SettingDescriptionDto>(ResponseStatusEnum.Success, data,_ms.MessageService(Message.Successfull));
        }
    }
}