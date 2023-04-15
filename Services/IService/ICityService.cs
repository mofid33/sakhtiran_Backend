using System.Collections.Generic;
using System.Threading.Tasks;
using MarketPlace.API.Data.Dtos.Accept;
using MarketPlace.API.Data.Dtos.City;
using MarketPlace.API.Data.Dtos.Pagination;
using MarketPlace.API.Helper;

namespace MarketPlace.API.Services.IService
{
    public interface ICityService
    {
        Task<ApiResponse<CityDto>> CityAdd(CityDto cityDto);
        Task<ApiResponse<CityDto>> CityEdit(CityDto cityDto);
        Task<ApiResponse<bool>> CityDelete(int id);
        Task<ApiResponse<bool>> CityExist(int id);
        Task<ApiResponse<Pagination<CityGetDto>>> CityGetAll(PaginationDto pagination);
        Task<ApiResponse<CityGetDto>> GetCityById(int cityId);
        Task<ApiResponse<bool>> ChangeAccept(List<AcceptDto> accept);

        Task<ApiResponse<CityShippingMethodAreaCodeDto>> AddShippingMethodAreaCode(CityShippingMethodAreaCodeDto ShippingMethodAreaCode);
        Task<ApiResponse<CityShippingMethodAreaCodeDto>> UpdateShippingMethodAreaCode(CityShippingMethodAreaCodeDto ShippingMethodAreaCode);
        Task<ApiResponse<bool>> DeleteShippingMethodAreaCode(int id);
        Task<ApiResponse<Pagination<CityShippingMethodAreaCodeDto>>> ShippingMethodAreaGetAll(PaginationDto pagination);


    }
}