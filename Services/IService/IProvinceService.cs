using System.Collections.Generic;
using System.Threading.Tasks;
using MarketPlace.API.Data.Dtos.Accept;
using MarketPlace.API.Data.Dtos.Province;
using MarketPlace.API.Data.Dtos.Pagination;
using MarketPlace.API.Helper;

namespace MarketPlace.API.Services.IService
{
    public interface IProvinceService
    {
        Task<ApiResponse<ProvinceDto>> ProvinceAdd(ProvinceDto ProvinceDto);
        Task<ApiResponse<ProvinceDto>> ProvinceEdit(ProvinceDto ProvinceDto);
        Task<ApiResponse<bool>> ProvinceDelete(int id);
        Task<ApiResponse<bool>> ProvinceExist(int id);
        Task<ApiResponse<Pagination<ProvinceGetDto>>> ProvinceGetAll(PaginationDto pagination);
        Task<ApiResponse<ProvinceGetDto>> GetProvinceById(int ProvinceId);
        Task<ApiResponse<bool>> ChangeAccept(List<AcceptDto> accept);
    }
}