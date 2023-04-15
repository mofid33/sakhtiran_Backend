using System.Collections.Generic;
using System.Threading.Tasks;
using MarketPlace.API.Data.Dtos.Accept;
using MarketPlace.API.Data.Dtos.City;
using MarketPlace.API.Data.Dtos.Pagination;
using MarketPlace.API.Data.Models;
using MarketPlace.API.Helper;

namespace MarketPlace.API.Data.Repositories.IRepository
{
    public interface ICityRepository
    {
        Task<TCity> CityAdd(TCity City);
        Task<TCity> CityEdit(TCity City);
        Task<RepRes<TCity>> CityDelete(int id);
        Task<bool> CityExist(int id);
        Task<List<CityGetDto>> CityGetAll(PaginationDto pagination);
        Task<int> CityGetAllCount(PaginationDto pagination);
        // Task<List<WebsiteCityDto>> GetCityForWebsite(List<int> catIds);
        Task<CityGetDto> GetCityById(int CityId);
        Task<bool> ChangeAccept(List<AcceptDto> accept);

        Task<bool> ShippingMethodAreaCodeExist(long areaId , int areaCode , int shippingMethodId , int cityId);
        Task<TShippingMethodAreaCode> AddShippingMethodAreaCode(TShippingMethodAreaCode ShippingMethodAreaCode);
        Task<TShippingMethodAreaCode> UpdateShippingMethodAreaCode(TShippingMethodAreaCode ShippingMethodAreaCode);
        Task<RepRes<TShippingMethodAreaCode>> DeleteShippingMethodAreaCode(int id);
        Task<List<CityShippingMethodAreaCodeDto>> ShippingMethodGetAll(PaginationDto pagination);
        Task<int> ShippingMethodGetAllCount(PaginationDto pagination);
    }
}