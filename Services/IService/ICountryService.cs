using System.Collections.Generic;
using System.Threading.Tasks;
using MarketPlace.API.Data.Dtos.Accept;
using MarketPlace.API.Data.Dtos.Country;
using MarketPlace.API.Data.Dtos.Pagination;
using MarketPlace.API.Helper;

namespace MarketPlace.API.Services.IService
{
    public interface ICountryService
    {
        Task<ApiResponse<CountryDto>> CountryAdd(CountrySerializeDto CountryDto);
        Task<ApiResponse<CountryDto>> CountryEdit(CountrySerializeDto CountryDto);
        Task<ApiResponse<bool>> CountryDelete(int id);
        Task<ApiResponse<bool>> CountryExist(int id);
        Task<ApiResponse<Pagination<CountryDto>>> CountryGetAll(PaginationDto pagination);
        Task<ApiResponse<CountryDto>> GetCountryById(int CountryId);
        Task<ApiResponse<bool>> ChangeAccept(List<AcceptDto> accept);
    }
}