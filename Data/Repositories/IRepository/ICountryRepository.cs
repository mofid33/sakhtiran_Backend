using System.Collections.Generic;
using System.Threading.Tasks;
using MarketPlace.API.Data.Dtos.Accept;
using MarketPlace.API.Data.Dtos.Country;
using MarketPlace.API.Data.Dtos.Pagination;
using MarketPlace.API.Data.Models;
using MarketPlace.API.Helper;

namespace MarketPlace.API.Data.Repositories.IRepository
{
    public interface ICountryRepository
    {
        Task<TCountry> CountryAdd(TCountry Country);
        Task<TCountry> CountryEdit(TCountry Country);
        Task<RepRes<TCountry>> CountryDelete(int id);
        Task<bool> CountryExist(int id);
        Task<List<CountryDto>> CountryGetAll(PaginationDto pagination);
        Task<int> CountryGetAllCount(PaginationDto pagination);
        // Task<List<WebsiteCountryDto>> GetCountryForWebsite(List<int> catIds);
        Task<CountryDto> GetCountryById(int CountryId);
        Task<bool> ChangeAccept(List<AcceptDto> accept);
    }
}