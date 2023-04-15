using System.Threading.Tasks;
using MarketPlace.API.Data.Dtos.Accept;
using MarketPlace.API.Data.Dtos.Guarantee;
using MarketPlace.API.Data.Dtos.Pagination;
using MarketPlace.API.Helper;

namespace MarketPlace.API.Services.IService
{
    public interface IGuaranteeService
    {
        Task<ApiResponse<GuaranteeDto>> GuaranteeAdd(GuaranteeDto Guarantee);
        Task<ApiResponse<GuaranteeDto>> GuaranteeEdit(GuaranteeDto Guarantee);
        Task<ApiResponse<bool>> GuaranteeDelete(int id);
        Task<ApiResponse<bool>> GuaranteeExist(int id);
        Task<ApiResponse<Pagination<GuaranteeDto>>> GuaranteeGetAll(PaginationDto pagination);
        Task<ApiResponse<GuaranteeGetOneDto>> GetGuaranteeById(int guaranteeId);
        Task<ApiResponse<bool>> ChangeAccept(AcceptNullDto accept);
    }
}