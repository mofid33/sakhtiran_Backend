using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MarketPlace.API.Data.Dtos.Accept;
using MarketPlace.API.Data.Dtos.Pagination;
using MarketPlace.API.Data.Dtos.PupupItem;
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
    public class PupupService : IPupupService
    {
        public IMapper _mapper { get; }
        public IPupupRepository _PupupRepository { get; }
        public IFileUploadService _fileUploadService { get; }
        public HeaderParseDto header { get; set; }
        public TokenParseDto token { get; set; }
        public IMessageLanguageService _ms { get; set; }
        public ICategoryRepository _categoryRepository { get; set; }

        public PupupService(
        IMapper mapper,
        IPupupRepository PupupRepository,
        IHttpContextAccessor httpContextAccessor,
        IMessageLanguageService ms,
        ICategoryRepository categoryRepository,
        IFileUploadService fileUploadService)
        {
            header = new HeaderParseDto(httpContextAccessor);
            token = new TokenParseDto(httpContextAccessor);
            this._PupupRepository = PupupRepository;
            this._mapper = mapper;
            this._fileUploadService = fileUploadService;
            this._ms = ms;
            _categoryRepository = categoryRepository;
        }
        public async Task<ApiResponse<PupupItemDto>> PupupItemAdd(PupupItemSerializeDto pupupItemDto)
        {
            var pupupItemObj = Extentions.Deserialize<PupupItemDto>(pupupItemDto.PupupItem);
            if (pupupItemObj == null)
            {
                return new ApiResponse<PupupItemDto>(ResponseStatusEnum.BadRequest, null,_ms.MessageService(Message.PopupItemDeserialize));
            }
           
            var pupupItemFileName = "";
            if (pupupItemDto.Image != null)
            {
                pupupItemFileName = _fileUploadService.UploadImage(pupupItemDto.Image, Pathes.PupupImgTemp);
                if (pupupItemFileName == null)
                {
                    return new ApiResponse<PupupItemDto>(ResponseStatusEnum.BadRequest, null,_ms.MessageService(Message.UploadFile));
                }
                pupupItemObj.PopupImageUrl = pupupItemFileName;
            }
            var mapPupupItem = _mapper.Map<TPopupItem>(pupupItemObj);
            var craetedPupupItem = await _PupupRepository.PupupItemAdd(mapPupupItem);
            if (craetedPupupItem == null)
            {
                if (pupupItemDto.Image != null)
                {
                    _fileUploadService.DeleteImage(pupupItemFileName, Pathes.PupupImgTemp);
                }
                return new ApiResponse<PupupItemDto>(ResponseStatusEnum.BadRequest, null,_ms.MessageService(Message.PopupItemAdding));
            }
            if (pupupItemDto.Image != null)
            {
                var isMoved = _fileUploadService.ChangeDestOfFile(pupupItemFileName, Pathes.PupupImgTemp, Pathes.PupupImg + craetedPupupItem.PopupId + "/");
                if (!isMoved)
                {
                    _fileUploadService.DeleteImage(pupupItemFileName, Pathes.PupupImgTemp);
                }
            }
            var mapcraetedPupupItem = _mapper.Map<PupupItemDto>(craetedPupupItem);
            return new ApiResponse<PupupItemDto>(ResponseStatusEnum.Success, mapcraetedPupupItem,_ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<bool>> PupupItemDelete(int id)
        {
            var result = await _PupupRepository.PupupItemDelete(id);
            if (result.Result == false)
            {
                return new ApiResponse<bool>(ResponseStatusEnum.BadRequest, result.Result,_ms.MessageService(result.Message) );
            }
            else
            {
                _fileUploadService.DeleteDirectory(Pathes.PupupImg + result.Data.PopupId);
                return new ApiResponse<bool>(ResponseStatusEnum.Success, result.Result,_ms.MessageService(result.Message) );
            }
        }

        public async Task<ApiResponse<PupupItemDto>> PupupItemEdit(PupupItemSerializeDto PupupItemDto)
        {
            var pupupItemObj = Extentions.Deserialize<PupupItemDto>(PupupItemDto.PupupItem);
            if (pupupItemObj == null)
            {
                return new ApiResponse<PupupItemDto>(ResponseStatusEnum.BadRequest, null,_ms.MessageService(Message.PopupItemDeserialize));
            }
            var exist = await this.PupupItemExist(pupupItemObj.PopupId);
            if (exist.Status == (int)ResponseStatusEnum.NotFound)
            {
                return new ApiResponse<PupupItemDto>((ResponseStatusEnum)exist.Status, null,_ms.MessageService(exist.Message) );
            }
            var fileName = "";
            if (PupupItemDto.Image != null)
            {
                fileName = _fileUploadService.UploadImage(PupupItemDto.Image, Pathes.PupupImgTemp);
                if (fileName == null)
                {
                    return new ApiResponse<PupupItemDto>(ResponseStatusEnum.BadRequest, null,_ms.MessageService(Message.UploadFile));
                }
                var isMoved = _fileUploadService.ChangeDestOfFile(fileName, Pathes.PupupImgTemp, Pathes.PupupImg + pupupItemObj.PopupId + "/");
                if (!isMoved)
                {
                    _fileUploadService.DeleteImage(fileName, Pathes.PupupImgTemp);
                }
                _fileUploadService.DeleteImage(pupupItemObj.PopupImageUrl, Pathes.PupupImg + pupupItemObj.PopupId + "/");
                pupupItemObj.PopupImageUrl = fileName;
            }
            var mapPupupItem = _mapper.Map<TPopupItem>(pupupItemObj);
            var editedPupupItem = await _PupupRepository.PupupItemEdit(mapPupupItem);
            if (editedPupupItem == null)
            {
                return new ApiResponse<PupupItemDto>(ResponseStatusEnum.BadRequest, null,_ms.MessageService(Message.PopupItemEditing));
            }
            var mapeditedPupupItem = _mapper.Map<PupupItemDto>(editedPupupItem);
            return new ApiResponse<PupupItemDto>(ResponseStatusEnum.Success, mapeditedPupupItem,_ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<bool>> PupupItemExist(int id)
        {
            var result = await _PupupRepository.PupupItemExist(id);
            if (result == false)
            {
                return new ApiResponse<bool>(ResponseStatusEnum.NotFound, result,_ms.MessageService(Message.PopupItemNotFoundById));
            }
            else
            {
                return new ApiResponse<bool>(ResponseStatusEnum.Success, result,_ms.MessageService(Message.Successfull));
            }
        }

        public async Task<ApiResponse<Pagination<PupupItemDto>>> PupupItemGetAll(PaginationDto pagination)
        {
           
            var data = await _PupupRepository.PupupItemGetAll(pagination);
            if (data == null)
            {
                return new ApiResponse<Pagination<PupupItemDto>>(ResponseStatusEnum.BadRequest, null,_ms.MessageService(Message.PopupItemNotFoundById));
            }
            var count = await _PupupRepository.PupupItemGetAllCount(pagination);
            return new ApiResponse<Pagination<PupupItemDto>>(ResponseStatusEnum.Success, new Pagination<PupupItemDto>(count, data),_ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<bool>> ChangeAccept(AcceptDto  accept)
        {
            var result = await _PupupRepository.ChangeAccept(accept);
            if (result == false)
            {
                return new ApiResponse<bool>(ResponseStatusEnum.BadRequest, false,_ms.MessageService(Message.PopupItemEditing));
            }
            else
            {
                return new ApiResponse<bool>(ResponseStatusEnum.Success, true,_ms.MessageService(Message.Successfull));
            }
        }

        public async Task<ApiResponse<PupupItemDto>> GetPupupItemById(int id)
        {
            var pupupItem = await _PupupRepository.GetPupupItemById(id);
            if (pupupItem == null)
            {
                return new ApiResponse<PupupItemDto>(ResponseStatusEnum.BadRequest, null,_ms.MessageService(Message.PopupItemNotFoundById));
            }
            else
            {
                return new ApiResponse<PupupItemDto>(ResponseStatusEnum.Success, pupupItem,_ms.MessageService(Message.Successfull));
            }
        }

        public async Task<ApiResponse<PupupItemDto>> GetWebsitePupup()
        {
            var pupupItem = await _PupupRepository.GetWebsitePupup();
            return new ApiResponse<PupupItemDto>(ResponseStatusEnum.Success, pupupItem,_ms.MessageService(Message.Successfull));
            
        }

    }
}