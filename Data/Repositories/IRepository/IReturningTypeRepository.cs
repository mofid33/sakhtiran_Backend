using System.Collections.Generic;
using System.Threading.Tasks;
using MarketPlace.API.Data.Dtos.ReturningType;
using MarketPlace.API.Data.Dtos.Pagination;
using MarketPlace.API.Data.Models;
using MarketPlace.API.Helper;

namespace MarketPlace.API.Data.Repositories.IRepository
{
    public interface IReturningTypeRepository
    {
        Task<TReturningAction> ReturningTypeAdd(TReturningAction ReturningType);
        Task<TReturningAction> ReturningTypeEdit(TReturningAction ReturningType);
        Task<RepRes<TReturningAction>> ReturningTypeDelete(int id);
        Task<bool> ReturningTypeExist(int id);
        Task<List<ReturningTypeDto>> ReturningTypeGetAll(PaginationDto pagination);
        Task<List<ReturningTypeDto>> ReturningTypeGetAllForm();
        Task<int> ReturningTypeGetAllCount(PaginationDto pagination);
    }
}