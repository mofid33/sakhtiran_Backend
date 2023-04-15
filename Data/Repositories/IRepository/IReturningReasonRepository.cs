using System.Collections.Generic;
using System.Threading.Tasks;
using MarketPlace.API.Data.Dtos.ReturningReason;
using MarketPlace.API.Data.Dtos.Pagination;
using MarketPlace.API.Data.Models;
using MarketPlace.API.Helper;
using MarketPlace.API.Data.Dtos.Accept;

namespace MarketPlace.API.Data.Repositories.IRepository
{
    public interface IReturningReasonRepository
    {
        Task<TReturningReason> ReturningReasonAdd(TReturningReason ReturningReason);
        Task<TReturningReason> ReturningReasonEdit(TReturningReason ReturningReason);
        Task<RepRes<TReturningReason>> ReturningReasonDelete(int id);
        Task<bool> ReturningReasonExist(int id);
        Task<List<ReturningReasonDto>> ReturningReasonGetAll(PaginationDto pagination);
        Task<List<ReturningReasonDto>> ReturningReasonGetAllForm();
        Task<int> ReturningReasonGetAllCount(PaginationDto pagination);
        Task<bool> ChangeAccept(AcceptDto accept);
    }
}