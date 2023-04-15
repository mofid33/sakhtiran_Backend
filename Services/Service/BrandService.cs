using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MarketPlace.API.Data.Dtos.Accept;
using MarketPlace.API.Data.Dtos.Pagination;
using MarketPlace.API.Data.Dtos.Brand;
using MarketPlace.API.Data.Models;
using MarketPlace.API.Data.Repositories.IRepository;
using MarketPlace.API.Helper;
using MarketPlace.API.Services.IService;
using MarketPlace.API.Data.Enums;
using MarketPlace.API.Data.Dtos.Header;
using MarketPlace.API.Data.Dtos.Token;
using Microsoft.AspNetCore.Http;

namespace MarketPlace.API.Services.Service
{
    public class BrandService : IBrandService
    {
        public IMapper _mapper { get; }
        public IBrandRepository _brandRepository { get; }
        public IFileUploadService _fileUploadService { get; }
        public HeaderParseDto header { get; set; }
        public TokenParseDto token { get; set; }
        public IMessageLanguageService _ms { get; set; }
        public ICategoryRepository _categoryRepository { get; set; }

        public BrandService(
        IMapper mapper,
        IBrandRepository brandRepository,
        IHttpContextAccessor httpContextAccessor,
        IMessageLanguageService ms,
        ICategoryRepository categoryRepository,
        IFileUploadService fileUploadService)
        {
            header = new HeaderParseDto(httpContextAccessor);
            token = new TokenParseDto(httpContextAccessor);
            this._brandRepository = brandRepository;
            this._mapper = mapper;
            this._fileUploadService = fileUploadService;
            this._ms = ms;
            _categoryRepository = categoryRepository;
        }
        public async Task<ApiResponse<BrandDto>> BrandAdd(BrandSerializeDto brandDto)
        {
            var brandObj = Extentions.Deserialize<BrandDto>(brandDto.Brand);
            if (brandObj == null)
            {
                return new ApiResponse<BrandDto>(ResponseStatusEnum.BadRequest, null,_ms.MessageService(Message.BrandDeserialize));
            }
            var existBrand = await _brandRepository.BrandExistWithTitle(brandObj.BrandTitle);
            if (existBrand)
            {
                return new ApiResponse<BrandDto>(ResponseStatusEnum.BadRequest, null,_ms.MessageService(Message.BrandExist));
            }
            if (token.Rule == UserGroupEnum.Seller)
            {
                if(await _brandRepository.AcceptShopBrandAdding())
                {
                    brandObj.IsAccepted = true;
                }
                else
                {
                    brandObj.IsAccepted = null;
                }
            }
            var BrandFileName = "";
            if (brandDto.Image != null)
            {
                BrandFileName = _fileUploadService.UploadImage(brandDto.Image, Pathes.BrandImgTemp);
                if (BrandFileName == null)
                {
                    return new ApiResponse<BrandDto>(ResponseStatusEnum.BadRequest, null,_ms.MessageService(Message.UploadFile));
                }
                brandObj.BrandLogoImage = BrandFileName;
            }
            var mapBrand = _mapper.Map<TBrand>(brandObj);
            var craetedBrand = await _brandRepository.BrandAdd(mapBrand);
            if (craetedBrand == null)
            {
                if (brandDto.Image != null)
                {
                    _fileUploadService.DeleteImage(BrandFileName, Pathes.BrandImgTemp);
                }
                return new ApiResponse<BrandDto>(ResponseStatusEnum.BadRequest, null,_ms.MessageService(Message.BrandAdding));
            }
            if (brandDto.Image != null)
            {
                var isMoved = _fileUploadService.ChangeDestOfFile(BrandFileName, Pathes.BrandImgTemp, Pathes.Brand + craetedBrand.BrandId + "/");
                if (!isMoved)
                {
                    _fileUploadService.DeleteImage(BrandFileName, Pathes.BrandImgTemp);
                }
            }
            var mapCraetedBrand = _mapper.Map<BrandDto>(craetedBrand);
            return new ApiResponse<BrandDto>(ResponseStatusEnum.Success, mapCraetedBrand,_ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<bool>> BrandDelete(int id)
        {
            var result = await _brandRepository.BrandDelete(id);
            if (result.Result == false)
            {
                return new ApiResponse<bool>(ResponseStatusEnum.BadRequest, result.Result,_ms.MessageService(result.Message) );
            }
            else
            {
                _fileUploadService.DeleteDirectory(Pathes.Brand + result.Data.BrandId);
                return new ApiResponse<bool>(ResponseStatusEnum.Success, result.Result,_ms.MessageService(result.Message) );
            }
        }

        public async Task<ApiResponse<BrandDto>> BrandEdit(BrandSerializeDto brandDto)
        {
            var brandObj = Extentions.Deserialize<BrandDto>(brandDto.Brand);
            if (brandObj == null)
            {
                return new ApiResponse<BrandDto>(ResponseStatusEnum.BadRequest, null,_ms.MessageService(Message.BrandDeserialize));
            }
            var exist = await this.BrandExist(brandObj.BrandId);
            if (exist.Status == (int)ResponseStatusEnum.NotFound)
            {
                return new ApiResponse<BrandDto>((ResponseStatusEnum)exist.Status, null,_ms.MessageService(exist.Message) );
            }
            var fileName = "";
            if (brandDto.Image != null)
            {
                fileName = _fileUploadService.UploadImage(brandDto.Image, Pathes.BrandImgTemp);
                if (fileName == null)
                {
                    return new ApiResponse<BrandDto>(ResponseStatusEnum.BadRequest, null,_ms.MessageService(Message.UploadFile));
                }
                var isMoved = _fileUploadService.ChangeDestOfFile(fileName, Pathes.BrandImgTemp, Pathes.Brand + brandObj.BrandId + "/");
                if (!isMoved)
                {
                    _fileUploadService.DeleteImage(fileName, Pathes.BrandImgTemp);
                }
                _fileUploadService.DeleteImage(brandObj.BrandLogoImage, Pathes.Brand + brandObj.BrandId + "/");
                brandObj.BrandLogoImage = fileName;
            }
            var mapBrand = _mapper.Map<TBrand>(brandObj);
            var editedBrand = await _brandRepository.BrandEdit(mapBrand);
            if (editedBrand == null)
            {
                return new ApiResponse<BrandDto>(ResponseStatusEnum.BadRequest, null,_ms.MessageService(Message.BrandEditing));
            }
            var mapEditedBrand = _mapper.Map<BrandDto>(editedBrand);
            return new ApiResponse<BrandDto>(ResponseStatusEnum.Success, mapEditedBrand,_ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<bool>> BrandExist(int id)
        {
            var result = await _brandRepository.BrandExist(id);
            if (result == false)
            {
                return new ApiResponse<bool>(ResponseStatusEnum.NotFound, result,_ms.MessageService(Message.BrandNotFoundById));
            }
            else
            {
                return new ApiResponse<bool>(ResponseStatusEnum.Success, result,_ms.MessageService(Message.Successfull));
            }
        }

        public async Task<ApiResponse<Pagination<BrandDto>>> BrandGetAll(PaginationDto pagination)
        {
            var ChildsId = new List<int>();
            if (token.Rule == UserGroupEnum.Seller)
            {
                pagination.shopId = token.Id;
                var shopCatIds = await _categoryRepository.GetShopCatIds(pagination.shopId);
                foreach (var item in shopCatIds)
                {
                    var ids = await _categoryRepository.GetCategoriesChilds(item);
                    ChildsId.AddRange(ids);
                }
                pagination.ChildIds = ChildsId.Distinct().ToList();
            }
            var data = await _brandRepository.BrandGetAll(pagination);
            if (data == null)
            {
                return new ApiResponse<Pagination<BrandDto>>(ResponseStatusEnum.BadRequest, null,_ms.MessageService(Message.BrandGetting));
            }
            var count = await _brandRepository.BrandGetAllCount(pagination);
            return new ApiResponse<Pagination<BrandDto>>(ResponseStatusEnum.Success, new Pagination<BrandDto>(count, data),_ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<bool>> ChangeAccept(List<AcceptNullDto> accept)
        {
            var result = await _brandRepository.ChangeAccept(accept);
            if (result == false)
            {
                return new ApiResponse<bool>(ResponseStatusEnum.BadRequest, false,_ms.MessageService(Message.BrandEditing));
            }
            else
            {
                return new ApiResponse<bool>(ResponseStatusEnum.Success, true,_ms.MessageService(Message.Successfull));
            }
        }

        public async Task<ApiResponse<BrandGetOneDto>> GetBrandById(int brandId)
        {
            var brand = await _brandRepository.GetBrandById(brandId);
            if (brand == null)
            {
                return new ApiResponse<BrandGetOneDto>(ResponseStatusEnum.BadRequest, null,_ms.MessageService(Message.BrandGetting));
            }
            else
            {
                return new ApiResponse<BrandGetOneDto>(ResponseStatusEnum.Success, brand,_ms.MessageService(Message.Successfull));
            }
        }
    }
}