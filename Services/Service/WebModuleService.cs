using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MarketPlace.API.Data.Dtos.WebModule;
using MarketPlace.API.Data.Dtos.Height;
using MarketPlace.API.Data.Models;
using MarketPlace.API.Data.Repositories.IRepository;
using MarketPlace.API.Helper;
using MarketPlace.API.Services.IService;
using MarketPlace.API.Data.Dtos.ChangePriority;
using MarketPlace.API.Data.Dtos.Accept;
using MarketPlace.API.Data.Dtos.Category;
using MarketPlace.API.Data.Enums;
using MarketPlace.API.Data.Dtos.Header;
using MarketPlace.API.Data.Dtos.Token;
using Microsoft.AspNetCore.Http;
using MarketPlace.API.Data.Dtos.Image;
using System;

namespace MarketPlace.API.Services.Service
{
    public class WebModuleService : IWebModuleService
    {
        public IMapper _mapper { get; }
        public IWebModuleRepository _webModuleRepository { get; }
        IGoodsRepository _goodsRepository { get; }
        ICategoryService _categoryService { get; }
        ICategoryRepository _categoryRepository { get; }
        IDiscountRepository _discountRepository { get; }
        public IFileUploadService _fileUploadService { get; }
        public HeaderParseDto header { get; set; }
        public TokenParseDto token { get; set; }
        public IMessageLanguageService _ms { get; set; }
        public WebModuleService(IMapper mapper,
        IWebModuleRepository webModuleRepository,
        IFileUploadService fileUploadService,
        ICategoryService categoryService,
        ICategoryRepository categoryRepository,
        IMessageLanguageService ms,
        IGoodsRepository goodsRepository,
        IHttpContextAccessor httpContextAccessor,
        IDiscountRepository discountRepository)
        {
            header = new HeaderParseDto(httpContextAccessor);
            token = new TokenParseDto(httpContextAccessor);
            _ms = ms;
            this._discountRepository = discountRepository;
            this._goodsRepository = goodsRepository;
            this._categoryService = categoryService;
            this._categoryRepository = categoryRepository;
            this._fileUploadService = fileUploadService;
            this._webModuleRepository = webModuleRepository;
            this._mapper = mapper;
        }
        public async Task<ApiResponse<WebIndexModuleListAddDto>> WebIndexModuleListAdd(WebIndexModuleListAddDto webIndexModuleList)
        {
            var mapWebIndexModuleList = _mapper.Map<WebIndexModuleList>(webIndexModuleList);
            mapWebIndexModuleList.Status = false;
            var createWebIndexModuleList = await _webModuleRepository.WebIndexModuleListAdd(mapWebIndexModuleList);
            if (createWebIndexModuleList == null)
            {
                return new ApiResponse<WebIndexModuleListAddDto>(ResponseStatusEnum.BadRequest,null,_ms.MessageService(Message.ModuleAdding));
            }
            var mapCreateWebIndexModuleList = _mapper.Map<WebIndexModuleListAddDto>(createWebIndexModuleList);
            return new ApiResponse<WebIndexModuleListAddDto>(ResponseStatusEnum.Success,mapCreateWebIndexModuleList,_ms.MessageService(Message.Successfull));
        }
         public async Task<ApiResponse<WebIndexModuleListAddDto>> WebIndexModuleListEdit(WebIndexModuleListAddDto webIndexModuleList)
        {
            var mapWebIndexModuleList = _mapper.Map<WebIndexModuleList>(webIndexModuleList);
            var createWebIndexModuleList = await _webModuleRepository.WebIndexModuleListEdit(mapWebIndexModuleList);
            if (createWebIndexModuleList == null)
            {
                return new ApiResponse<WebIndexModuleListAddDto>(ResponseStatusEnum.BadRequest,null,_ms.MessageService(Message.ModuleEditing));
            }
            var mapCreateWebIndexModuleList = _mapper.Map<WebIndexModuleListAddDto>(createWebIndexModuleList);
            return new ApiResponse<WebIndexModuleListAddDto>(ResponseStatusEnum.Success,mapCreateWebIndexModuleList,_ms.MessageService(Message.Successfull));
        }
        public async Task<ApiResponse<bool>> UploadModuleListImage(WebIndexModuleListSerialieDto imageDto)
        {
            var Exist = await _webModuleRepository.WebIndexModuleListExist(imageDto.Id);
            if (!Exist)
            {
                return new ApiResponse<bool>(ResponseStatusEnum.BadRequest, false, _ms.MessageService(Message.ModuleNotFoundById));
            }
            var fileName = "";
            if (imageDto.Image != null)
            {
                fileName = _fileUploadService.UploadImage(imageDto.Image, Pathes.ModuleListImg + imageDto.Id + "/");
                if (fileName == null)
                {
                    return new ApiResponse<bool>(ResponseStatusEnum.BadRequest, false, _ms.MessageService(Message.UploadFile));
                }
            }
            var result = await _webModuleRepository.UploadModuleListImage(fileName, imageDto.Title, imageDto.Id);
            if (result == false)
            {
                _fileUploadService.DeleteImage(fileName, Pathes.ModuleListImg + imageDto.Id + "/");
                return new ApiResponse<bool>(ResponseStatusEnum.BadRequest, false, _ms.MessageService(Message.ModuleAdding));
            }
            return new ApiResponse<bool>(ResponseStatusEnum.Success, true, _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<bool>> ChangePriorityOfWebIndexModuleList(ChangePriorityDto changePriority)
        {
            if (!await _webModuleRepository.WebIndexModuleListExist(changePriority.Id))
            {
                return new ApiResponse<bool>(ResponseStatusEnum.BadRequest, false, _ms.MessageService(Message.ModuleNotFoundById));
            }
            var result = await _webModuleRepository.ChangePriorityOfWebIndexModuleList(changePriority);
            if (result == false)
            {
                return new ApiResponse<bool>(ResponseStatusEnum.BadRequest, false, _ms.MessageService(Message.ModuleChangePriority));
            }
            return new ApiResponse<bool>(ResponseStatusEnum.Success, true, _ms.MessageService(Message.Successfull));
        }


        public async Task<ApiResponse<bool>> ChangeAcceptOfWebIndexModuleList(AcceptDto accept)
        {
            if (!await _webModuleRepository.WebIndexModuleListExist(accept.Id))
            {
                return new ApiResponse<bool>(ResponseStatusEnum.BadRequest, false, _ms.MessageService(Message.ModuleNotFoundById));
            }
            var result = await _webModuleRepository.ChangeAcceptOfWebIndexModuleList(accept);
            if (result == false)
            {
                return new ApiResponse<bool>(ResponseStatusEnum.BadRequest, false, _ms.MessageService(Message.ModuleChangePriority));
            }
            return new ApiResponse<bool>(ResponseStatusEnum.Success, true, _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<bool>> WebIndexModuleListDelete(int id)
        {
            if (!await _webModuleRepository.WebIndexModuleListExist(id))
            {
                return new ApiResponse<bool>(ResponseStatusEnum.BadRequest, false, _ms.MessageService(Message.ModuleNotFoundById));
            }
            var DeleteWebIndexModuleList = await _webModuleRepository.WebIndexModuleListDelete(id);
            if (DeleteWebIndexModuleList == null)
            {
                return new ApiResponse<bool>(ResponseStatusEnum.BadRequest, false, _ms.MessageService(Message.ModuleDelete));
            }
            _fileUploadService.DeleteDirectory(Pathes.ModuleListImg + DeleteWebIndexModuleList.IModuleId);
            _fileUploadService.DeleteDirectory(Pathes.ModuleCollections + DeleteWebIndexModuleList.IModuleId);
            return new ApiResponse<bool>(ResponseStatusEnum.Success, true, _ms.MessageService(Message.Successfull));
        }


        public async Task<ApiResponse<WebModuleCollectionsAddDto>> WebModuleCollectionsAdd(WebModuleCollectionsSerializeDto collectionsDto)
        {
            var fileName = "";
            var ResponsivefileName = "";
            var CollectionsObj = Extentions.Deserialize<WebModuleCollectionsAddDto>(collectionsDto.WebModuleCollections);
            if (CollectionsObj == null)
            {
                    return new ApiResponse<WebModuleCollectionsAddDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.GoodsDeserialize));
            }

            if (collectionsDto.Image != null)
            {
                fileName = _fileUploadService.UploadImage(collectionsDto.Image, Pathes.ModuleImgTemp);
                if (fileName == null)
                {
                    return new ApiResponse<WebModuleCollectionsAddDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.UploadFile));
                }
                CollectionsObj.ImageUrl = fileName;
            }
            if (collectionsDto.ResponsiveImage != null)
            {
                ResponsivefileName = _fileUploadService.UploadImage(collectionsDto.ResponsiveImage, Pathes.ModuleImgTemp);
                if (ResponsivefileName == null)
                {
                    return new ApiResponse<WebModuleCollectionsAddDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.UploadFile));
                }
                CollectionsObj.ResponsiveImageUrl = ResponsivefileName;
            }
            var mapWebModuleCollections = _mapper.Map<WebModuleCollections>(CollectionsObj);
            var createWebModuleCollections = await _webModuleRepository.WebModuleCollectionsAdd(mapWebModuleCollections);
            if (createWebModuleCollections == null)
            {
                if (collectionsDto.Image != null)
                {
                    _fileUploadService.DeleteImage(fileName, Pathes.ModuleImgTemp);
                }
                if (collectionsDto.ResponsiveImage != null)
                {
                    _fileUploadService.DeleteImage(ResponsivefileName, Pathes.ModuleImgTemp);
                }
                return null;
            }
            if (collectionsDto.Image != null)
            {
                var isMoved = _fileUploadService.ChangeDestOfFile(fileName, Pathes.ModuleImgTemp, Pathes.ModuleCollections + createWebModuleCollections.FkIModuleId + "/" + createWebModuleCollections.CollectionId + "/");
                if (!isMoved)
                {
                    _fileUploadService.DeleteImage(fileName, Pathes.ModuleImgTemp);
                    return new ApiResponse<WebModuleCollectionsAddDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.UploadFile));
                }                
            }
            if (collectionsDto.ResponsiveImage != null)
            {
                var isMoved = _fileUploadService.ChangeDestOfFile(ResponsivefileName, Pathes.ModuleImgTemp, Pathes.ModuleCollections + createWebModuleCollections.FkIModuleId + "/" + createWebModuleCollections.CollectionId + "/");
                if (!isMoved)
                {
                    _fileUploadService.DeleteImage(ResponsivefileName, Pathes.ModuleImgTemp);
                    return new ApiResponse<WebModuleCollectionsAddDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.UploadFile));
                }
            }
            var mapCreateWebModuleCollections = _mapper.Map<WebModuleCollectionsAddDto>(createWebModuleCollections);
            return new ApiResponse<WebModuleCollectionsAddDto>(ResponseStatusEnum.Success, mapCreateWebModuleCollections, _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<bool>> ChangePriorityOfWebModuleCollections(ChangePriorityDto changePriority)
        {
            if (!await _webModuleRepository.WebModuleCollectionsExist(changePriority.Id))
            {
                return new ApiResponse<bool>(ResponseStatusEnum.BadRequest, false, _ms.MessageService(Message.ModuleNotFoundById));
            }
            var result = await _webModuleRepository.ChangePriorityOfWebModuleCollections(changePriority);
            if (result == false)
            {
                return new ApiResponse<bool>(ResponseStatusEnum.BadRequest, false, _ms.MessageService(Message.ModuleChangePriority));
            }
            return new ApiResponse<bool>(ResponseStatusEnum.Success, true, _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<List<WebCollectionTypeDto>>> GetWebCollectionType(bool slider)
        {
            var result = await _webModuleRepository.GetWebCollectionType(slider);
            var mapResult = _mapper.Map<List<WebCollectionTypeDto>>(result);
            return new ApiResponse<List<WebCollectionTypeDto>>(ResponseStatusEnum.Success,mapResult,_ms.MessageService(Message.Successfull));
        }
        public async Task<ApiResponse<List<WebHomeIndexModuleListDto>>> WebIndexModuleListGet()
        {
            var webModule = await _webModuleRepository.GetModuleCollection(2,(decimal)1.0,null); // get type 2 for admin
            return new ApiResponse<List<WebHomeIndexModuleListDto>>(ResponseStatusEnum.Success,webModule,_ms.MessageService(Message.Successfull));
        }
        public async Task<ApiResponse<List<WebHomeIndexModuleListDto>>> CategoryWebIndexModuleListGet(int categoryId)
        {
            var webModule = await _webModuleRepository.GetModuleCollection(2,(decimal)1.0,categoryId); // get type 2 for admin
            return new ApiResponse<List<WebHomeIndexModuleListDto>>(ResponseStatusEnum.Success,webModule,_ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<List<WebHomeModuleCollectionsDto>>> WebModuleCollections(int moduleId)
        {
            var moduleCollections = await _webModuleRepository.WebModuleCollectionsByModuleId(moduleId);
            return new ApiResponse<List<WebHomeModuleCollectionsDto>>(ResponseStatusEnum.Success,moduleCollections,_ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<bool>> WebModuleCollectionsDelete(int id)
        {
            if (!await _webModuleRepository.WebModuleCollectionsExist(id))
            {
                return new ApiResponse<bool>(ResponseStatusEnum.BadRequest, false, _ms.MessageService(Message.ModuleNotFoundById));
            }
            var DeleteWebModuleCollections = await _webModuleRepository.WebModuleCollectionsDelete(id);
            if (DeleteWebModuleCollections == null)
            {
                return new ApiResponse<bool>(ResponseStatusEnum.BadRequest,false,_ms.MessageService(Message.ModuleDelete));
            }
            _fileUploadService.DeleteDirectory(Pathes.ModuleCollections + Pathes.ModuleCollections + DeleteWebModuleCollections.FkIModuleId + "/" + DeleteWebModuleCollections.CollectionId);
            return new ApiResponse<bool>(ResponseStatusEnum.Success,true,_ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<WebModuleCollectionsAddDto>> WebModuleCollectionsEdit(WebModuleCollectionsSerializeDto collectionDto)
        {
            var collectionObj = Extentions.Deserialize<WebModuleCollectionsAddDto>(collectionDto.WebModuleCollections);
            if (collectionObj == null)
            {
                return new ApiResponse<WebModuleCollectionsAddDto>(ResponseStatusEnum.BadRequest,null,_ms.MessageService(Message.GoodsDeserialize));
            }
            var CollectionExist = await _webModuleRepository.WebModuleCollectionsExist(collectionObj.CollectionId);
            if (!CollectionExist)
            {
                return new ApiResponse<WebModuleCollectionsAddDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.ModuleNotFoundById));
            }
            if (collectionDto.Image != null)
            {
                if (!String.IsNullOrWhiteSpace(collectionObj.ImageUrl))
                {
                    _fileUploadService.DeleteImage(collectionObj.ImageUrl, Pathes.ModuleCollections + collectionObj.FkIModuleId + "/" + collectionObj.CollectionId);
                }
                collectionObj.ImageUrl = _fileUploadService.UploadImage(collectionDto.Image, Pathes.ModuleCollections + collectionObj.FkIModuleId + "/" + collectionObj.CollectionId + "/");
                if (collectionObj.ImageUrl == null)
                {
                    return new ApiResponse<WebModuleCollectionsAddDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.UploadFile));
                }
            }
            if (collectionDto.ResponsiveImage != null)
            {
                if (!String.IsNullOrWhiteSpace(collectionObj.ResponsiveImageUrl))
                {
                    _fileUploadService.DeleteImage(collectionObj.ResponsiveImageUrl, Pathes.ModuleCollections + collectionObj.FkIModuleId + "/" + collectionObj.CollectionId);
                }
                collectionObj.ResponsiveImageUrl = _fileUploadService.UploadImage(collectionDto.ResponsiveImage, Pathes.ModuleCollections + collectionObj.FkIModuleId + "/" + collectionObj.CollectionId + "/");
                if (collectionObj.ResponsiveImageUrl == null)
                {
                    return new ApiResponse<WebModuleCollectionsAddDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.UploadFile));
                }
            }
            var mapWebModuleCollections = _mapper.Map<WebModuleCollections>(collectionObj);
            var EditWebModuleCollections = await _webModuleRepository.WebModuleCollectionsEdit(mapWebModuleCollections);
            if (EditWebModuleCollections == null)
            {
                return null;
            }
            var mapEditWebModuleCollections = _mapper.Map<WebModuleCollectionsAddDto>(EditWebModuleCollections);
            return new ApiResponse<WebModuleCollectionsAddDto>(ResponseStatusEnum.Success,mapEditWebModuleCollections,_ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<bool>> UploadWebModuleCollectionsImage(UploadTowImageDto imageDto)
        {
            var Exist = await _webModuleRepository.WebModuleCollectionsExist(imageDto.Id);
            if (!Exist)
            {
                return new ApiResponse<bool>(ResponseStatusEnum.BadRequest, false, _ms.MessageService(Message.ModuleNotFoundById));
            }
            var fileName = "";
            var ResponsiveFileName = "";
            if (imageDto.Image != null)
            {
                fileName = _fileUploadService.UploadImage(imageDto.Image, Pathes.ModuleCollections + imageDto.ParentId + "/" + imageDto.Id + "/");
                if (fileName == null)
                {
                    return new ApiResponse<bool>(ResponseStatusEnum.BadRequest, false, _ms.MessageService(Message.UploadFile));
                }
            }
            if (imageDto.ResponsiveImage != null)
            {
                ResponsiveFileName = _fileUploadService.UploadImage(imageDto.ResponsiveImage, Pathes.ModuleCollections + imageDto.ParentId + "/" + imageDto.Id + "/");
                if (fileName == null)
                {
                    return new ApiResponse<bool>(ResponseStatusEnum.BadRequest, false, _ms.MessageService(Message.UploadFile));
                }
            }
            var oldFileName = await _webModuleRepository.UploadWebModuleCollectionsImage(fileName,ResponsiveFileName, imageDto.Id);
            if (oldFileName != null)
            {
                if(imageDto.Image != null)
                {
                    _fileUploadService.DeleteImage(oldFileName.ImageUrl, Pathes.ModuleCollections + oldFileName.FkIModuleId + "/" + oldFileName.CollectionId + "/");
                }
                if(imageDto.ResponsiveImage != null)
                {
                    _fileUploadService.DeleteImage(oldFileName.ResponsiveImageUrl, Pathes.ModuleCollections + oldFileName.FkIModuleId + "/" + oldFileName.CollectionId + "/");
                }
                return new ApiResponse<bool>(ResponseStatusEnum.Success, true, _ms.MessageService(Message.ModuleEditing));
            }
            else
            {
                _fileUploadService.DeleteImage(fileName, Pathes.ModuleCollections + imageDto.ParentId + "/" + imageDto.Id + "/");
                _fileUploadService.DeleteImage(ResponsiveFileName, Pathes.ModuleCollections + imageDto.ParentId + "/" + imageDto.Id + "/");
                return new ApiResponse<bool>(ResponseStatusEnum.BadRequest, false, _ms.MessageService(Message.Successfull));
            }
        }

        public async Task<ApiResponse<WebModuleCollectionsGetDto>> WebModuleCollectionsGetById(int id)
        {
            var mapCollection = await _webModuleRepository.WebModuleCollectionsGetById(id);
            if (mapCollection == null)
            {
                return null;
            }
            var ids = new List<int>();
            if (!string.IsNullOrWhiteSpace(mapCollection.XitemIds))
            {
                var baseIds = mapCollection.XitemIds.Split(',');
                for (int i = 0; i < baseIds.Length; i++)
                {
                    if (!string.IsNullOrWhiteSpace(baseIds[i]))
                    {
                        ids.Add(Int32.Parse(baseIds[i]));
                    }
                }
            }
            if (mapCollection.FkCollectionType != null)
            {
                if (mapCollection.FkCollectionType.SelectCategory)
                {
                    var CategorySelected = await _categoryRepository.GetCategoryByIds(ids);
                    var mapCategorySelected = _mapper.Map<List<CategoryAddGetDto>>(CategorySelected);
                    mapCollection.CategorySelected = mapCategorySelected.FirstOrDefault();
                }
                if (mapCollection.FkCollectionType.SelectGoods)
                {
                    var goods = await _goodsRepository.GoodsGetByIds(ids);
                    mapCollection.Goods = goods;
                }
                if (mapCollection.FkCollectionType.SelectSpecialSale)
                {
                    var discount = await _discountRepository.GetSpecialSellPlanByIds(ids);
                    mapCollection.SpecialSellPlan = discount;
                }
            }

            return new ApiResponse<WebModuleCollectionsGetDto>(ResponseStatusEnum.Success,mapCollection,_ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<bool>> ChangeHeightOfWebIndexModuleList(ChangeHeight changeHeight)
        {
            if (!await _webModuleRepository.WebIndexModuleListExist(changeHeight.Id))
            {
                return new ApiResponse<bool>(ResponseStatusEnum.BadRequest, false, _ms.MessageService(Message.ModuleNotFoundById));
            }
            var result = await _webModuleRepository.ChangeHeightOfWebIndexModuleList(changeHeight);
            if (result == false)
            {
                return new ApiResponse<bool>(ResponseStatusEnum.BadRequest, false, _ms.MessageService(Message.ModuleChangePriority));
            }
            return new ApiResponse<bool>(ResponseStatusEnum.Success, true, _ms.MessageService(Message.Successfull));
        }

    }
}