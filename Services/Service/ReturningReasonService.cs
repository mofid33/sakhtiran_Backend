using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using MarketPlace.API.Data.Dtos.ReturningReason;
using MarketPlace.API.Data.Models;
using MarketPlace.API.Data.Repositories.IRepository;
using MarketPlace.API.Helper;
using MarketPlace.API.Services.IService;
using MarketPlace.API.Data.Enums;
using MarketPlace.API.Data.Dtos.Pagination;
using MarketPlace.API.Data.Dtos.Header;
using MarketPlace.API.Data.Dtos.Token;
using Microsoft.AspNetCore.Http;
using MarketPlace.API.Data.Dtos.Accept;

namespace MarketPlace.API.Services.Service
{
    public class ReturningReasonService : IReturningReasonService
    {
        public IMapper _mapper { get; }
        public IReturningReasonRepository _returningReasonRepository { get; }
        public HeaderParseDto header { get; set; }
        public TokenParseDto token { get; set; }
        public IMessageLanguageService _ms { get; set; }

        public ReturningReasonService(IMapper mapper,
                IHttpContextAccessor httpContextAccessor,
        IMessageLanguageService ms, IReturningReasonRepository ReturningReasonRepository)
        {
            this._returningReasonRepository = ReturningReasonRepository;
            this._mapper = mapper;
            header = new HeaderParseDto(httpContextAccessor);
            this._ms = ms;
            token = new TokenParseDto(httpContextAccessor);
        }
        public async Task<ApiResponse<ReturningReasonDto>> ReturningReasonAdd(ReturningReasonDto ReturningReason)
        {
            var mapReturningReason = _mapper.Map<TReturningReason>(ReturningReason);
            var craetedReturningReason = await _returningReasonRepository.ReturningReasonAdd(mapReturningReason);
            if (craetedReturningReason == null)
            {
                return new ApiResponse<ReturningReasonDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.ReturningReasonAdding));
            }
            var mapCraetedReturningReason = _mapper.Map<ReturningReasonDto>(craetedReturningReason);
            return new ApiResponse<ReturningReasonDto>(ResponseStatusEnum.Success, mapCraetedReturningReason, _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<bool>> ReturningReasonDelete(int id)
        {
            var exist = await this.ReturningReasonExist(id);
            if (exist.Status == (int)ResponseStatusEnum.NotFound)
            {
                return new ApiResponse<bool>((ResponseStatusEnum)exist.Status, false, _ms.MessageService(exist.Message));
            }
            var result = await _returningReasonRepository.ReturningReasonDelete(id);
            if (result.Result == false)
            {
                return new ApiResponse<bool>(ResponseStatusEnum.BadRequest, result.Result, _ms.MessageService(result.Message));
            }
            else
            {
                return new ApiResponse<bool>(ResponseStatusEnum.Success, result.Result, _ms.MessageService(result.Message));
            }
        }

        public async Task<ApiResponse<ReturningReasonDto>> ReturningReasonEdit(ReturningReasonDto ReturningReason)
        {
            var exist = await this.ReturningReasonExist(ReturningReason.ReasonId);
            if (exist.Status == (int)ResponseStatusEnum.NotFound)
            {
                return new ApiResponse<ReturningReasonDto>((ResponseStatusEnum)exist.Status, null, _ms.MessageService(exist.Message));
            }
            var mapReturningReason = _mapper.Map<TReturningReason>(ReturningReason);
            var editedReturningReason = await _returningReasonRepository.ReturningReasonEdit(mapReturningReason);
            if (editedReturningReason == null)
            {
                return new ApiResponse<ReturningReasonDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.ReturningReasonEditing));
            }
            var mapEditedReturningReason = _mapper.Map<ReturningReasonDto>(editedReturningReason);
            return new ApiResponse<ReturningReasonDto>(ResponseStatusEnum.Success, mapEditedReturningReason, _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<bool>> ReturningReasonExist(int id)
        {
            var result = await _returningReasonRepository.ReturningReasonExist(id);
            if (result == false)
            {
                return new ApiResponse<bool>(ResponseStatusEnum.NotFound, result, _ms.MessageService(Message.ReturningReasonNotFoundById));
            }
            else
            {
                return new ApiResponse<bool>(ResponseStatusEnum.Success, result, _ms.MessageService(Message.Successfull));
            }
        }

        public async Task<ApiResponse<Pagination<ReturningReasonDto>>> ReturningReasonGetAll(PaginationDto pagination)
        {
            var data = await _returningReasonRepository.ReturningReasonGetAll(pagination);
            if (data == null)
            {
                return new ApiResponse<Pagination<ReturningReasonDto>>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.ReturningReasonGetting));
            }
            var count = await _returningReasonRepository.ReturningReasonGetAllCount(pagination);
            return new ApiResponse<Pagination<ReturningReasonDto>>(ResponseStatusEnum.Success, new Pagination<ReturningReasonDto>(count, data), _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<List<ReturningReasonDto>>> GetReturningReason()
        {
            var data = await _returningReasonRepository.ReturningReasonGetAllForm();
            if (data == null)
            {
                return new ApiResponse<List<ReturningReasonDto>>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.ReturningReasonGetting));
            }
            return new ApiResponse<List<ReturningReasonDto>>(ResponseStatusEnum.Success, data, _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<bool>> ChangeAccept(AcceptDto accept)
        {
            var data = await _returningReasonRepository.ChangeAccept(accept);
            if (data == false)
            {
                return new ApiResponse<bool>(ResponseStatusEnum.BadRequest, false, _ms.MessageService(Message.ReturningReasonChangeStatus));
            }
            return new ApiResponse<bool>(ResponseStatusEnum.Success, true, _ms.MessageService(Message.Successfull));
        }
    }
}