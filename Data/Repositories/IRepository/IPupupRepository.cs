using System.Collections.Generic;
using System.Threading.Tasks;
using MarketPlace.API.Data.Dtos.PupupItem;
using MarketPlace.API.Data.Dtos.Pagination;
using MarketPlace.API.Data.Models;
using MarketPlace.API.Helper;
using MarketPlace.API.Data.Dtos.Accept;

namespace MarketPlace.API.Data.Repositories.IRepository
{
    public interface IPupupRepository
    {
        Task<TPopupItem> PupupItemAdd(TPopupItem pupupItem);
        Task<TPopupItem> PupupItemEdit(TPopupItem pupupItem);
        Task<RepRes<TPopupItem>> PupupItemDelete(int id);
        Task<bool> PupupItemExist(int id);
        Task<List<PupupItemDto>> PupupItemGetAll(PaginationDto pagination);
        Task<int> PupupItemGetAllCount(PaginationDto pagination);
        Task<PupupItemDto> GetPupupItemById(int pupupItemId);
        Task<PupupItemDto> GetWebsitePupup();
        Task<bool> ChangeAccept(AcceptDto  accept);

    }
}