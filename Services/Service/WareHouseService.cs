using System.Threading.Tasks;
using AutoMapper;
using MarketPlace.API.Data.Dtos.Header;
using MarketPlace.API.Data.Dtos.Pagination;
using MarketPlace.API.Data.Dtos.Token;
using MarketPlace.API.Data.Dtos.WareHouse;
using MarketPlace.API.Data.Enums;
using MarketPlace.API.Data.Repositories.IRepository;
using MarketPlace.API.Helper;
using MarketPlace.API.Services.IService;

namespace MarketPlace.API.Services.Service
{
    public class WareHouseService : IWareHouseService
    {
        public IMapper _mapper { get; }
        public IWareHouseRepository _wareHouseRepository { get; }
        public HeaderParseDto header { get; set; }
        public TokenParseDto token { get; set; }
        public IMessageLanguageService _ms { get; set; }


        public WareHouseService(
        IMapper mapper,
        IWareHouseRepository wareHouseRepository,
        IMessageLanguageService ms,
        Microsoft.AspNetCore.Http.IHttpContextAccessor httpContextAccessor)
        {
            header = new HeaderParseDto(httpContextAccessor);
            token = new TokenParseDto(httpContextAccessor);
            this._wareHouseRepository = wareHouseRepository;
            _ms = ms;
            this._mapper = mapper;
        }

        public async Task<ApiResponse<Pagination<WareHouseOprationListDto>>> GetWareHouseOprationList(WareHouseOprationListPaginationDto pagination)
        {
            if(token.Rule == UserGroupEnum.Seller)
            {
                pagination.ShopId = token.Id;
            }
            var data = await _wareHouseRepository.GetWareHouseOprationList(pagination);
            if(data == null)
            {
                return new ApiResponse<Pagination<WareHouseOprationListDto>>(ResponseStatusEnum.BadRequest,null,_ms.MessageService(Message.WareHouseOprationGetting));
            }
            var count = await _wareHouseRepository.GetWareHouseOprationListCount(pagination);
            return new ApiResponse<Pagination<WareHouseOprationListDto>>(ResponseStatusEnum.Success,new Pagination<WareHouseOprationListDto>(count,data),_ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<Pagination<WareHouseOprationDetailDto>>> GetWareHouseOperationDetail(PaginationFormDto pagination)
        {
            var data = await _wareHouseRepository.GetWareHouseOperationDetail(pagination);
            if(data == null)
            {
                return new ApiResponse<Pagination<WareHouseOprationDetailDto>>(ResponseStatusEnum.BadRequest,null,_ms.MessageService(Message.WareHouseOprationGetting));
            }
            var count = await _wareHouseRepository.GetWareHouseOperationDetailCount(pagination);
            return new ApiResponse<Pagination<WareHouseOprationDetailDto>>(ResponseStatusEnum.Success,new Pagination<WareHouseOprationDetailDto>(count,data),_ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<bool>> AddWareHouseOperation(WareHouseOprationAddDto opration)
        {
            var data = await _wareHouseRepository.AddWareHouseOperation(opration);
            if(data.Result == false)
            {
                return new ApiResponse<bool>(ResponseStatusEnum.BadRequest,false,_ms.MessageService(data.Message));
            }
            return new ApiResponse<bool>(ResponseStatusEnum.Success,true,_ms.MessageService(data.Message));
        }
    }
}