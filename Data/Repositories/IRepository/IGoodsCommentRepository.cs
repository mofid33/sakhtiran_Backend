using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using MarketPlace.API.Data.Dtos;
using MarketPlace.API.Data.Dtos.Pagination;
using MarketPlace.API.Data.Dtos.Survey;
using MarketPlace.API.Data.Models;

namespace MarketPlace.API.Data.Repositories.IRepository
{
    public interface IGoodsCommentRepository
    {


        // نظرات کاربران به یک کالای خاص
        Task<GoodsCommentWithSurveyDto> CustomerCommentGet(PaginationFormDto paginatin);
        Task<int> CustomerCommentGetCount(PaginationFormDto paginatin);
        Task<bool> ChangeIsAccept(int commentId, bool? isAccepted);
    }
}