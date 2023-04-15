using System.Collections.Generic;
using System.Threading.Tasks;
using MarketPlace.API.Data.Dtos.Goods;
using MarketPlace.API.Data.Enums;
using MarketPlace.API.Data.Models;
using MarketPlace.API.Data.Repositories.IRepository;
using MarketPlace.API.Helper;
using MarketPlace.API.Services.IService;
using AutoMapper;
using MarketPlace.API.Data.Dtos.Token;
using MarketPlace.API.Data.Dtos.Header;
using Microsoft.AspNetCore.Http;
using MarketPlace.API.Data.Dtos.Category;
using MarketPlace.API.Data.Dtos.Variation;
using MarketPlace.API.Data.Dtos.Specification;
using System;
using MarketPlace.API.Data.Dtos.Accept;
using System.Linq;

namespace MarketPlace.API.Services.Service
{
    public class GoodsService : IGoodsService
    {
        public IMapper _mapper { get; }
        public IGoodsRepository _goodsRepository { get; }
        public IFileUploadService _fileUploadService { get; }
        public ICategoryRepository _categoryRepository { get; }
        public ISpecificationGroupRepository _specificationGroupRepository { get; }
        public ICategoryService _categoryService { get; }
        public IBrandRepository _brandRepository { get; }
        public IMeasurementUnitRepository _measurementUnitRepository { get; }
        public IGuaranteeRepository _guaranteeRepository { get; }
        public IVariationRepository _variationRepository { get; }
        public IMessageRepository _messageRepository { get; }
        public HeaderParseDto header { get; set; }
        public TokenParseDto token { get; set; }
        public IMessageLanguageService _ms { get; set; }

        public GoodsService(
        IMapper mapper,
        ICategoryRepository categoryRepository,
        IGoodsRepository goodsRepository,
        ISpecificationGroupRepository specificationGroupRepository,
        ICategoryService categoryService,
        IBrandRepository brandRepository,
        IMeasurementUnitRepository measurementUnitRepository,
        IFileUploadService fileUploadService,
        IGuaranteeRepository guaranteeRepository,
        IVariationRepository variationRepository,
        IMessageRepository messageRepository,
        IMessageLanguageService ms,
        IHttpContextAccessor httpContextAccessor)
        {
            this._measurementUnitRepository = measurementUnitRepository;
            this._brandRepository = brandRepository;
            this._categoryService = categoryService;
            this._specificationGroupRepository = specificationGroupRepository;
            this._categoryRepository = categoryRepository;
            this._goodsRepository = goodsRepository;
            this._messageRepository = messageRepository;
            this._guaranteeRepository = guaranteeRepository;
            this._mapper = mapper;
            this._fileUploadService = fileUploadService;
            _ms = ms;
            header = new HeaderParseDto(httpContextAccessor);
            token = new TokenParseDto(httpContextAccessor);
            _variationRepository = variationRepository;
        }


        public async Task<ApiResponse<GoodsBaseDetailDto>> GoodsAdd(GoodsSerializeDto goodsDto)
        {
            var goodObj = Extentions.Deserialize<GoodsDto>(goodsDto.Goods);
            if (goodObj == null)
            {
                return new ApiResponse<GoodsBaseDetailDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.GoodsDeserialize));
            }

            if (token.Rule == UserGroupEnum.Seller)
            {
                var havePermisionToAddGoods = await _goodsRepository.CanShopAddGoods(token.Id, goodObj.FkCategoryId);
                if (havePermisionToAddGoods.Result == false)
                {
                    return new ApiResponse<GoodsBaseDetailDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(havePermisionToAddGoods.Message));
                }
            }

            if (goodObj.HaveVariation == true && goodObj.GoodsProvider != null)
            {
                goodObj.GoodsProvider = null;
            }

            if (goodObj.GoodsProvider != null)
            {
                if (goodObj.GoodsProvider.ReturningAllowed == true && goodObj.GoodsProvider.MaxDeadlineDayToReturning == null)
                {
                    return new ApiResponse<GoodsBaseDetailDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.YouShouldSetMaxDeadlineDayToReturning));
                }
                if (goodObj.GoodsProvider.InventoryCount < 0)
                {
                    return new ApiResponse<GoodsBaseDetailDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.InventoryCountCantBeUnderZero));
                }
            }

            var fileName = _fileUploadService.UploadImageWhitThumb(goodsDto.Image, Pathes.GoodsImgTemp);
            if (fileName == null)
            {
                return new ApiResponse<GoodsBaseDetailDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.UploadFile));
            }
            goodObj.ImageUrl = fileName;

            if (goodObj.IsDownloadable == true)
            {
                goodObj.DownloadableFileUrl = _fileUploadService.UploadImage(goodsDto.File, Pathes.PrivateTemp);
                if (goodObj.DownloadableFileUrl == null)
                {
                    return new ApiResponse<GoodsBaseDetailDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.UploadFile));
                }
            }

            var mapGoods = _mapper.Map<TGoods>(goodObj);
            mapGoods.RegisterDate = DateTime.Now;
            mapGoods.ToBeDisplayed = true;


            if (token.Rule == UserGroupEnum.Admin)
            {
                mapGoods.IsCommonGoods = true;
                mapGoods.FkOwnerId = null;
                //  mapGoods.IsAccepted = true;
            }
            else
            {
                if (token.Rule == UserGroupEnum.Seller)
                {
                    if (await _goodsRepository.AcceptShopGoodsAdding())
                    {
                        mapGoods.IsAccepted = true;
                    }
                    else
                    {
                        mapGoods.IsAccepted = null;
                    }
                }
                mapGoods.IsCommonGoods = false;
                mapGoods.FkOwnerId = token.Id;
                // var result = await this.CanChangeGoodsShow(goodObj.FkCategoryId);
                // if (!result.Result)
                // {
                //     mapGoods.ToBeDisplayed = false;
                // }
                if (mapGoods.HaveVariation == false)
                {
                    if (goodObj.GoodsProvider != null)
                    {
                        mapGoods.TGoodsProvider = new List<TGoodsProvider>();
                        var mapGoodsProvider = _mapper.Map<TGoodsProvider>(goodObj.GoodsProvider);
                        mapGoodsProvider.FkGoodsId = 0;
                        mapGoodsProvider.FkShopId = token.Id;
                        mapGoodsProvider.TGoodsVariety = null;
                        if (mapGoodsProvider.InventoryCount == null)
                        {
                            mapGoodsProvider.HasInventory = false;
                            mapGoodsProvider.InventoryCount = 0;
                        }
                        else
                        {
                            if (mapGoodsProvider.InventoryCount > 0)
                            {
                                mapGoodsProvider.HasInventory = true;
                            }
                        }

                        mapGoods.TGoodsProvider.Add(mapGoodsProvider);
                    }
                }
                else
                {
                    mapGoods.TGoodsProvider = null;
                }
            }
            // mapGoods.SurveyCount = 1;
            // mapGoods.SurveyScore = new Random().Next(3, 5);
            var CreatedGoods = await _goodsRepository.GoodsAdd(mapGoods);
            if (CreatedGoods == null)
            {
                _fileUploadService.DeleteImageWithThumb(fileName, Pathes.GoodsImgTemp);
                if (goodObj.IsDownloadable)
                {
                    _fileUploadService.DeleteImage(goodObj.DownloadableFileUrl, Pathes.PrivateTemp);
                }
                return new ApiResponse<GoodsBaseDetailDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.GoodsAdding));
            }
            var isMoved = _fileUploadService.ChangeDestOfFileWhitThumb(fileName, Pathes.GoodsImgTemp, Pathes.Goods + CreatedGoods.GoodsId + "/");
            if (!isMoved)
            {
                _fileUploadService.DeleteImageWithThumb(fileName, Pathes.GoodsImgTemp);
                return new ApiResponse<GoodsBaseDetailDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.UploadFile));
            }
            if (goodObj.IsDownloadable)
            {
                var isMoved2 = _fileUploadService.ChangeDestOfFile(goodObj.DownloadableFileUrl, Pathes.PrivateTemp, Pathes.Private + CreatedGoods.GoodsId + "/");
                if (!isMoved2)
                {
                    _fileUploadService.DeleteImage(fileName, Pathes.PrivateTemp);
                    return new ApiResponse<GoodsBaseDetailDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.UploadFile));
                }
            }
            var GetGoodsBaseDetailDto = await this.GoodsBaseDataDetail(CreatedGoods.GoodsId);
            // ارسال پیام به مدیر وقتی فروشنده کالای جدید ثبت میکنند
            if(token.Rule == UserGroupEnum.Seller) {
                await _messageRepository.SendMessageToAdmin( " تامین کننده "  + CreatedGoods.FkOwner.StoreName + " کالای جدیدی را به کد  " + CreatedGoods.GoodsCode + "ثبت کرد", CreatedGoods.FkOwner.StoreName + " - ثبت کالای جدید" );
            }
            return GetGoodsBaseDetailDto;
        }


        public async Task<ApiResponse<GoodsBaseDetailDto>> GoodsEdit(GoodsSerializeDto goodsDto)
        {
            var goodObj = Extentions.Deserialize<GoodsDto>(goodsDto.Goods);
            if (goodObj == null)
            {
                return new ApiResponse<GoodsBaseDetailDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.GoodsDeserialize));
            }

            var goodsProviderIsAccepted = goodObj.GoodsProvider.IsAccepted;
            var goodsProviderId = goodObj.GoodsProvider.FkShopId;

            // if (goodObj.GoodsProvider.Price == 0)
            // {
            //     goodObj.GoodsProvider = null;
            // }

            if (goodObj.HaveVariation == true && goodObj.GoodsProvider != null)
            {
                goodObj.GoodsProvider = null;
            }

            if (goodObj.GoodsProvider != null)
            {
                if (goodObj.GoodsProvider.ReturningAllowed == true && goodObj.GoodsProvider.MaxDeadlineDayToReturning == null)
                {
                    return new ApiResponse<GoodsBaseDetailDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.YouShouldSetMaxDeadlineDayToReturning));
                }
                if (goodObj.GoodsProvider.InventoryCount < 0)
                {
                    return new ApiResponse<GoodsBaseDetailDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.InventoryCountCantBeUnderZero));
                }
            }

            if (token.Rule == UserGroupEnum.Seller)
            {
                if (!await _goodsRepository.ShopHasThisGoods(token.Id, goodObj.GoodsId, false))
                {
                    goodsDto.Image = null;
                    goodsDto.File = null;
                }
            }


            var fileName = "";
            var OldFileName = "";
            if (goodsDto.Image != null)
            {
                fileName = _fileUploadService.UploadImageWhitThumb(goodsDto.Image, Pathes.Goods + goodObj.GoodsId + "/");
                if (fileName == null)
                {
                    return new ApiResponse<GoodsBaseDetailDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.UploadFile));
                }
                OldFileName = goodObj.ImageUrl;
                goodObj.ImageUrl = fileName;
            }

            var oldPrivateFile = "";
            if (goodObj.IsDownloadable == true)
            {
                if (goodsDto.File != null)
                {
                    oldPrivateFile = goodObj.DownloadableFileUrl;
                    goodObj.DownloadableFileUrl = _fileUploadService.UploadImage(goodsDto.File, Pathes.Private + goodObj.GoodsId + "/");
                    if (goodObj.DownloadableFileUrl == null)
                    {
                        return new ApiResponse<GoodsBaseDetailDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.UploadFile));
                    }
                }
            }

            var mapGoods = _mapper.Map<TGoods>(goodObj);
            if (token.Rule == UserGroupEnum.Admin)
            {
                if (mapGoods.HaveVariation == false)
                {
                    if (goodObj.GoodsProvider != null)
                    {
                        mapGoods.TGoodsProvider = new List<TGoodsProvider>();
                        var mapGoodsProvider = _mapper.Map<TGoodsProvider>(goodObj.GoodsProvider);
                        mapGoodsProvider.FkGoodsId = goodObj.GoodsId;
                        mapGoodsProvider.TGoodsVariety = null;
                        if (mapGoodsProvider.DiscountAmount == null)
                        {
                            mapGoodsProvider.DiscountAmount = 0;
                        }
                        if (mapGoodsProvider.DiscountPercentage == null)
                        {
                            mapGoodsProvider.DiscountPercentage = 0;
                        }
                        if (mapGoodsProvider.InventoryCount == null)
                        {
                            mapGoodsProvider.HasInventory = false;
                            mapGoodsProvider.InventoryCount = 0;
                        }
                        else
                        {
                            if (mapGoodsProvider.InventoryCount > 0)
                            {
                                mapGoodsProvider.HasInventory = true;
                            }
                        }
                        mapGoods.TGoodsProvider.Add(mapGoodsProvider);
                    }
                }
                else
                {
                    mapGoods.TGoodsProvider = null;
                }
                // mapGoods.IsAccepted = true;
            }
            if (token.Rule == UserGroupEnum.Seller)
            {
             //   var result = await this.CanChangeGoodsShow(goodObj.FkCategoryId);
                if (token.Rule == UserGroupEnum.Seller)
                {
                    if (await _goodsRepository.AcceptShopGoodsAdding())
                    {
                        mapGoods.IsAccepted = true;
                    }
                    else
                    {
                        mapGoods.IsAccepted = null;
                    }
                }
                // if (!result.Result)
                // {
                //     mapGoods.ToBeDisplayed = false;
                // }
                if (mapGoods.HaveVariation == false)
                {
                    if (goodObj.GoodsProvider != null)
                    {
                        mapGoods.TGoodsProvider = new List<TGoodsProvider>();
                        var mapGoodsProvider = _mapper.Map<TGoodsProvider>(goodObj.GoodsProvider);
                        mapGoodsProvider.FkGoodsId = goodObj.GoodsId;
                        mapGoodsProvider.FkShopId = token.Id;
                        mapGoodsProvider.TGoodsVariety = null;
                        if (mapGoodsProvider.DiscountAmount == null)
                        {
                            mapGoodsProvider.DiscountAmount = 0;
                        }
                        if (mapGoodsProvider.DiscountPercentage == null)
                        {
                            mapGoodsProvider.DiscountPercentage = 0;
                        }
                        if (mapGoodsProvider.InventoryCount == null)
                        {
                            mapGoodsProvider.HasInventory = false;
                            mapGoodsProvider.InventoryCount = 0;
                        }
                        else
                        {
                            if (mapGoodsProvider.InventoryCount > 0)
                            {
                                mapGoodsProvider.HasInventory = true;
                            }
                        }
                        mapGoods.TGoodsProvider.Add(mapGoodsProvider);
                    }
                }
                else
                {
                    mapGoods.TGoodsProvider = null;
                }
            }
            var editedGoods = await _goodsRepository.GoodsEdit(mapGoods , goodsProviderIsAccepted , goodsProviderId);
            if (editedGoods.Result == false)
            {
                if (goodsDto.Image != null)
                {
                    _fileUploadService.DeleteImageWithThumb(fileName, Pathes.Goods + goodObj.GoodsId + "/");
                }
                if (goodsDto.File != null)
                {
                    _fileUploadService.DeleteImage(goodObj.DownloadableFileUrl, Pathes.Private + goodObj.GoodsId + "/");
                }
                return new ApiResponse<GoodsBaseDetailDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(editedGoods.Message));
            }
            if (goodsDto.Image != null)
            {
                _fileUploadService.DeleteImageWithThumb(OldFileName, Pathes.Goods + goodObj.GoodsId + "/");
            }
            if (goodsDto.File != null)
            {
                _fileUploadService.DeleteImage(oldPrivateFile, Pathes.Private + goodObj.GoodsId + "/");
            }
            var GetGoodsBaseDetailDto = await this.GoodsBaseDataDetail(goodObj.GoodsId);
            return GetGoodsBaseDetailDto;
        }

        public async Task<ApiResponse<bool>> GoodsExist(int goodsId)
        {
            var result = await _goodsRepository.GoodsExist(goodsId);
            if (result == false)
            {
                return new ApiResponse<bool>(ResponseStatusEnum.NotFound, result, _ms.MessageService(Message.GoodsNotFoundById));
            }
            else
            {
                return new ApiResponse<bool>(ResponseStatusEnum.Success, result, _ms.MessageService(Message.Successfull));
            }
        }

        public async Task<ApiResponse<GoodsDescriptionDto>> EditDescription(GoodsDescriptionDto goods)
        {
            if (token.Rule == UserGroupEnum.Seller)
            {
                if (!await _goodsRepository.ShopHasThisGoods(token.Id, goods.GoodsId, false))
                {
                    return new ApiResponse<GoodsDescriptionDto>(ResponseStatusEnum.Forbidden, null, _ms.MessageService(Message.GoodsEditing));
                }
            }
            var result = await _goodsRepository.EditDescription(goods);
            if (result == null)
            {
                return new ApiResponse<GoodsDescriptionDto>(ResponseStatusEnum.NotFound, null, _ms.MessageService(Message.GoodsEditing));
            }
            else
            {
                return new ApiResponse<GoodsDescriptionDto>(ResponseStatusEnum.Success, result, _ms.MessageService(Message.Successfull));
            }
        }

        public async Task<ApiResponse<GoodsDescriptionDto>> GetGoodsDescription(int goodsId)
        {
            if (token.Rule == UserGroupEnum.Seller)
            {
                if (!await _goodsRepository.ShopHasThisGoods(token.Id, goodsId, true))
                {
                    return new ApiResponse<GoodsDescriptionDto>(ResponseStatusEnum.Forbidden, null, null);
                }
            }
            var data = await _goodsRepository.GetGoodsDescription(goodsId);
            var goodsDesc = new GoodsDescriptionDto();
            goodsDesc.GoodsId = goodsId;
            goodsDesc.Description = data;
            return new ApiResponse<GoodsDescriptionDto>(ResponseStatusEnum.Success, goodsDesc, _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<bool>> ChangeAccept(List<AcceptNullDto> accept)
        {
             var data = false ;
            if (token.Rule == UserGroupEnum.Seller)
            {
              data = await _goodsRepository.ChangeProviderToBeDisplay(accept);
            }
            else {
              data = await _goodsRepository.ChangeAccept(accept);
            }
            if (data == false)
            {
                return new ApiResponse<bool>(ResponseStatusEnum.BadRequest, false, _ms.MessageService(Message.GoodsChangeStatus));
            }
            return new ApiResponse<bool>(ResponseStatusEnum.Success, true, _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<GoodsDocumentDto>> UploadGoodsDocument(GoodsDocumentAddDto GoodsDocument)
        {
            if (token.Rule == UserGroupEnum.Seller)
            {
                if (!await _goodsRepository.ShopHasThisGoods(token.Id, GoodsDocument.FkGoodsId, false))
                {
                    return new ApiResponse<GoodsDocumentDto>(ResponseStatusEnum.Forbidden, null, null);
                }
            }
            var VarietyPath = "";
            if (GoodsDocument.FkVarietyId != null)
            {
                VarietyPath = Pathes.Variety + GoodsDocument.FkVarietyId + "/";
            }
            var fileName = _fileUploadService.UploadImage(GoodsDocument.Document, Pathes.Goods + GoodsDocument.FkGoodsId + "/" + VarietyPath);
            if (fileName == null)
            {
                return new ApiResponse<GoodsDocumentDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.UploadFile));
            }
            var goodsDocument = new TGoodsDocument();
            goodsDocument.FkGoodsId = GoodsDocument.FkGoodsId;
            goodsDocument.DocumentUrl = fileName;
            goodsDocument.FkVarietyId = GoodsDocument.FkVarietyId;
            var CreateGoodsDocument = await _goodsRepository.UploadGoodsDocument(goodsDocument);
            if (CreateGoodsDocument == null)
            {
                _fileUploadService.DeleteImageWithThumb(fileName, Pathes.Goods + GoodsDocument.FkGoodsId + "/");
                return new ApiResponse<GoodsDocumentDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.UploadFile));
            }
            var MapreateGoodsDocument = _mapper.Map<GoodsDocumentDto>(CreateGoodsDocument);
            return new ApiResponse<GoodsDocumentDto>(ResponseStatusEnum.Success, MapreateGoodsDocument, _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<List<GoodsDocumentDto>>> GetGoodsDocumentById(int goodsId, int? varityId)
        {
            if (token.Rule == UserGroupEnum.Seller)
            {
                if (!await _goodsRepository.ShopHasThisGoods(token.Id, goodsId, true))
                {
                    return new ApiResponse<List<GoodsDocumentDto>>(ResponseStatusEnum.Forbidden, null, null);
                }
            }
            var GoodsDocument = await _goodsRepository.GetGoodsDocumentById(goodsId);
            if (GoodsDocument == null)
            {
                return new ApiResponse<List<GoodsDocumentDto>>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.GoodsDocumentGetting));
            }
            var mapGoodsDocument = _mapper.Map<List<GoodsDocumentDto>>(GoodsDocument);
            return new ApiResponse<List<GoodsDocumentDto>>(ResponseStatusEnum.Success, mapGoodsDocument, _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<bool>> DeleteImageById(int imageId, int goodsId)
        {
            if (token.Rule == UserGroupEnum.Seller)
            {
                if (!await _goodsRepository.ShopHasThisGoods(token.Id, goodsId, false))
                {
                    return new ApiResponse<bool>(ResponseStatusEnum.Forbidden, false, null);
                }
            }
            var GoodsDocument = await _goodsRepository.GoodsDocumentExist(imageId);
            if (GoodsDocument == false)
            {
                return new ApiResponse<bool>(ResponseStatusEnum.BadRequest, false, _ms.MessageService(Message.GoodsDocumentDelete));
            }
            var result = await _goodsRepository.DeleteImageById(imageId);
            var VarietyPath = "";
            if (result.Data.FkVarietyId != null)
            {
                VarietyPath = Pathes.Variety + result.Data.FkVarietyId + "/";
            }
            if (result.Result == false)
            {
                return new ApiResponse<bool>(ResponseStatusEnum.BadRequest, result.Result, result.Message);
            }
            else
            {
                _fileUploadService.DeleteImage(result.Data.DocumentUrl, Pathes.Goods + result.Data.FkGoodsId + "/");
                return new ApiResponse<bool>(ResponseStatusEnum.Success, result.Result, result.Message);
            }
        }

        public async Task<ApiResponse<List<GoodsBaseDetailDto>>> GetGoodsByCategoryId(int CategoryId, string Filter)
        {
            var ChildsId = await _categoryRepository.GetCategoriesChilds(CategoryId);
            if (ChildsId == null)
            {
                return new ApiResponse<List<GoodsBaseDetailDto>>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.GoodsGetting));
            }
            var goods = await _goodsRepository.GetGoodsByCategoryId(ChildsId, Filter);
            if (goods == null)
            {
                return new ApiResponse<List<GoodsBaseDetailDto>>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.GoodsGetting));
            }
            return new ApiResponse<List<GoodsBaseDetailDto>>(ResponseStatusEnum.Success, goods, _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<Pagination<GoodsListGetDto>>> GetAllGoodsByCategoryId(GoodsPaginationDto pagination)
        {
            if (token.Rule == UserGroupEnum.Seller)
            {
                pagination.ShopId = token.Id;
            }
            if (pagination.CategoryId != 0)
            {
                pagination.CatChilds = await _categoryRepository.GetCategoriesChilds(pagination.CategoryId);
            }
            var goods = await _goodsRepository.GetAllGoodsByCategoryId(pagination);
            if (goods == null)
            {
                return new ApiResponse<Pagination<GoodsListGetDto>>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.GoodsGetting));
            }
            var Count = await _goodsRepository.GetAllGoodsByCategoryIdCount(pagination);
            return new ApiResponse<Pagination<GoodsListGetDto>>(ResponseStatusEnum.Success, new Pagination<GoodsListGetDto>(Count, goods), _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<bool>> GoodsDelete(int goodsId)
        {
            if (token.Rule == UserGroupEnum.Seller)
            {
                if (!await _goodsRepository.ShopHasThisGoods(token.Id, goodsId, false))
                {
                    return new ApiResponse<bool>(ResponseStatusEnum.Forbidden, false, null);
                }
            }
            var result = await _goodsRepository.GoodsDelete(goodsId);
            if (result.Result == false)
            {
                return new ApiResponse<bool>(ResponseStatusEnum.BadRequest, result.Result, _ms.MessageService(result.Message));
            }
            else
            {
                _fileUploadService.DeleteDirectory(Pathes.Goods + result.Data.GoodsId);
                return new ApiResponse<bool>(ResponseStatusEnum.Success, result.Result, _ms.MessageService(result.Message));
            }
        }

        public async Task<ApiResponse<List<SpecificationGroupGetForGoodsDto>>> GetGoodsSpecification(int goodsId)
        {
            if (token.Rule == UserGroupEnum.Seller)
            {
                if (!await _goodsRepository.ShopHasThisGoods(token.Id, goodsId, true))
                {
                    return new ApiResponse<List<SpecificationGroupGetForGoodsDto>>(ResponseStatusEnum.Forbidden, null, null);
                }
            }
            var catid = await _categoryRepository.GetGoodsCategoryId(goodsId);
            if (catid == 0)
            {
                return new ApiResponse<List<SpecificationGroupGetForGoodsDto>>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.SpecificationGetting));
            }
            var categoryIds = await _categoryRepository.GetParentCatIds(catid);
            if (categoryIds == null)
            {
                categoryIds.Add(catid);
            }
            else if (categoryIds.Count == 0)
            {
                categoryIds.Add(catid);
            }
            var spec = await _specificationGroupRepository.SpecificationGroupGetWithCategoryId(categoryIds, goodsId);
            if (spec == null)
            {
                return new ApiResponse<List<SpecificationGroupGetForGoodsDto>>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.SpecificationGetting));
            }
            return new ApiResponse<List<SpecificationGroupGetForGoodsDto>>(ResponseStatusEnum.Success, spec, _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<bool>> CanChangeGoodsShow(int catId)
        {
            var categoryIds = await _categoryRepository.GetParentCatIds(catId);
            if (categoryIds == null)
            {
                categoryIds.Add(catId);
            }
            else if (categoryIds.Count == 0)
            {
                categoryIds.Add(catId);
            }
            var result = await _goodsRepository.CanChangeGoodsShow(categoryIds);
            if (result == false)
            {
                return new ApiResponse<bool>(ResponseStatusEnum.BadRequest, false, _ms.MessageService(Message.GoodsCantChangeGoodsShow));
            }
            else
            {
                return new ApiResponse<bool>(ResponseStatusEnum.Success, true, _ms.MessageService(Message.Successfull));
            }
        }

        public async Task<ApiResponse<bool>> GoodsShow(int goodsId)
        {
            if (token.Rule == UserGroupEnum.Seller)
            {
                if (!await _goodsRepository.ShopHasThisGoods(token.Id, goodsId, true))
                {
                    return new ApiResponse<bool>(ResponseStatusEnum.Forbidden, false, null);
                }
            }
            var catid = await _categoryRepository.GetGoodsCategoryId(goodsId);
            if (catid == 0)
            {
                return new ApiResponse<bool>(ResponseStatusEnum.BadRequest, false, _ms.MessageService(Message.GoodsCantChangeGoodsShow));
            }
            var can = await this.CanChangeGoodsShow(catid);
            if (can.Result == false)
            {
                return new ApiResponse<bool>((ResponseStatusEnum)can.Status, false, can.Message);
            }
            var result = await _goodsRepository.GoodsShow(goodsId);
            if (result == false)
            {
                return new ApiResponse<bool>(ResponseStatusEnum.BadRequest, false, _ms.MessageService(Message.GoodsChangeGoodsShow));
            }
            else
            {
                return new ApiResponse<bool>(ResponseStatusEnum.Success, true, _ms.MessageService(Message.Successfull));
            }
        }

        public async Task<ApiResponse<GoodsBaseDetailDto>> GoodsBaseDataDetail(int goodsId)
        {
            if (token.Rule == UserGroupEnum.Seller)
            {
                if (!await _goodsRepository.ShopHasThisGoods(token.Id, goodsId, true))
                {
                    return new ApiResponse<GoodsBaseDetailDto>(ResponseStatusEnum.Forbidden, null, null);
                }
            }
            var goodsBaseDetail = await _goodsRepository.GetGoodsBaseDetailDto(goodsId);
            if (goodsBaseDetail == null)
            {
                return new ApiResponse<GoodsBaseDetailDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.GoodsGetting));
            }
            return new ApiResponse<GoodsBaseDetailDto>(ResponseStatusEnum.Success, goodsBaseDetail, _ms.MessageService(Message.Successfull));
        }


        public async Task<ApiResponse<List<GoodsSpecificationDto>>> AddGoodsSpecification(List<GoodsSpecificationDto> goodsSpec)
        {
            if (token.Rule == UserGroupEnum.Seller)
            {
                if (!await _goodsRepository.ShopHasThisGoods(token.Id, goodsSpec[0].FkGoodsId, false))
                {
                    return new ApiResponse<List<GoodsSpecificationDto>>(ResponseStatusEnum.Forbidden, null, null);
                }
            }
            // var deleteGoodsSpec = await _goodsRepository.DeleteGoodsSpecificationByGoodsId(goodsSpec[0].FkGoodsId);
            // if (!deleteGoodsSpec)
            // {
            //     return new ApiResponse<List<GoodsSpecificationDto>>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.GoodsSpec));
            // }
            var mapGoodsSpecs = _mapper.Map<List<TGoodsSpecification>>(goodsSpec);
            var insertedGoodsSpecs = await _goodsRepository.AddGoodsSpecification(mapGoodsSpecs);
            if (insertedGoodsSpecs == null)
            {
                return new ApiResponse<List<GoodsSpecificationDto>>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.GoodsSpec));
            }
            var mapInsertedGoodsSpecs = _mapper.Map<List<GoodsSpecificationDto>>(insertedGoodsSpecs);
            return new ApiResponse<List<GoodsSpecificationDto>>(ResponseStatusEnum.Success, mapInsertedGoodsSpecs, _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<GoodsFormDto>> GetGoodsBaseDataById(int goodsId)
        {
            if (token.Rule == UserGroupEnum.Seller)
            {
                if (!await _goodsRepository.ShopHasThisGoods(token.Id, goodsId, true))
                {
                    return new ApiResponse<GoodsFormDto>(ResponseStatusEnum.Forbidden, null, null);
                }
            }
            var MeasurementUnit = await _measurementUnitRepository.MeasurementUnitGetAllForm();
            var Goods = new GoodsDto();
            var goodsBaseDetail = new GoodsBaseDetailDto();
            var category = new ApiResponse<List<CategoryTreeView>>(ResponseStatusEnum.BadRequest, null, null);
            if (goodsId != 0)
            {
                Goods = await _goodsRepository.GetGoodsById(goodsId);
                goodsBaseDetail = await _goodsRepository.GetGoodsBaseDetailDto(goodsId);
                category = await _categoryService.CategoryGetbyGoodsId(goodsId);
            }

            var categoryIds = await _categoryRepository.GetCategoriesChilds(Goods.FkCategoryId);
            var brands = await _brandRepository.GetBrandForWebsite(categoryIds);
            var gurrantee = await _guaranteeRepository.GetGuaranteeForWebsite(categoryIds);
            if (goodsId != 0)
            {
                if (MeasurementUnit == null || Goods == null || goodsBaseDetail == null || category.Result == null || gurrantee == null)
                {
                    return new ApiResponse<GoodsFormDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.GoodsGetting));
                }
            }
            else
            {
                if (MeasurementUnit == null || gurrantee == null)
                {
                    return new ApiResponse<GoodsFormDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.GoodsGetting));
                }
            }

            var GoodsForm = new GoodsFormDto();
            GoodsForm.Brands = brands;
            GoodsForm.Guarantee = gurrantee;
            GoodsForm.MeasurementUnit = MeasurementUnit;
            GoodsForm.Good = Goods;
            GoodsForm.GoodsBaseDetail = goodsBaseDetail;
            GoodsForm.Category = category.Result;

            return new ApiResponse<GoodsFormDto>(ResponseStatusEnum.Success, GoodsForm, _ms.MessageService(Message.Successfull));
        }


        public async Task<ApiResponse<GoodsDto>> GetGoodsById(int goodsId)
        {
            if (token.Rule == UserGroupEnum.Seller)
            {
                if (!await _goodsRepository.ShopHasThisGoods(token.Id, goodsId, true))
                {
                    return new ApiResponse<GoodsDto>(ResponseStatusEnum.Forbidden, null, null);
                }
            }
            var Goods = await _goodsRepository.GetGoodsById(goodsId);
            if (Goods == null)
            {
                return new ApiResponse<GoodsDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.GoodsGetting));
            }
            return new ApiResponse<GoodsDto>(ResponseStatusEnum.Success, Goods, _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<List<VariationParameterGetDto>>> GetVarityParameter(int goodsId)
        {
            if (token.Rule == UserGroupEnum.Seller)
            {
                if (!await _goodsRepository.ShopHasThisGoods(token.Id, goodsId, true))
                {
                    return new ApiResponse<List<VariationParameterGetDto>>(ResponseStatusEnum.Forbidden, null, null);
                }
            }
            var catIds = await _goodsRepository.GetGoodsParentCategoryIds(goodsId);
            if (catIds == null)
            {
                return new ApiResponse<List<VariationParameterGetDto>>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.VariationParameterGetting));
            }
            var data = await _variationRepository.GetVarityParameterByCategpryId(catIds);
            if (data == null)
            {
                return new ApiResponse<List<VariationParameterGetDto>>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.VariationParameterGetting));
            }
            return new ApiResponse<List<VariationParameterGetDto>>(ResponseStatusEnum.Success, data, _ms.MessageService(Message.Successfull));

        }

        public async Task<ApiResponse<bool>> AddGoodsProvider(GoodsProviderSerializeDto goodsProviderDto)
        {
            var goodProvidersObj = Extentions.Deserialize<GoodsProviderAddDto>(goodsProviderDto.GoodsProvider);
            var parameterIds = new List<int>();
            if (goodsProviderDto.ParameterValuesIds != null)
            {
                parameterIds = Extentions.Deserialize<List<int>>(goodsProviderDto.ParameterValuesIds);
            }
            if (goodProvidersObj == null)
            {
                return new ApiResponse<bool>(ResponseStatusEnum.BadRequest, false, _ms.MessageService(Message.GoodsDeserialize));
            }
            if (goodProvidersObj.TGoodsVariety == null)
            {
                return new ApiResponse<bool>(ResponseStatusEnum.BadRequest, false, _ms.MessageService(Message.PleaseChooseVariety));
            }
            if (goodProvidersObj.ReturningAllowed == true && goodProvidersObj.MaxDeadlineDayToReturning == null)
            {
                return new ApiResponse<bool>(ResponseStatusEnum.BadRequest, false, _ms.MessageService(Message.YouShouldSetMaxDeadlineDayToReturning));
            }
            if (goodProvidersObj.TGoodsVariety.Count == 0)
            {
                return new ApiResponse<bool>(ResponseStatusEnum.BadRequest, false, _ms.MessageService(Message.PleaseChooseVariety));
            }
            if (goodsProviderDto.Images != null)
            {
                if (parameterIds.Count != goodsProviderDto.Images.Count)
                {
                    return new ApiResponse<bool>(ResponseStatusEnum.BadRequest, false, _ms.MessageService(Message.GoodsDeserialize));
                }
            }
            if (token.Rule == UserGroupEnum.Seller)
            {
                //set the ShopHasThisGoods for showing true because the shop can add goods provider for the goods that admin created
                if (!await _goodsRepository.ShopHasThisGoods(token.Id, goodsProviderDto.GoodsId, true))
                {
                    return new ApiResponse<bool>(ResponseStatusEnum.Forbidden, false, null);
                }
                goodProvidersObj.FkShopId = token.Id;
            }
            var fileNames = new List<string>();
            if (goodsProviderDto.Images != null)
            {
                for (int i = 0; i < goodsProviderDto.Images.Count; i++)
                {
                    var fileName = _fileUploadService.UploadImage(goodsProviderDto.Images[i], Pathes.Goods + goodsProviderDto.GoodsId + "/" + Pathes.GoodsVariety);
                    if (fileName == null)
                    {
                        return new ApiResponse<bool>(ResponseStatusEnum.BadRequest, false, _ms.MessageService(Message.UploadFile));
                    }
                    fileNames.Add(fileName);
                    foreach (var item in goodProvidersObj.TGoodsVariety.Where(x => x.FkVariationParameterValueId == parameterIds[i]))
                    {
                        item.ImageUrl = fileName;
                    }
                }
            }
            goodProvidersObj.FkGoodsId = goodsProviderDto.GoodsId;
            var mapGoodsProvider = _mapper.Map<TGoodsProvider>(goodProvidersObj);
            var CreatedGoodsProvider = await _goodsRepository.AddGoodsProvider(mapGoodsProvider, parameterIds, fileNames);
            if (CreatedGoodsProvider.Result == false)
            {
                foreach (var item in fileNames)
                {
                    _fileUploadService.DeleteImageWithThumb(item, Pathes.Goods + goodsProviderDto.GoodsId + "/" + Pathes.GoodsVariety);
                }
                return new ApiResponse<bool>(ResponseStatusEnum.BadRequest, false, _ms.MessageService(CreatedGoodsProvider.Message));
            }

            return new ApiResponse<bool>(ResponseStatusEnum.Success, true, _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<List<GoodsProviderGetDto>>> GetGoodsProvider(int goodsId, int shopId)
        {
            if (token.Rule == UserGroupEnum.Seller)
            {
                shopId = token.Id;
                if (!await _goodsRepository.ShopHasThisGoods(token.Id, goodsId, true))
                {
                    return new ApiResponse<List<GoodsProviderGetDto>>(ResponseStatusEnum.Forbidden, null, null);
                }
            }
            var data = await _goodsRepository.GetGoodsProvider(goodsId, shopId);
            return new ApiResponse<List<GoodsProviderGetDto>>(ResponseStatusEnum.Success, data, _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<bool>> DeleteGoodsProvider(int goodsId,int goodsProviderId)
        {
            if (token.Rule == UserGroupEnum.Seller)
            {
                if (!await _goodsRepository.ShopHasThisGoods(token.Id, goodsId, true))
                {
                    return new ApiResponse<bool>(ResponseStatusEnum.Forbidden, false, null);
                }
            }
            var data = await _goodsRepository.DeleteGoodsProvider(goodsId , goodsProviderId, token.Id);
            if(data.Data == false) {
                    return new ApiResponse<bool>(ResponseStatusEnum.BadRequest, false,_ms.MessageService(data.Message));
            }
            return new ApiResponse<bool>(ResponseStatusEnum.Success, data.Data, _ms.MessageService(Message.Successfull));
        }


        public async Task<ApiResponse<GoodsMetaDto>> EditGoodsMetaService(GoodsMetaDto goodsmeta)
        {
            if (token.Rule == UserGroupEnum.Seller)
            {
                if (!await _goodsRepository.ShopHasThisGoods(token.Id, goodsmeta.GoodsId, false))
                {
                    return new ApiResponse<GoodsMetaDto>(ResponseStatusEnum.Forbidden, null, null);
                }
            }
            var result = await _goodsRepository.EditGoodsMeta(goodsmeta);
            if (result == null)
            {
                return new ApiResponse<GoodsMetaDto>(ResponseStatusEnum.NotFound, null, _ms.MessageService(Message.GoodsEditing));
            }
            else
            {
                return new ApiResponse<GoodsMetaDto>(ResponseStatusEnum.Success, result, _ms.MessageService(Message.Successfull));
            }
        }

        public async Task<ApiResponse<GoodsMetaDto>> GetGoodsMeta(int goodsId)
        {
            if (token.Rule == UserGroupEnum.Seller)
            {
                if (!await _goodsRepository.ShopHasThisGoods(token.Id, goodsId, true))
                {
                    return new ApiResponse<GoodsMetaDto>(ResponseStatusEnum.Forbidden, null, null);
                }
            }
            var data = await _goodsRepository.GetGoodsMeta(goodsId);
            return new ApiResponse<GoodsMetaDto>(ResponseStatusEnum.Success, data, _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<NoVariationGoodsProviderGetDto>> GetNoVariationGoodsProvider(int goodsId, int shopId)
        {
            if (token.Rule == UserGroupEnum.Seller)
            {
                shopId = token.Id;
                if (!await _goodsRepository.ShopHasThisGoods(token.Id, goodsId, true))
                {
                    return new ApiResponse<NoVariationGoodsProviderGetDto>(ResponseStatusEnum.Forbidden, null, null);
                }
            }
            var data = await _goodsRepository.GetNoVariationGoodsProvider(goodsId, shopId);
            return new ApiResponse<NoVariationGoodsProviderGetDto>(ResponseStatusEnum.Success, data, _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<bool>> GetGoodsIncludeVat(int shopId)
        {
            if (token.Rule == UserGroupEnum.Seller)
            {
                shopId = token.Id;
            }
            var data = await _goodsRepository.GetGoodsIncludeVat(shopId);
            return new ApiResponse<bool>(ResponseStatusEnum.Success, data, _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<bool>> GoodsGroupEditing(GoodsGroupEditingDto goodsGroupEditing)
        {
            var shopId = 0 ;
            if (token.Rule == UserGroupEnum.Seller)
            {
                shopId = token.Id;
            }
            var data = await _goodsRepository.GoodsGroupEditing(goodsGroupEditing , shopId);
            return new ApiResponse<bool>(ResponseStatusEnum.Success, data, _ms.MessageService(Message.Successfull));
        }
    }
}