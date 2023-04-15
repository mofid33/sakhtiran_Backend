using System.Collections.Generic;
using System.Threading.Tasks;
using MarketPlace.API.Data.Dtos.OrderReturning;
using MarketPlace.API.Data.Dtos.Pagination;
using MarketPlace.API.Data.Dtos.UserOrder;
using MarketPlace.API.Helper;

namespace MarketPlace.API.Data.Repositories.IRepository
{
    public interface IOrderReturnRepository
    {

        Task<List<OrderReturningListDto>> GetOrderReturningList(OrderReturningPaginationDto pagination);
        Task<int> GetOrderReturningListCount(OrderReturningPaginationDto pagination);
        Task<List<OrderReturningLogDto>> GetOrderReturningLog(int returnId);
        Task<OrderReturningDetailDto> GetOrderReturningDetail(int returnId);
        Task<RepRes<bool>> EditOrderReturning(OrderReturningChangeDto orderReturning);
        Task<RepRes<bool>> BlockAmountOrderReturning(AmountDto amount);
        Task<RepRes<bool>> RefoundAmountOrderReturning(AmountDto amount);
        Task<AmountDto> GetOrderReturningAmount(int returnId);
        Task<bool> SendEmailAndSMS(int returnId , string msg);

    }
}