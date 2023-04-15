using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MarketPlace.API.Data.Dtos.WebSlider;
using MarketPlace.API.Data.Models;
using MarketPlace.API.Data.Repositories.IRepository;
using MarketPlace.API.Helper;
using MarketPlace.API.Services.IService;
using MarketPlace.API.Data.Enums;
using System;
using MarketPlace.API.Data.Dtos.Category;
using MarketPlace.API.Data.Dtos.Image;
using MarketPlace.API.Data.Dtos.ChangePriority;

namespace MarketPlace.API.Services.Service
{
    public class WebSliderService : IWebSliderService
    {
        public IMapper _mapper { get; }
        public IFileUploadService _fileUploadService { get; }
        IGoodsRepository _goodsRepository { get; }
        ICategoryService _categoryService { get; }
        ICategoryRepository _categoryRepository { get; }
        IDiscountRepository _discountRepository { get; }
        public IWebSliderRepository _webSliderRepository { get; }
        public IMessageLanguageService _ms { get; }
        public WebSliderService(IMapper mapper, IWebSliderRepository webSliderRepository, IFileUploadService fileUploadService,
        IGoodsRepository goodsRepository, ICategoryService categoryService,
        IMessageLanguageService ms
        , IDiscountRepository discountRepository
        , ICategoryRepository categoryRepository)
        {
            this._discountRepository = discountRepository;
            this._goodsRepository = goodsRepository;
            this._categoryService = categoryService;
            this._fileUploadService = fileUploadService;
            this._webSliderRepository = webSliderRepository;
            this._categoryRepository = categoryRepository;
            this._mapper = mapper;
            _ms = ms;
        }
        public async Task<ApiResponse<WebSliderAddDto>> SliderAdd(WebSliderSerializeDto webSliderDto)
        {
            var slider = Extentions.Deserialize<WebSliderAddDto>(webSliderDto.WebSlider);
            if (slider == null)
            {
                return new ApiResponse<WebSliderAddDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.SliderDeserialize));
            }
            if (webSliderDto.SliderImg == null)
            {
                return new ApiResponse<WebSliderAddDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.UploadFile));
            }
            var MaxCountSlider = await _webSliderRepository.SliderCount(slider.FkCategoryId);
            if (MaxCountSlider == 6)
            {
                return new ApiResponse<WebSliderAddDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.SliderCount));
            }
            var fileName = "";
            var ResponsiveImage = "";
            if (webSliderDto.SliderImg != null)
            {
                fileName = _fileUploadService.UploadImage(webSliderDto.SliderImg, Pathes.Slider);
                if (fileName == null)
                {
                    return new ApiResponse<WebSliderAddDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.UploadFile));
                }
            }
            if (webSliderDto.ResponsiveImage != null)
            {
                ResponsiveImage = _fileUploadService.UploadImage(webSliderDto.ResponsiveImage, Pathes.Slider);
                if (ResponsiveImage == null)
                {
                    return new ApiResponse<WebSliderAddDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.UploadFile));
                }
            }
            slider.ImageUrl = fileName;
            slider.ResponsiveImageUrl = ResponsiveImage;
            var mapWebSlider = _mapper.Map<WebSlider>(slider);
            var createWebSlider = await _webSliderRepository.WebSliderAdd(mapWebSlider);
            if (createWebSlider == null)
            {
                _fileUploadService.DeleteImage(fileName, Pathes.Slider);
                _fileUploadService.DeleteImage(ResponsiveImage, Pathes.Slider);
                return new ApiResponse<WebSliderAddDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.SliderAdding));
            }
            var mapCreateWebSlider = _mapper.Map<WebSliderAddDto>(createWebSlider);
            return new ApiResponse<WebSliderAddDto>(ResponseStatusEnum.Success, mapCreateWebSlider, _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<bool>> SliderExist(int id)
        {
            var result = await _webSliderRepository.SliderExist(id);
            if (result == false)
            {
                return new ApiResponse<bool>(ResponseStatusEnum.NotFound, result, _ms.MessageService(Message.SliderNotFoundById));
            }
            else
            {
                return new ApiResponse<bool>(ResponseStatusEnum.Success, result, _ms.MessageService(Message.Successfull));
            }
        }

        public async Task<ApiResponse<bool>> SliderDelete(int id)
        {
            var exist = await this.SliderExist(id);
            if (exist.Status == (int)ResponseStatusEnum.NotFound)
            {
                return new ApiResponse<bool>((ResponseStatusEnum)exist.Status, exist.Result, exist.Message);
            }
            var result = await _webSliderRepository.SliderDelete(id);
            if (result.Result == false)
            {
                return new ApiResponse<bool>(ResponseStatusEnum.BadRequest, result.Result, result.Message);
            }
            else
            {
                _fileUploadService.DeleteImage(result.Data.ImageUrl, Pathes.Slider);
                _fileUploadService.DeleteImage(result.Data.ResponsiveImageUrl, Pathes.Slider);
                return new ApiResponse<bool>(ResponseStatusEnum.Success, result.Result, _ms.MessageService(Message.Successfull));

            }
        }

        public async Task<ApiResponse<bool>> ChangePrioritySlider(ChangePriorityDto changePriority)
        {
            var result = await _webSliderRepository.ChangePrioritySlider(changePriority);
            if (result == true)
            {
                return new ApiResponse<bool>(ResponseStatusEnum.Success, true, _ms.MessageService(Message.Successfull));
            }
            else
            {
                return new ApiResponse<bool>(ResponseStatusEnum.BadRequest, false, _ms.MessageService(Message.SliderChangePriority));
            }
        }

        public async Task<ApiResponse<WebSliderAddDto>> SliderEdit(WebSliderSerializeDto webSlider)
        {
            var sliderObj = Extentions.Deserialize<WebSliderAddDto>(webSlider.WebSlider);
            if (sliderObj == null)
            {
                return new ApiResponse<WebSliderAddDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.SliderDeserialize));
            }
            var exist = await this.SliderExist(sliderObj.SliderId);
            if (exist.Status == (int)ResponseStatusEnum.NotFound)
            {
                return new ApiResponse<WebSliderAddDto>((ResponseStatusEnum)exist.Status, null, exist.Message);
            }
            if (webSlider.SliderImg != null)
            {
                // if (!String.IsNullOrWhiteSpace(sliderObj.ImageUrl))
                // {
                //     _fileUploadService.DeleteImage(sliderObj.ImageUrl, Pathes.Slider);
                // }
                sliderObj.ImageUrl = _fileUploadService.UploadImage(webSlider.SliderImg, Pathes.Slider);
                if (sliderObj.ImageUrl == null)
                {
                    return new ApiResponse<WebSliderAddDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.UploadFile));
                }
            }
            if (webSlider.ResponsiveImage != null)
            {
                // if (!String.IsNullOrWhiteSpace(sliderObj.ResponsiveImageUrl))
                // {
                //     _fileUploadService.DeleteImage(sliderObj.ResponsiveImageUrl, Pathes.Slider);
                // }
                sliderObj.ResponsiveImageUrl = _fileUploadService.UploadImage(webSlider.ResponsiveImage, Pathes.Slider);
                if (sliderObj.ResponsiveImageUrl == null)
                {
                    return new ApiResponse<WebSliderAddDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.UploadFile));
                }
            }
            var mapSlider = _mapper.Map<WebSlider>(sliderObj);
            var EditSlider = await _webSliderRepository.SliderEdit(mapSlider);
            if (EditSlider == null)
            {
                if (webSlider.SliderImg != null)
                {
                    _fileUploadService.DeleteImage(sliderObj.ImageUrl, Pathes.Slider);
                }
                if (webSlider.ResponsiveImage != null)
                {
                    _fileUploadService.DeleteImage(sliderObj.ResponsiveImageUrl, Pathes.Slider);
                }
                return new ApiResponse<WebSliderAddDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.SliderEditing));
            }
            var mapEditSlider = _mapper.Map<WebSliderAddDto>(EditSlider);
            return new ApiResponse<WebSliderAddDto>(ResponseStatusEnum.Success, mapEditSlider, _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<bool>> UploadSliderImage(UploadTowImageDto imageDto)
        {
            var exist = await this.SliderExist(imageDto.Id);
            if (exist.Status == (int)ResponseStatusEnum.NotFound)
            {
                return new ApiResponse<bool>((ResponseStatusEnum)exist.Status, false, exist.Message);
            }
            var fileName = "";
            var ResponsiveImage = "";
            if (imageDto.Image != null)
            {
                fileName = _fileUploadService.UploadImage(imageDto.Image, Pathes.Slider);
                if (fileName == null)
                {
                    return new ApiResponse<bool>(ResponseStatusEnum.BadRequest, false, _ms.MessageService(Message.UploadFile));
                }
            }
            if (imageDto.ResponsiveImage != null)
            {
                ResponsiveImage = _fileUploadService.UploadImage(imageDto.ResponsiveImage, Pathes.Slider);
                if (ResponsiveImage == null)
                {
                    return new ApiResponse<bool>(ResponseStatusEnum.BadRequest, false, _ms.MessageService(Message.UploadFile));
                }
            }
            var oldFileName = await _webSliderRepository.UploadSliderImage(fileName, ResponsiveImage, imageDto.Id);
            if (oldFileName != null)
            {
                if (imageDto.Image != null)
                {
                    _fileUploadService.DeleteImage(oldFileName.ImageUrl, Pathes.Slider);
                }
                if (imageDto.ResponsiveImage != null)
                {
                    _fileUploadService.DeleteImage(oldFileName.ResponsiveImageUrl, Pathes.Slider);
                }
                return new ApiResponse<bool>(ResponseStatusEnum.Success, true, _ms.MessageService(Message.Successfull));
            }
            else
            {
                return new ApiResponse<bool>(ResponseStatusEnum.BadRequest, false, _ms.MessageService(Message.UploadFile));
            }
        }

        public async Task<ApiResponse<WebSliderGetDto>> SliderGetById(int id)
        {
            var mapSlider = await _webSliderRepository.SliderGetById(id);
            if (mapSlider == null)
            {
                return new ApiResponse<WebSliderGetDto>(ResponseStatusEnum.BadRequest, null,_ms.MessageService(Message.SliderGetting));
            }            
            var ids = new List<int>();
            if (!string.IsNullOrWhiteSpace(mapSlider.XitemIds))
            {
                var baseIds = mapSlider.XitemIds.Split(',');
                for (int i = 0; i < baseIds.Length; i++)
                {
                    if (!string.IsNullOrWhiteSpace(baseIds[i]))
                    {
                        ids.Add(Int32.Parse(baseIds[i]));
                    }
                }
            }
            if (mapSlider.FkCollectionType != null)
            {
                if (mapSlider.FkCollectionType.SelectCategory)
                {
                    var CategorySelected = await _categoryRepository.GetCategoryByIds(ids.ToList());
                    var mapCategorySelected = _mapper.Map<List<CategoryAddGetDto>>(CategorySelected);
                    mapSlider.CategorySelected = mapCategorySelected.FirstOrDefault();
                }
                if (mapSlider.FkCollectionType.SelectGoods)
                {
                    var goods = await _goodsRepository.GoodsGetByIds(ids);
                    mapSlider.Goods = goods;

                }
                if (mapSlider.FkCollectionType.SelectSpecialSale)
                {
                    var discount = await _discountRepository.GetSpecialSellPlanByIds(ids);
                    mapSlider.SpecialSellPlan = discount;
                }
            }
            return new ApiResponse<WebSliderGetDto>(ResponseStatusEnum.Success, mapSlider, _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<List<WebSliderGetListDto>>> SliderGet()
        {
            var Sliders = await _webSliderRepository.SliderGet(2, (decimal)1.0, null);
            if (Sliders == null)
            {
                return new ApiResponse<List<WebSliderGetListDto>>(ResponseStatusEnum.BadRequest, null,_ms.MessageService(Message.SliderGetting));

            }
            return new ApiResponse<List<WebSliderGetListDto>>(ResponseStatusEnum.Success, Sliders, _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<List<WebSliderGetListDto>>> CategorySliderGet(int categoryId)
        {
            var Sliders = await _webSliderRepository.SliderGet(2, (decimal)1.0, categoryId);
            if (Sliders == null)
            {
                return new ApiResponse<List<WebSliderGetListDto>>(ResponseStatusEnum.BadRequest, null,_ms.MessageService(Message.SliderGetting));

            }
            return new ApiResponse<List<WebSliderGetListDto>>(ResponseStatusEnum.Success, Sliders, _ms.MessageService(Message.Successfull));
        }
    }
}