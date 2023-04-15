using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using MarketPlace.API.Data.Dtos.Accept;
using MarketPlace.API.Data.Dtos.Image;
using MarketPlace.API.Data.Dtos.Pagination;
using MarketPlace.API.Data.Dtos.Shop;
using MarketPlace.API.Data.Dtos.ShopPlan;
using MarketPlace.API.Data.Dtos.UserOrder;
using MarketPlace.API.Data.Dtos.Token;
using MarketPlace.API.Data.Enums;
using MarketPlace.API.Data.Models;
using MarketPlace.API.Data.Repositories.IRepository;
using MarketPlace.API.Helper;
using MarketPlace.API.PaymentGateway.PaypalHelper;
using MarketPlace.API.Services.IService;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Linq;
using MarketPlace.API.PaymentGateway.CredimaxHelper;
using System;
using FSS.Pipe;
using System.Globalization;
using RestSharp;
using MarketPlace.API.Data.Dtos.Province;

namespace MarketPlace.API.Services.Service
{
    public class ShopService : IShopService
    {
        public IMapper _mapper { get; }
        public TokenParseDto token { get; set; }
        public IShopRepository _shopRepository { get; }
        public ICategoryRepository _categoryRepository { get; }
        public IFileUploadService _fileUploadService { get; }
        public ISettingRepository _settingRepository { get; }
        public IUserOrderRepository _userOrderRepository { get; }
        public IMessageLanguageService _ms { get; }
        public IConfiguration Configuration { get; }

        public ShopService(IMapper mapper,
         IShopRepository shopRepository,
         ISettingRepository settingRepository,
         IUserOrderRepository userOrderRepository,
        IMessageLanguageService ms,
        IConfiguration Configuration,
        IHttpContextAccessor httpContextAccessor,
        ICategoryRepository categoryRepository, IFileUploadService fileUploadService)
        {
            this._fileUploadService = fileUploadService;
            this._categoryRepository = categoryRepository;
            this._shopRepository = shopRepository;
            this._userOrderRepository = userOrderRepository;
            this._settingRepository = settingRepository;
            this._mapper = mapper;
            this.Configuration = Configuration;
            _ms = ms;
            token = new TokenParseDto(httpContextAccessor);
        }

        public async Task<ApiResponse<Pagination<ShopListGetDto>>> GetShopList(ShopListPaginationDto pagination)
        {
            var data = await _shopRepository.GetShopList(pagination);
            if (data == null)
            {
                return new ApiResponse<Pagination<ShopListGetDto>>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.ShopListGetting));
            }
            var count = await _shopRepository.GetShopListCount(pagination);
            return new ApiResponse<Pagination<ShopListGetDto>>(ResponseStatusEnum.Success, new Pagination<ShopListGetDto>(count, data), _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<ShopGeneralDto>> GetShopGeneralDetail(int shopId)
        {
            if (token.Rule == UserGroupEnum.Seller)
            {
                shopId = token.Id;
            }
            var data = await _shopRepository.GetShopGeneralDetail(shopId);
            if (data == null)
            {
                return new ApiResponse<ShopGeneralDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.ShopGetting));
            }
            return new ApiResponse<ShopGeneralDto>(ResponseStatusEnum.Success, data, _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<ShopGeneralDto>> EditShopGeneralDetail(ShopGeneralDto shopDto)
        {
            if (token.Rule == UserGroupEnum.Seller)
            {
                shopDto.ShopId = token.Id;
            }
            var editData = await _shopRepository.EditShopGeneralDetail(shopDto);
            if (editData.Result == false)
            {
                return new ApiResponse<ShopGeneralDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(editData.Message));
            }
            return new ApiResponse<ShopGeneralDto>(ResponseStatusEnum.Success, shopDto, _ms.MessageService(editData.Message));
        }

        public async Task<ApiResponse<ShopBalanceGetDto>> GetShopBalance(ShopBalancePagination pagination)
        {
            if (token.Rule == UserGroupEnum.Seller)
            {
                pagination.ShopId = token.Id;
            }
            var data = new ShopBalanceGetDto();
            data.AvailableBalance = await _shopRepository.GetAvailableBalance(pagination.ShopId);
            var Balance = await _shopRepository.GetShopBalance(pagination);
            var count = await _shopRepository.GetShopBalanceCount(pagination);
            data.ShopBalance = new Pagination<ShopBalanceDto>(count, Balance);
            return new ApiResponse<ShopBalanceGetDto>(ResponseStatusEnum.Success, data, _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<bool>> EditShopDescription(ShopDescriptionDto shopDto)
        {
            if (token.Rule == UserGroupEnum.Seller)
            {
                shopDto.ShopId = token.Id;
            }
            var editData = await _shopRepository.EditShopDescription(shopDto);
            if (editData.Result == false)
            {
                return new ApiResponse<bool>(ResponseStatusEnum.BadRequest, false, _ms.MessageService(editData.Message));
            }
            return new ApiResponse<bool>(ResponseStatusEnum.Success, true, _ms.MessageService(editData.Message));
        }

        public async Task<ApiResponse<bool>> EditShopTermsAndConditions(ShopDescriptionDto shopDto)
        {
            if (token.Rule == UserGroupEnum.Seller)
            {
                shopDto.ShopId = token.Id;
            }
            var editData = await _shopRepository.EditShopTermsAndConditions(shopDto);
            if (editData.Result == false)
            {
                return new ApiResponse<bool>(ResponseStatusEnum.BadRequest, false, _ms.MessageService(editData.Message));
            }
            return new ApiResponse<bool>(ResponseStatusEnum.Success, true, _ms.MessageService(editData.Message));
        }

        public async Task<ApiResponse<ShopDescriptionDto>> GetShopDescription(int shopId)
        {
            if (token.Rule == UserGroupEnum.Seller)
            {
                shopId = token.Id;
            }
            var data = await _shopRepository.GetShopDescription(shopId);
            if (data == null)
            {
                return new ApiResponse<ShopDescriptionDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.ShopGetting));
            }
            return new ApiResponse<ShopDescriptionDto>(ResponseStatusEnum.Success, data, _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<ShopDescriptionDto>> GetShopTermsAndConditions(int shopId)
        {
            if (token.Rule == UserGroupEnum.Seller)
            {
                shopId = token.Id;
            }
            var data = await _shopRepository.GetShopTermsAndConditions(shopId);
            if (data == null)
            {
                return new ApiResponse<ShopDescriptionDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.ShopGetting));
            }
            return new ApiResponse<ShopDescriptionDto>(ResponseStatusEnum.Success, data, _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<List<ShopFilesGetDto>>> GetShopDocument(int shopId)
        {
            if (token.Rule == UserGroupEnum.Seller)
            {
                shopId = token.Id;
            }
            var data = await _shopRepository.GetShopDocument(shopId);
            if (data == null)
            {
                return new ApiResponse<List<ShopFilesGetDto>>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.ShopGetting));
            }
            return new ApiResponse<List<ShopFilesGetDto>>(ResponseStatusEnum.Success, data, _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<ShopProfileDto>> GetShopProfile(int shopId)
        {
            if (token.Rule == UserGroupEnum.Seller)
            {
                shopId = token.Id;
            }
            var data = await _shopRepository.GetShopProfile(shopId);
            if (data == null)
            {
                return new ApiResponse<ShopProfileDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.ShopGetting));
            }
            return new ApiResponse<ShopProfileDto>(ResponseStatusEnum.Success, data, _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<ShopProfileDto>> EditShopProfile(ShopProfileSerializeDto shopDto)
        {
            var logo = "";
            var profile = "";
            if (token.Rule == UserGroupEnum.Seller)
            {
                shopDto.ShopId = token.Id;
            }
            if (shopDto.Logo != null)
            {
                logo = _fileUploadService.UploadImage(shopDto.Logo, Pathes.Shop + shopDto.ShopId + "/");
                if (logo == null)
                {
                    return new ApiResponse<ShopProfileDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.UploadFile));
                }
            }
            if (shopDto.Profile != null)
            {
                profile = _fileUploadService.UploadImage(shopDto.Profile, Pathes.Shop + shopDto.ShopId + "/");
                if (profile == null)
                {
                    return new ApiResponse<ShopProfileDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.UploadFile));
                }
            }
            var editData = await _shopRepository.EditShopProfile(profile, logo, shopDto.ShopId,shopDto.IsLogoNull , shopDto.IsProfileNull);
            if (editData.Result == false)
            {
                if (shopDto.Logo != null)
                {
                    _fileUploadService.DeleteImage(logo, Pathes.Shop + shopDto.ShopId + "/");
                }
                if (shopDto.Profile != null)
                {
                    _fileUploadService.DeleteImage(profile, Pathes.Shop + shopDto.ShopId + "/");
                }
                return new ApiResponse<ShopProfileDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(editData.Message));
            }
            var shopProfile = new ShopProfileDto();
            shopProfile.ShopId = shopDto.ShopId;
            shopProfile.Profile = editData.Data.ProfileImage;
            shopProfile.Logo = editData.Data.LogoImage;
            if (shopDto.Logo != null)
            {
                shopProfile.Profile = logo;
                _fileUploadService.DeleteImage(editData.Data.LogoImage, Pathes.Shop + shopDto.ShopId + "/");
            }
            if (shopDto.Profile != null)
            {
                shopProfile.Profile = profile;
                _fileUploadService.DeleteImage(editData.Data.ProfileImage, Pathes.Shop + shopDto.ShopId + "/");
            }
            return new ApiResponse<ShopProfileDto>(ResponseStatusEnum.Success, shopProfile, _ms.MessageService(editData.Message));
        }

        public async Task<ApiResponse<bool>> EditShopSetting(ShopSetting shopDto)
        {
            if (token.Rule == UserGroupEnum.Seller)
            {
                shopDto.ShopId = token.Id;
            }
            var editData = await _shopRepository.EditShopSetting(shopDto);
            if (editData.Result == false)
            {
                return new ApiResponse<bool>(ResponseStatusEnum.BadRequest, false, _ms.MessageService(editData.Message));
            }
            return new ApiResponse<bool>(ResponseStatusEnum.Success, true, _ms.MessageService(editData.Message));
        }

        public async Task<ApiResponse<ShopSetting>> GetShopSetting(int shopId)
        {
            if (token.Rule == UserGroupEnum.Seller)
            {
                shopId = token.Id;
            }
            var data = await _shopRepository.GetShopSetting(shopId);
            if (data == null)
            {
                return new ApiResponse<ShopSetting>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.ShopGetting));
            }
            return new ApiResponse<ShopSetting>(ResponseStatusEnum.Success, data, _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<ShopBankInformationDto>> EditShopBankInformation(ShopSerializeDto shopDto)
        {
            var shopObj = Extentions.Deserialize<ShopBankInformationDto>(shopDto.Shop);
            var shopFileDeleted = Extentions.Deserialize<List<int>>(shopDto.DeleteFilesId);
            if (shopObj == null)
            {
                return new ApiResponse<ShopBankInformationDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.ShopDeserialize));
            }
            if (shopDto.Files != null)
            {
                if (shopDto.Files.Count != shopObj.TShopFiles.Count)
                {
                    return new ApiResponse<ShopBankInformationDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.UploadFile));
                }
                for (int i = 0; i < shopDto.Files.Count; i++)
                {
                    shopObj.TShopFiles[i].FileUrl = _fileUploadService.UploadImage(shopDto.Files[i], Pathes.Shop + shopObj.ShopId + "/" + Pathes.Docs);
                    if (shopObj.TShopFiles[i].FileUrl == null)
                    {
                        return new ApiResponse<ShopBankInformationDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.UploadFile));
                    }
                }
            }
            if (token.Rule == UserGroupEnum.Seller)
            {
                shopObj.ShopId = token.Id;
                foreach (var item in shopObj.TShopFiles)
                {
                    item.FkShopId = token.Id;
                }
            }
            var editData = await _shopRepository.EditShopBankInformation(shopObj,shopFileDeleted);
            if (editData == null)
            {
                foreach (var item in shopObj.TShopFiles)
                {
                    _fileUploadService.DeleteImage(item.FileUrl, Pathes.Shop + shopObj.ShopId + "/" + Pathes.Docs);
                }
                return new ApiResponse<ShopBankInformationDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(editData.Message));
            }
            foreach (var item in editData.Data)
            {
                _fileUploadService.DeleteImage(item.FileUrl, Pathes.Shop + shopObj.ShopId + "/" + Pathes.Docs);
            }
            return await this.GetShopBankInformation(shopObj.ShopId);
        }

        public async Task<ApiResponse<ShopTaxDto>> EditShopTax(ShopSerializeDto shopDto)
        {
            var shopObj = Extentions.Deserialize<ShopTaxDto>(shopDto.Shop);
            var shopFileDeleted = Extentions.Deserialize<List<int>>(shopDto.DeleteFilesId);
            if (shopObj == null)
            {
                return new ApiResponse<ShopTaxDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.ShopDeserialize));
            }
            if (shopDto.Files != null)
            {
                if (shopDto.Files.Count != shopObj.TShopFiles.Count)
                {
                    return new ApiResponse<ShopTaxDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.UploadFile));
                }
                for (int i = 0; i < shopDto.Files.Count; i++)
                {
                    shopObj.TShopFiles[i].FileUrl = _fileUploadService.UploadImage(shopDto.Files[i], Pathes.Shop + shopObj.ShopId + "/" + Pathes.Docs);
                    if (shopObj.TShopFiles[i].FileUrl == null)
                    {
                        return new ApiResponse<ShopTaxDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.UploadFile));
                    }
                }
            }
            if (token.Rule == UserGroupEnum.Seller)
            {
                shopObj.ShopId = token.Id;
                foreach (var item in shopObj.TShopFiles)
                {
                    item.FkShopId = token.Id;
                }
            }
            var editData = await _shopRepository.EditShopTax(shopObj,shopFileDeleted);
            if (editData == null)
            {
                foreach (var item in shopObj.TShopFiles)
                {
                    _fileUploadService.DeleteImage(item.FileUrl, Pathes.Shop + shopObj.ShopId + "/" + Pathes.Docs);
                }
                return new ApiResponse<ShopTaxDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(editData.Message));
            }
            foreach (var item in editData.Data)
            {
                _fileUploadService.DeleteImage(item.FileUrl, Pathes.Shop + shopObj.ShopId + "/" + Pathes.Docs);
            }
            return await this.GetShopTax(shopObj.ShopId);
        }

        public async Task<ApiResponse<ShopBankInformationDto>> GetShopBankInformation(int shopId)
        {
            if (token.Rule == UserGroupEnum.Seller)
            {
                shopId = token.Id;
            }
            var data = await _shopRepository.GetShopBankInformation(shopId);
            if (data == null)
            {
                return new ApiResponse<ShopBankInformationDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.ShopGetting));
            }
            return new ApiResponse<ShopBankInformationDto>(ResponseStatusEnum.Success, data, _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<ShopTaxDto>> GetShopTax(int shopId)
        {
            if (token.Rule == UserGroupEnum.Seller)
            {
                shopId = token.Id;
            }
            var data = await _shopRepository.GetShopTax(shopId);
            if (data == null)
            {
                return new ApiResponse<ShopTaxDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.ShopGetting));
            }
            return new ApiResponse<ShopTaxDto>(ResponseStatusEnum.Success, data, _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<bool>> EditShopActivityCountry(ShopActivityCountryEditDto shopDto)
        {
            if (token.Rule == UserGroupEnum.Seller)
            {
                shopDto.ShopId = token.Id;
            }
            var editData = await _shopRepository.EditShopActivityCountry(shopDto);
            if (editData.Result == false)
            {
                return new ApiResponse<bool>(ResponseStatusEnum.BadRequest, false, _ms.MessageService(editData.Message));
            }
            return new ApiResponse<bool>(ResponseStatusEnum.Success, true, _ms.MessageService(editData.Message));
        }

        public async Task<ApiResponse<Pagination<ShopActivityCountryGetDto>>> GetShopActivityCountry(PaginationFormDto pagination)
        {
            if (token.Rule == UserGroupEnum.Seller)
            {
                pagination.Id = token.Id;
            }
            var data = await _shopRepository.GetShopActivityCountry(pagination);
            if (data == null)
            {
                return new ApiResponse<Pagination<ShopActivityCountryGetDto>>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.ShopGetting));
            }
            var count = await _shopRepository.GetShopActivityCountryCount(pagination);
            return new ApiResponse<Pagination<ShopActivityCountryGetDto>>(ResponseStatusEnum.Success, new Pagination<ShopActivityCountryGetDto>(count, data), _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<bool>> EditShopActivityCity(ShopActivityCityEditDto shopDto)
        {
            if (token.Rule == UserGroupEnum.Seller)
            {
                shopDto.ShopId = token.Id;
            }
            var editData = await _shopRepository.EditShopActivityCity(shopDto);
            if (editData.Result == false)
            {
                return new ApiResponse<bool>(ResponseStatusEnum.BadRequest, false, _ms.MessageService(editData.Message));
            }
            return new ApiResponse<bool>(ResponseStatusEnum.Success, true, _ms.MessageService(editData.Message));
        }

        public async Task<ApiResponse<Pagination<ShopActivityCityGetDto>>> GetShopActivityCity(PaginationFormDto pagination , int provinceId)
        {
            if (token.Rule == UserGroupEnum.Seller)
            {
                pagination.Id = token.Id;
            }
            var data = await _shopRepository.GetShopActivityCity(pagination,provinceId);
            if (data == null)
            {
                return new ApiResponse<Pagination<ShopActivityCityGetDto>>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.ShopGetting));
            }
            var count = await _shopRepository.GetShopActivityCityCount(pagination,provinceId);
            return new ApiResponse<Pagination<ShopActivityCityGetDto>>(ResponseStatusEnum.Success, new Pagination<ShopActivityCityGetDto>(count, data), _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<bool>> EditShopActivityProvince(ShopActivityCityEditDto shopDto)
        {
            if (token.Rule == UserGroupEnum.Seller)
            {
                shopDto.ShopId = token.Id;
            }
            var editData = await _shopRepository.EditShopActivityProvince(shopDto);
            if (editData.Result == false)
            {
                return new ApiResponse<bool>(ResponseStatusEnum.BadRequest, false, _ms.MessageService(editData.Message));
            }
            return new ApiResponse<bool>(ResponseStatusEnum.Success, true, _ms.MessageService(editData.Message));
        }

        public async Task<ApiResponse<Pagination<ShopActivityCityGetDto>>> GetShopActivityProvince(PaginationFormDto pagination)
        {
            if (token.Rule == UserGroupEnum.Seller)
            {
                pagination.Id = token.Id;
            }
            var data = await _shopRepository.GetShopActivityProvince(pagination);
            if (data == null)
            {
                return new ApiResponse<Pagination<ShopActivityCityGetDto>>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.ShopGetting));
            }
            var count = await _shopRepository.GetShopActivityProvinceCount(pagination);
            return new ApiResponse<Pagination<ShopActivityCityGetDto>>(ResponseStatusEnum.Success, new Pagination<ShopActivityCityGetDto>(count, data), _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<List<ProvinceFormDto>>> GetAllShopProvince()
        {
            var shopId = 0 ;
            if (token.Rule == UserGroupEnum.Seller)
            {
                shopId = token.Id;
            }
            var data = await _shopRepository.GetAllShopProvince(shopId);
            if (data == null)
            {
                return new ApiResponse<List<ProvinceFormDto>>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.ShopGetting));
            }
            return new ApiResponse<List<ProvinceFormDto>>(ResponseStatusEnum.Success,  data, _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<ShopSliderDto>> AddShopSlider(UploadImageDto shopDto)
        {
            if (shopDto.Image == null)
            {
                return new ApiResponse<ShopSliderDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.UploadFile));
            }
            var CanAddSlider = await _shopRepository.CanAddSlider(shopDto.Id);
            if (CanAddSlider == false)
            {
                return new ApiResponse<ShopSliderDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.SliderCount));
            }
            var fileName = _fileUploadService.UploadImage(shopDto.Image, Pathes.Shop + shopDto.Id + "/" + Pathes.ShopSlider);
            if (fileName == null)
            {
                return new ApiResponse<ShopSliderDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.UploadFile));
            }
            var slider = new TShopSlider();
            slider.ImageUrl = fileName;
            slider.FkShopId = shopDto.Id;
            slider.Status = true;
            if (token.Rule == UserGroupEnum.Seller)
            {
                slider.FkShopId = token.Id;
            }
            var createWebSlider = await _shopRepository.AddShopSlider(slider);
            if (createWebSlider == null)
            {
                _fileUploadService.DeleteImage(fileName, Pathes.Shop + shopDto.Id + "/" + Pathes.ShopSlider);
                return new ApiResponse<ShopSliderDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.SliderAdding));
            }
            var mapCreateData = _mapper.Map<ShopSliderDto>(createWebSlider);
            return new ApiResponse<ShopSliderDto>(ResponseStatusEnum.Success, mapCreateData, _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<bool>> DeleteShopSlider(int sliderId)
        {
            var shopId = 0;
            if (token.Rule == UserGroupEnum.Seller)
            {
                shopId = token.Id;
            }
            var editData = await _shopRepository.DeleteShopSlider(sliderId, shopId);
            if (editData.Result == false)
            {
                return new ApiResponse<bool>(ResponseStatusEnum.BadRequest, false, _ms.MessageService(editData.Message));
            }
            _fileUploadService.DeleteImage(editData.Data.ImageUrl, Pathes.Shop + sliderId + "/" + Pathes.ShopSlider);
            return new ApiResponse<bool>(ResponseStatusEnum.Success, true, _ms.MessageService(editData.Message));
        }

        public async Task<ApiResponse<List<ShopSliderDto>>> GetShopSlider(int shopId)
        {
            if (token.Rule == UserGroupEnum.Seller)
            {
                shopId = token.Id;
            }
            var data = await _shopRepository.GetShopSlider(shopId);
            if (data == null)
            {
                return new ApiResponse<List<ShopSliderDto>>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.ShopGetting));
            }
            return new ApiResponse<List<ShopSliderDto>>(ResponseStatusEnum.Success, data, _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<bool>> ChangeShopSliderStatus(AcceptDto accept)
        {
            var shopId = 0;
            if (token.Rule == UserGroupEnum.Seller)
            {
                shopId = token.Id;
            }
            var editData = await _shopRepository.ChangeShopSliderStatus(accept, shopId);
            if (editData.Result == false)
            {
                return new ApiResponse<bool>(ResponseStatusEnum.BadRequest, false, _ms.MessageService(editData.Message));
            }
            return new ApiResponse<bool>(ResponseStatusEnum.Success, true, _ms.MessageService(editData.Message));
        }

        public async Task<ApiResponse<bool>> EditShopPlan(int shopId, int planId)
        {
            if (token.Rule == UserGroupEnum.Seller)
            {
                shopId = token.Id;
            }
            // var editData = await _shopRepository.EditShopPlan(shopId, planId);
            // if (editData.Result == false)
            // {
            //     return new ApiResponse<bool>(ResponseStatusEnum.BadRequest, false, _ms.MessageService(editData.Message));
            // }
            return new ApiResponse<bool>(ResponseStatusEnum.Success, true, _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<List<PlanShopDto>>> GetShopPlan(int shopId)
        {
            if (token.Rule == UserGroupEnum.Seller)
            {
                shopId = token.Id;
            }
            var data = await _shopRepository.GetShopPlan(shopId);
            if (data == null)
            {
                return new ApiResponse<List<PlanShopDto>>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.ShopGetting));
            }
            return new ApiResponse<List<PlanShopDto>>(ResponseStatusEnum.Success, data, _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<ShopStatisticsDto>> GetShopStatistics(int shopId)
        {
            if (token.Rule == UserGroupEnum.Seller)
            {
                shopId = token.Id;
            }
            var data = new ShopStatisticsDto();
            data.ActiveOrder = await _shopRepository.GetShopActiveOrderCount(shopId);
            data.ActiveProduct = await _shopRepository.GetShopActiveProductCount(shopId);
            data.Balance = await _shopRepository.GetAvailableBalance(shopId);
            data.Income = await _shopRepository.GetShopIncome(shopId);
            data.Orders = await _shopRepository.GetShopOrdersCount(shopId);
            data.OutOfStuck = await _shopRepository.GetShopOutOfStuckCount(shopId);
            data.Sales = await _shopRepository.GetShopSales(shopId);
            return new ApiResponse<ShopStatisticsDto>(ResponseStatusEnum.Success, data, _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<string>> GetShopUserName(int shopId)
        {
            if (token.Rule == UserGroupEnum.Seller)
            {
                shopId = token.Id;
            }
            var data = await _shopRepository.GetShopUserName(shopId);
            if (data == null)
            {
                return new ApiResponse<string>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.UserNameGetting));
            }
            return new ApiResponse<string>(ResponseStatusEnum.Success, data, _ms.MessageService(Message.Successfull));
        }
        public async Task<ApiResponse<List<ShopFilesGetDto>>> EditShopDocument(int shopId, ShopSerializeDto shopDto)
        {
            if (token.Rule == UserGroupEnum.Seller)
            {
                shopId = token.Id;
            }
            var shopObj = Extentions.Deserialize<List<ShopFileDto>>(shopDto.Shop);
            var shopFileDeleted = Extentions.Deserialize<List<int>>(shopDto.DeleteFilesId);
            if (shopObj == null)
            {
                return new ApiResponse<List<ShopFilesGetDto>>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.ShopDeserialize));
            }
            if (shopDto.Files != null)
            {
                if (shopDto.Files.Count != shopObj.Count)
                {
                    return new ApiResponse<List<ShopFilesGetDto>>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.UploadFile));
                }
                for (int i = 0; i < shopDto.Files.Count; i++)
                {
                    shopObj[i].FileUrl = _fileUploadService.UploadImage(shopDto.Files[i], Pathes.Shop + shopId + "/" + Pathes.Docs);
                    if (shopObj[i].FileUrl == null)
                    {
                        return new ApiResponse<List<ShopFilesGetDto>>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.UploadFile));
                    }
                    shopObj[i].FkShopId = shopId;
                }
            }
            var editData = await _shopRepository.EditShopDocument(shopId, shopObj,shopFileDeleted);
            if (editData == null)
            {
                foreach (var item in shopObj)
                {
                    _fileUploadService.DeleteImage(item.FileUrl, Pathes.Shop + shopId + "/" + Pathes.Docs);
                }
                return new ApiResponse<List<ShopFilesGetDto>>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(editData.Message));
            }
            foreach (var item in editData.Data)
            {
                _fileUploadService.DeleteImage(item.FileUrl, Pathes.Shop + shopId + "/" + Pathes.Docs);
            }
            return await this.GetShopDocument(shopId);
        }


        public async Task<ApiResponse<ShopAccessDto>> CheckShopAccess()
        {
            var shopId = 0;
            var result = new ShopAccessDto();
            if (token.Rule == UserGroupEnum.Seller)
            {
                shopId = token.Id;
            }
            var data = await _shopRepository.GetShopStoreName(shopId);
            if (data == null)
            {
                return new ApiResponse<ShopAccessDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.ShopGetting));
            }
            result.ShopStatusId = data.FkStatusId;
            result.ShopStatus = data.StatusTitle;
            result.PlanId = data.PlanId;
            result.ShopStatusDesc = data.StatusDesc;
            result.ShopRegisterDate = data.RegisteryDateTime;
            result.ShopStoreName = data.StoreName;
            result.VendorUrlid = data.VendorUrlid;
            result.ShopMessage = data.ShopMessage;
            result.Active = false;
            if (data.FkStatusId == (int)ShopStatusEnum.Active)
            {
                result.Active = true;
            }
            return new ApiResponse<ShopAccessDto>(ResponseStatusEnum.Success, result, _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<ShopCategoryPlanGetDto>> GetShopCategory(int shopId)
        {
            if (token.Rule == UserGroupEnum.Seller)
            {
                shopId = token.Id;
            }
            var data = await _shopRepository.GetShopCategory(shopId);
            if (data == null)
            {
                return new ApiResponse<ShopCategoryPlanGetDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.ShopCategoryGetting));
            }
            return new ApiResponse<ShopCategoryPlanGetDto>(ResponseStatusEnum.Success, data, _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<bool>> AddShopCategory(int shopId, int categoryId)
        {
            if (token.Rule == UserGroupEnum.Seller)
            {
                shopId = token.Id;
            }
            var editData = await _shopRepository.AddShopCategory(shopId, categoryId);
            if (editData.Result == false)
            {
                return new ApiResponse<bool>(ResponseStatusEnum.BadRequest, false, _ms.MessageService(editData.Message));
            }
            return new ApiResponse<bool>(ResponseStatusEnum.Success, true, _ms.MessageService(editData.Message));
        }

        public async Task<ApiResponse<bool>> ShopPlanDelete(int planId)
        {
            var result = await _shopRepository.ShopPlanDelete(planId);
            if (result.Result == false)
            {
                return new ApiResponse<bool>(ResponseStatusEnum.BadRequest, result.Result, _ms.MessageService(result.Message));
            }
            else
            {
                return new ApiResponse<bool>(ResponseStatusEnum.Success, result.Result, _ms.MessageService(result.Message));
            }

        }

        public async Task<ApiResponse<Pagination<ShopGeneralDto>>> GetWebShopList(ShopListWebPaginationDto pagination)
        {
            var data = await _shopRepository.GetWebShopList(pagination);
            if (data == null)
            {
                return new ApiResponse<Pagination<ShopGeneralDto>>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.ShopListGetting));
            }
            var count = await _shopRepository.GetWebShopListCount(pagination);
            return new ApiResponse<Pagination<ShopGeneralDto>>(ResponseStatusEnum.Success, new Pagination<ShopGeneralDto>(count, data), _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<bool>> ChangeShopStatus(AcceptNullDto accept)
        {
            var result = await _shopRepository.ChangeShopStatus(accept);
            if (result == false)
            {
                return new ApiResponse<bool>(ResponseStatusEnum.BadRequest, false, _ms.MessageService(Message.ShopGetting));
            }
            else
            {
                return new ApiResponse<bool>(ResponseStatusEnum.Success, true, _ms.MessageService(Message.Successfull));
            }
        }

        public async Task<ApiResponse<ShopBaseDto>> GetShopStoreName(int shopId)
        {
            if (token.Rule == UserGroupEnum.Seller)
            {
                shopId = token.Id;
            }
            var data = await _shopRepository.GetShopStoreName(shopId);
            if (data == null)
            {
                return new ApiResponse<ShopBaseDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.ShopGetting));
            }
            return new ApiResponse<ShopBaseDto>(ResponseStatusEnum.Success, data, _ms.MessageService(Message.Successfull));
        }




        public async Task<ApiResponse<bool>> EditInactiveShopMessage(ShopDescriptionDto shopDto)
        {
            if (token.Rule == UserGroupEnum.Seller)
            {
                shopDto.ShopId = token.Id;
            }
            var editData = await _shopRepository.EditInactiveShopMessage(shopDto);
            if (editData.Result == false)
            {
                return new ApiResponse<bool>(ResponseStatusEnum.BadRequest, false, _ms.MessageService(editData.Message));
            }
            return new ApiResponse<bool>(ResponseStatusEnum.Success, true, _ms.MessageService(Message.Successfull));
        }


        public async Task<ApiResponse<ShopDescriptionDto>> GetInactiveShopMessage(int shopId)
        {
            if (token.Rule == UserGroupEnum.Seller)
            {
                shopId = token.Id;
            }
            var data = await _shopRepository.GetInactiveShopMessage(shopId);
            if (data == null)
            {
                return new ApiResponse<ShopDescriptionDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.ShopGetting));
            }
            return new ApiResponse<ShopDescriptionDto>(ResponseStatusEnum.Success, data, _ms.MessageService(Message.Successfull));
        }



        // init payment plan

        public async Task<ApiResponse<string>> InitPlanPayment(ShopPlanPaymentDto shopPlanPaymentDto)
        {

            var transactionCode = await _settingRepository.GetRandomNumber();
            var editDataPlan = await _shopRepository.EditShopPlan(shopPlanPaymentDto, false, (shopPlanPaymentDto.PaymentId == (int)PaymentMethodEnum.PayPal ? (int)CurrencyEnum.USD : (int)CurrencyEnum.BHD));
            var shopDetail = await _shopRepository.GetShopGeneralDetail(shopPlanPaymentDto.ShopId);
            if (editDataPlan.Result == false)
            {
                return new ApiResponse<string>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(editDataPlan.Message));
            }

            if (editDataPlan.Data.RentPerMonth == (decimal)0.00 || token.Rule == UserGroupEnum.Admin)
            {
                return new ApiResponse<string>(ResponseStatusEnum.Success, "success", _ms.MessageService(editDataPlan.Message));
            }
            var paymentTransaction = new TPaymentTransaction();
            paymentTransaction.FkPlanId = editDataPlan.Data.PlanId;
            paymentTransaction.FkShopId = shopPlanPaymentDto.ShopId;
            paymentTransaction.PlanMonth = shopPlanPaymentDto.Month;

            // beh pardakht melat

            if (shopPlanPaymentDto.PaymentId == (int)PaymentMethodEnum.Mellat && !shopPlanPaymentDto.UseCredit)
            {

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
               "amount", (double)editDataPlan.Data.RentPerMonth * 10,
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
                "callBackUrl", Configuration["RedirectPanelUrl:returnUrl"],
                ParameterType.QueryString);
                request.AddParameter(
                "payerId", paymentTransaction.FkShopId,
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
                        paymentTransaction.PaymentId = transactionOrderId[1];
                        paymentTransaction.PaymentToken = transactionOrderId[1];
                        paymentTransaction.FkCurrencyId = (int)CurrencyEnum.TMN;
                        paymentTransaction.FkPaymentMethodId = (int)PaymentMethodEnum.Mellat;
                        var result = await _shopRepository.TransactionPlanPayment(paymentTransaction);

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



        public async Task<ApiResponse<bool>> GetStatusPlanPayment(string paymentId, string payerId)
        {
            var ShopId = 0;
            if (token.Rule == UserGroupEnum.Seller)
            {
                ShopId = token.Id;
            }
            var paymentTransaction = await _userOrderRepository.GetPaymentTransaction(paymentId);
            if (paymentTransaction == null)
            {
                return new ApiResponse<bool>(ResponseStatusEnum.BadRequest, false, _ms.MessageService(Message.OrderGetting));
            }
            if (paymentTransaction.Status == true)
            {
                return new ApiResponse<bool>(ResponseStatusEnum.Success, true, _ms.MessageService(Message.Successfull));
            }

                return new ApiResponse<bool>(ResponseStatusEnum.BadRequest, false, _ms.MessageService(Message.OrderGetting));
            

        }


        public async Task<ApiResponse<bool>> PayPlanPayment(string refId, string resCode, string saleId, string saleReferenceId)
        {
           
            var paymentTransaction = await _userOrderRepository.GetPaymentTransaction(refId);
            var ShopId = paymentTransaction.FkShopId;
            if (paymentTransaction == null)
            {
                return new ApiResponse<bool>(ResponseStatusEnum.BadRequest, false, _ms.MessageService(Message.OrderGetting));
            }
            if (paymentTransaction.Status == true)
            {
                return new ApiResponse<bool>(ResponseStatusEnum.Success, true, _ms.MessageService(Message.Successfull));
            }
            var planShopDetails = await _shopRepository.planShopDetailsWithPaymentId(refId);
            if (ShopId != (int)planShopDetails.FkShopId)
            {
                return new ApiResponse<bool>(ResponseStatusEnum.BadRequest, false, _ms.MessageService(Message.OrderGetting));
            }
            var paymentTrans = new ShopPlanPaymentDto();
            paymentTrans.PlanId = (int)planShopDetails.FkPlanId;
            paymentTrans.ShopId = (int)planShopDetails.FkShopId;
            paymentTrans.Month = (int)planShopDetails.PlanMonth;
            paymentTrans.PaymentPayId = refId;
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
            "orderId", saleId,
            ParameterType.QueryString);
            request.AddParameter(
           "saleOrderId", saleId,
           ParameterType.QueryString);
            request.AddParameter(
           "saleReferenceId", saleReferenceId,
           ParameterType.QueryString);
            IRestResponse response = await client.ExecuteAsync(request);

            if (response.IsSuccessful)
            {
                string resultCode = Extentions.getBetweenXmlFile(response.Content, "<return>", "</return>");
                if (resultCode == "0")
                {
                    var PayOrder = await _shopRepository.EditShopPlan(paymentTrans, true);
                    if (PayOrder.Result == false)
                    {
                        return new ApiResponse<bool>(ResponseStatusEnum.BadRequest, false, _ms.MessageService(PayOrder.Message));
                    }
                    else
                    {
                        return new ApiResponse<bool>(ResponseStatusEnum.Success,true, _ms.MessageService(PayOrder.Message));
                    }
                }
                else
                {
                    return new ApiResponse<bool>(ResponseStatusEnum.BadRequest, false, _ms.MessageService(Message.OrderGetting));
                }
            }
            else
            {
                return new ApiResponse<bool>(ResponseStatusEnum.BadRequest, false, _ms.MessageService(Message.OrderGetting));
            }

        }


        public async Task<ApiResponse<bool>> DeleteShopCategory(int categoryId, int shopId)
        {

            if (token.Rule == UserGroupEnum.Seller)
            {
                shopId = token.Id;
            }
            var ChildsId = await _categoryRepository.GetCategoriesChilds(categoryId);
            var editData = await _shopRepository.DeleteShopCategory(ChildsId, categoryId, shopId);
            if (editData.Result == false)
            {
                return new ApiResponse<bool>(ResponseStatusEnum.BadRequest, false, _ms.MessageService(editData.Message));
            }
            return new ApiResponse<bool>(ResponseStatusEnum.Success, true, _ms.MessageService(editData.Message));
        }




        public async Task<ApiResponse<bool>> ShopDelete(int shopId)
        {
            var result = await _shopRepository.ShopDelete(shopId);
            if (result.Result == false)
            {
                return new ApiResponse<bool>(ResponseStatusEnum.BadRequest, result.Result, _ms.MessageService(result.Message));
            }
            else
            {
                return new ApiResponse<bool>(ResponseStatusEnum.Success, result.Result, _ms.MessageService(result.Message));
            }

        }


        
        public async Task<ApiResponse<bool>> ChangeAccept(List<AcceptNullDto> accept)
        {
             var data = false ;
            
              data = await _shopRepository.ChangeAccept(accept);
            
            if (data == false)
            {
                return new ApiResponse<bool>(ResponseStatusEnum.BadRequest, false, _ms.MessageService(Message.GoodsChangeStatus));
            }
            return new ApiResponse<bool>(ResponseStatusEnum.Success, true, _ms.MessageService(Message.Successfull));
        }


    }
}