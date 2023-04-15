using System.Threading.Tasks;
using MarketPlace.API.Data.Dtos.Guarantee;
using MarketPlace.API.Data.Dtos.Pagination;
using MarketPlace.API.Data.Enums;
using MarketPlace.API.Data.Models;
using MarketPlace.API.Data.Repositories.IRepository;
using MarketPlace.API.Helper;
using MarketPlace.API.Services.IService;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using MarketPlace.API.Data.Dtos.Header;
using MarketPlace.API.Data.Dtos.Token;
using System.Collections.Generic;
using MarketPlace.API.Data.Dtos.Accept;
using System.Linq;

namespace MarketPlace.API.Services.Service
{
    public class GuaranteeService : IGuaranteeService
    {
        public IMapper _mapper { get; }
        public IGuaranteeRepository _guaranteeRepository { get; }
        public HeaderParseDto header { get; set; }
        public TokenParseDto token { get; set; }
        public IMessageLanguageService _ms { get; set; }
        public ICategoryRepository _categoryRepository { get; set; }

        public GuaranteeService(IMapper mapper, 
         ICategoryRepository categoryRepository,
        IGuaranteeRepository guaranteeRepository, 
        IMessageLanguageService ms,
        IHttpContextAccessor httpContextAccessor)
        {
            header = new HeaderParseDto(httpContextAccessor);
            token = new TokenParseDto(httpContextAccessor);
            this._guaranteeRepository = guaranteeRepository;
            this._mapper = mapper;
            _ms = ms;
            _categoryRepository = categoryRepository;
        }

        public async Task<ApiResponse<GuaranteeDto>> GuaranteeAdd(GuaranteeDto Guarantee)
        {
            var mapGuarantee = _mapper.Map<TGuarantee>(Guarantee);
            if (token.Rule == UserGroupEnum.Seller)
            {
                if (await _guaranteeRepository.AcceptShopGuaranteeAdding())
                {
                    mapGuarantee.IsAccepted = true;
                }
                else
                {
                    mapGuarantee.IsAccepted = null;
                }
            }
            var craetedGuarantee = await _guaranteeRepository.GuaranteeAdd(mapGuarantee);
            if (craetedGuarantee == null)
            {
                return new ApiResponse<GuaranteeDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.GuaranteeAdding));
            }
            var mapCraetedGuarantee = _mapper.Map<GuaranteeDto>(craetedGuarantee);
            return new ApiResponse<GuaranteeDto>(ResponseStatusEnum.Success, mapCraetedGuarantee, _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<bool>> GuaranteeDelete(int id)
        {
            var exist = await this.GuaranteeExist(id);
            if (exist.Status == (int)ResponseStatusEnum.NotFound)
            {
                return new ApiResponse<bool>((ResponseStatusEnum)exist.Status, exist.Result, _ms.MessageService(exist.Message));
            }
            var result = await _guaranteeRepository.GuaranteeDelete(id);
            if (result.Result == false)
            {
                return new ApiResponse<bool>(ResponseStatusEnum.BadRequest, result.Result, _ms.MessageService(result.Message));
            }
            else
            {
                return new ApiResponse<bool>(ResponseStatusEnum.Success, result.Result, _ms.MessageService(result.Message));
            }
        }

        public async Task<ApiResponse<GuaranteeDto>> GuaranteeEdit(GuaranteeDto Guarantee)
        {
            var exist = await this.GuaranteeExist(Guarantee.GuaranteeId);
            if (exist.Status == (int)ResponseStatusEnum.NotFound)
            {
                return new ApiResponse<GuaranteeDto>((ResponseStatusEnum)exist.Status, null, _ms.MessageService(exist.Message));
            }
            var mapGuarantee = _mapper.Map<TGuarantee>(Guarantee);
            var editedGuarantee = await _guaranteeRepository.GuaranteeEdit(mapGuarantee);
            if (editedGuarantee == null)
            {
                return new ApiResponse<GuaranteeDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.GuaranteeEditing));
            }
            var mapEditedGuarantee = _mapper.Map<GuaranteeDto>(editedGuarantee);
            return new ApiResponse<GuaranteeDto>(ResponseStatusEnum.Success, mapEditedGuarantee, _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<bool>> GuaranteeExist(int id)
        {
            var result = await _guaranteeRepository.GuaranteeExist(id);
            if (result == false)
            {
                return new ApiResponse<bool>(ResponseStatusEnum.NotFound, result, _ms.MessageService(Message.GuaranteeNotFoundById));
            }
            else
            {
                return new ApiResponse<bool>(ResponseStatusEnum.Success, result, _ms.MessageService(Message.Successfull));
            }
        }

        public async Task<ApiResponse<Pagination<GuaranteeDto>>> GuaranteeGetAll(PaginationDto pagination)
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
            var data = await _guaranteeRepository.GuaranteeGetAll(pagination);
            if (data == null)
            {
                return new ApiResponse<Pagination<GuaranteeDto>>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.GuaranteeGetting));
            }
            var count = await _guaranteeRepository.GuaranteeGetAllCount(pagination);
            return new ApiResponse<Pagination<GuaranteeDto>>(ResponseStatusEnum.Success, new Pagination<GuaranteeDto>(count, data), _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<bool>> ChangeAccept(AcceptNullDto accept)
        {
            var result = await _guaranteeRepository.ChangeAccept(accept);
            if (result == false)
            {
                return new ApiResponse<bool>(ResponseStatusEnum.BadRequest, false, _ms.MessageService(Message.GuaranteeEditing));
            }
            else
            {
                return new ApiResponse<bool>(ResponseStatusEnum.Success, true, _ms.MessageService(Message.Successfull));
            }
        }

        public async Task<ApiResponse<GuaranteeGetOneDto>> GetGuaranteeById(int guaranteeId)
        {
            var Guarantee = await _guaranteeRepository.GetGuaranteeById(guaranteeId);
            if (Guarantee == null)
            {
                return new ApiResponse<GuaranteeGetOneDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.GuaranteeGetting));
            }
            else
            {
                return new ApiResponse<GuaranteeGetOneDto>(ResponseStatusEnum.Success, Guarantee, _ms.MessageService(Message.Successfull));
            }
        }
    }
}