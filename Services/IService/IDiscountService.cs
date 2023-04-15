using System.Collections.Generic;
using System.Threading.Tasks;
using MarketPlace.API.Data.Dtos.Accept;
using MarketPlace.API.Data.Dtos.Discount;
using MarketPlace.API.Data.Dtos.Pagination;
using MarketPlace.API.Helper;

namespace MarketPlace.API.Services.IService
{
    public interface IDiscountService
    {
        Task<ApiResponse<DiscountPlanAddDto>> DiscountPlanAdd(DiscountPlanAddDto discountDto);
        Task<ApiResponse<DiscountPlanEditDto>> DiscountPlanEdit(DiscountPlanEditDto discountDto);
        Task<ApiResponse<List<DiscountCodeExelDto>>> GetCoponCodeForExel(int planId);
        Task<ApiResponse<Pagination<DiscountPlanGetDto>>> DiscountPlanGet(DiscountFilterDto filterDto);
        Task<ApiResponse<Pagination<DiscountCodeDetailDto>>> GetDiscountCodeDetail(DiscountCodePaginationDto pagination);
        Task<ApiResponse<bool>> ChangeAccept(AcceptDto accept);
        Task<ApiResponse<bool>> DeleteDiscount(long planId);
        Task<ApiResponse<DiscountPlanGetOneDto>> GetOne(int planId);

    }
}