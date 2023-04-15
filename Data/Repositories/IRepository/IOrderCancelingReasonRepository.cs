using System.Collections.Generic;
using System.Threading.Tasks;
using MarketPlace.API.Data.Dtos.Accept;
using MarketPlace.API.Data.Dtos.OrderCancelingReason;
using MarketPlace.API.Data.Dtos.Pagination;
using MarketPlace.API.Data.Models;
using MarketPlace.API.Helper;

namespace MarketPlace.API.Data.Repositories.IRepository
{
    public interface IOrderCancelingReasonRepository
    {
        Task<TOrderCancelingReason> OrderCancelingReasonAdd(TOrderCancelingReason OrderCancelingReason);
        Task<TOrderCancelingReason> OrderCancelingReasonEdit(TOrderCancelingReason OrderCancelingReason);
        Task<RepRes<TOrderCancelingReason>> OrderCancelingReasonDelete(int id);
        Task<bool> OrderCancelingReasonExist(int id);
        Task<List<OrderCancelingReasonDto>> OrderCancelingReasonGetAll(PaginationDto pagination);
        Task<List<OrderCancelingReasonDto>> OrderCancelingReasonGetAllForm();
        Task<int> OrderCancelingReasonGetAllCount(PaginationDto pagination);
        Task<bool> ChangeAccept(AcceptDto accept);
    }
}