using System.Collections.Generic;
using System.Threading.Tasks;
using MarketPlace.API.Data.Dtos.Accept;
using MarketPlace.API.Data.Dtos.Brand;
using MarketPlace.API.Data.Dtos.Pagination;
using MarketPlace.API.Data.Models;
using MarketPlace.API.Helper;

namespace MarketPlace.API.Data.Repositories.IRepository
{
    public interface IBrandRepository
    {
        Task<TBrand> BrandAdd(TBrand brand);
        Task<TBrand> BrandEdit(TBrand brand);
        Task<bool> ChangeAccept(List<AcceptNullDto> accept);
        Task<RepRes<TBrand>> BrandDelete(int id);
        Task<bool> BrandExist(int id);
        Task<bool> BrandExistWithTitle(string title);
        Task<List<BrandDto>> BrandGetAll(PaginationDto pagination);
        Task<int> BrandGetAllCount(PaginationDto pagination);
        Task<List<WebsiteBrandDto>> GetBrandForWebsite(List<int> catIds);
        Task<List<WebsiteBrandDto>> GetBrandForWebsiteWithFillter(PaginationBrandDto pagination , List<int> catIds);
        Task<BrandGetOneDto> GetBrandById(int brandId);
        Task<bool> AcceptShopBrandAdding();
    }
}