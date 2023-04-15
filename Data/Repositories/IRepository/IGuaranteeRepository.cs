using System.Collections.Generic;
using System.Threading.Tasks;
using MarketPlace.API.Data.Dtos.Accept;
using MarketPlace.API.Data.Dtos.Guarantee;
using MarketPlace.API.Data.Dtos.Pagination;
using MarketPlace.API.Data.Models;
using MarketPlace.API.Helper;

namespace MarketPlace.API.Data.Repositories.IRepository
{
    public interface IGuaranteeRepository
    {
        Task<TGuarantee> GuaranteeAdd(TGuarantee Guarantee);
        Task<TGuarantee> GuaranteeEdit(TGuarantee Guarantee);
        Task<RepRes<TGuarantee>> GuaranteeDelete(int id);
        Task<bool> GuaranteeExist(int id);
        Task<List<GuaranteeDto>> GuaranteeGetAll(PaginationDto pagination);
        Task<int> GuaranteeGetAllCount(PaginationDto pagination);
        Task<List<GuaranteeFormDto>> GetGuaranteeForWebsite(List<int> catIds);
        Task<bool> ChangeAccept(AcceptNullDto accept);
        Task<GuaranteeGetOneDto> GetGuaranteeById(int guaranteeId);
        Task<bool> AcceptShopGuaranteeAdding();
    }
}