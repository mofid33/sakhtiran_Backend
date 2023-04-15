using System.Threading.Tasks;
using MarketPlace.API.Data.Dtos.Brand;
using MarketPlace.API.Data.Dtos.Pagination;
using MarketPlace.API.Data.Dtos.Accept;
using MarketPlace.API.Helper;
using System.Collections.Generic;

namespace MarketPlace.API.Services.IService
{
    public interface IBrandService
    {
        Task<ApiResponse<BrandDto>> BrandAdd(BrandSerializeDto brandDto);
        Task<ApiResponse<BrandDto>> BrandEdit(BrandSerializeDto brandDto);
        Task<ApiResponse<bool>> BrandDelete(int id);
        Task<ApiResponse<bool>> ChangeAccept(List<AcceptNullDto>  accept);
        Task<ApiResponse<bool>> BrandExist(int id);
        Task<ApiResponse<Pagination<BrandDto>>> BrandGetAll(PaginationDto pagination);
        Task<ApiResponse<BrandGetOneDto>> GetBrandById(int brandId);
    }
}