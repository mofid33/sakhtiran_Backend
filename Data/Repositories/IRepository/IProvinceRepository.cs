using System.Collections.Generic;
using System.Threading.Tasks;
using MarketPlace.API.Data.Dtos.Accept;
using MarketPlace.API.Data.Dtos.Province;
using MarketPlace.API.Data.Dtos.Pagination;
using MarketPlace.API.Data.Models;
using MarketPlace.API.Helper;

namespace MarketPlace.API.Data.Repositories.IRepository
{
    public interface IProvinceRepository
    {
        Task<TProvince> ProvinceAdd(TProvince Province);
        Task<TProvince> ProvinceEdit(TProvince Province);
        Task<RepRes<TProvince>> ProvinceDelete(int id);
        Task<bool> ProvinceExist(int id);
        Task<List<ProvinceGetDto>> ProvinceGetAll(PaginationDto pagination);
        Task<int> ProvinceGetAllCount(PaginationDto pagination);
        // Task<List<WebsiteProvinceDto>> GetProvinceForWebsite(List<int> catIds);
        Task<ProvinceGetDto> GetProvinceById(int ProvinceId);
        Task<bool> ChangeAccept(List<AcceptDto> accept);
    }
}