using System.Threading.Tasks;
using AutoMapper;
using MarketPlace.API.Data.Dtos.Accept;
using MarketPlace.API.Data.Dtos.DocumentType;
using MarketPlace.API.Data.Dtos.Header;
using MarketPlace.API.Data.Dtos.Pagination;
using MarketPlace.API.Data.Dtos.Token;
using MarketPlace.API.Data.Enums;
using MarketPlace.API.Data.Models;
using MarketPlace.API.Data.Repositories.IRepository;
using MarketPlace.API.Helper;
using MarketPlace.API.Services.IService;
using Microsoft.AspNetCore.Http;


namespace MarketPlace.API.Services.Service
{
    public class DocumentTypeService : IDocumentTypeService
    {
        public IMapper _mapper { get; }
        public IDocumentTypeRepository _documentTypeRepository { get; }
        public HeaderParseDto header { get; set; }
        public TokenParseDto token { get; set; }
        public IMessageLanguageService _ms { get; set; }


        public DocumentTypeService(
        IMapper mapper,
        IDocumentTypeRepository documentTypeRepository,
        IMessageLanguageService ms,
        IHttpContextAccessor httpContextAccessor)
        {
            header = new HeaderParseDto(httpContextAccessor);
            token = new TokenParseDto(httpContextAccessor);
            this._documentTypeRepository = documentTypeRepository;
            _ms = ms;
            this._mapper = mapper;
        }

        public async Task<ApiResponse<DocumentTypeDto>> DocumentTypeAdd(DocumentTypeDto documentType)
        {
            var mapDocumentType = _mapper.Map<TDocumentType>(documentType);
            var craetedDocumentType = await _documentTypeRepository.DocumentTypeAdd(mapDocumentType);
            if (craetedDocumentType == null)
            {
                return new ApiResponse<DocumentTypeDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.DocumentTypeAdding));
            }
            var mapCraetedDocumentType = _mapper.Map<DocumentTypeDto>(craetedDocumentType);
            return new ApiResponse<DocumentTypeDto>(ResponseStatusEnum.Success, mapCraetedDocumentType, _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<DocumentTypeDto>> DocumentTypeEdit(DocumentTypeDto documentType)
        {
            var exist = await this.DocumentTypeExist(documentType.DocumentTypeId);
            if (exist.Status == (int)ResponseStatusEnum.NotFound)
            {
                return new ApiResponse<DocumentTypeDto>((ResponseStatusEnum)exist.Status, null, _ms.MessageService(exist.Message));
            }
            var mapDocumentType = _mapper.Map<TDocumentType>(documentType);
            var editedDocumentType = await _documentTypeRepository.DocumentTypeEdit(mapDocumentType);
            if (editedDocumentType == null)
            {
                return new ApiResponse<DocumentTypeDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.DocumentTypeEditing));
            }
            var mapEditedDocumentType = _mapper.Map<DocumentTypeDto>(editedDocumentType);
            return new ApiResponse<DocumentTypeDto>(ResponseStatusEnum.Success, mapEditedDocumentType, _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<bool>> DocumentTypeDelete(int id)
        {
            var exist = await this.DocumentTypeExist(id);
            if (exist.Status == (int)ResponseStatusEnum.NotFound)
            {
                return new ApiResponse<bool>((ResponseStatusEnum)exist.Status, exist.Result, _ms.MessageService(exist.Message));
            }
            var result = await _documentTypeRepository.DocumentTypeDelete(id);
            if (result.Result == false)
            {
                return new ApiResponse<bool>(ResponseStatusEnum.BadRequest, result.Result, _ms.MessageService(result.Message));
            }
            else
            {
                return new ApiResponse<bool>(ResponseStatusEnum.Success, result.Result, _ms.MessageService(result.Message));
            }
        }

        public async Task<ApiResponse<bool>> DocumentTypeExist(int id)
        {
            var result = await _documentTypeRepository.DocumentTypeExist(id);
            if (result == false)
            {
                return new ApiResponse<bool>(ResponseStatusEnum.NotFound, result, _ms.MessageService(Message.DocumentTypeNotFoundById));
            }
            else
            {
                return new ApiResponse<bool>(ResponseStatusEnum.Success, result, _ms.MessageService(Message.Successfull));
            }
        }

        public async Task<ApiResponse<Pagination<DocumentTypeGetDto>>> DocumentTypeGetAll(PaginationDto pagination)
        {
            var data = await _documentTypeRepository.DocumentTypeGetAll(pagination);
            if (data == null)
            {
                return new ApiResponse<Pagination<DocumentTypeGetDto>>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.DocumentTypeGetting));
            }
            var count = await _documentTypeRepository.DocumentTypeGetAllCount(pagination);
            return new ApiResponse<Pagination<DocumentTypeGetDto>>(ResponseStatusEnum.Success, new Pagination<DocumentTypeGetDto>(count, data), _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<DocumentTypeGetDto>> GetDocumentTypeById(int documentTypeId)
        {
            var data = await _documentTypeRepository.GetDocumentTypeById(documentTypeId);
            if (data == null)
            {
                return new ApiResponse<DocumentTypeGetDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.DocumentTypeGetting));
            }
            return new ApiResponse<DocumentTypeGetDto>(ResponseStatusEnum.Success, data, _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<bool>> ChangeAccept(AcceptDto accept)
        {
            var data = await _documentTypeRepository.ChangeAccept(accept);
            if (data == false)
            {
                return new ApiResponse<bool>(ResponseStatusEnum.BadRequest, false, _ms.MessageService(Message.DocumentTypeChangeStatus));
            }
            return new ApiResponse<bool>(ResponseStatusEnum.Success, true, _ms.MessageService(Message.Successfull));
        }
    }
}