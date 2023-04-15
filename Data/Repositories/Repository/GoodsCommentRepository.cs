using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MarketPlace.API.Data.Models;
using MarketPlace.API.Data.Repositories.IRepository;
using System.Data;
using MarketPlace.API.Data.Dtos.Pagination;
using MarketPlace.API.Data.Dtos.Survey;

namespace MarketPlace.API.Data.Repositories.Repository
{
    public class GoodsCommentRepository : IGoodsCommentRepository
    {
        public MarketPlaceDbContext _context { get; }

        public GoodsCommentRepository(MarketPlaceDbContext context)
        {
            this._context = context;
        }


        /// کامنت های کاربران همراه با نظرسنجی برای وب سایت
        public async Task<GoodsCommentWithSurveyDto> CustomerCommentGet(PaginationFormDto paginatin)
        {
            var Comments = new GoodsCommentWithSurveyDto();
            try
            {
                Comments.GoodsComment = await _context.TGoodsComment
                .Include(b=>b.TGoodsCommentPoints)
                .Include(b=>b.FkCustomer)
                .OrderByDescending(x => x.CommentId)
                .Where(c => c.FkGoodsId == paginatin.Id && c.IsAccepted == true)
                .Skip(paginatin.PageSize * (paginatin.PageNumber - 1)).Take(paginatin.PageSize)
                .Select(x => new GoodsCommentAddDto()
                {
                    CommentText = x.CommentText,
                    IsAccepted = x.IsAccepted,
                   ReviewPoint =  x.ReviewPoint ,
                    CustomerName = x.FkCustomer.Name + " " + x.FkCustomer.Family,
                    TGoodsCommentPoints = x.TGoodsCommentPoints.Select(t => new TGoodsCommentPoints()
                    {
                        PointId = t.PointId,
                        PointText = t.PointText,
                        PointType = t.PointType
                    }).ToList()
                }).AsNoTracking().ToListAsync();
                
                Comments.SurveyList = await _context.TGoodsComment
                .Where(c => c.FkGoodsId == paginatin.Id && c.IsAccepted == true)
                .GroupBy(g => g.ReviewPoint)
                .Select(g => new SurveyGetDto()
                {
                    value = g.Key,
                    Average = g.Average(x=>x.ReviewPoint)
                }).ToListAsync();

               if(Comments.SurveyList.Count > 0) {
                Comments.AllSurveyAverage =  await _context.TGoodsComment
                .Where(c => c.FkGoodsId == paginatin.Id && c.IsAccepted == true)
                .AverageAsync(x => x.ReviewPoint);
               }

                foreach (var item in Comments.SurveyList)
                {
                    item.Average = (item.Average * 100) / 5 ;
                }
                return Comments;
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<int> CustomerCommentGetCount(PaginationFormDto paginatin)
        {
            return await _context.TGoodsComment.CountAsync(g => g.FkGoodsId == paginatin.Id && g.IsAccepted == true);
        }

        public async Task<bool> ChangeIsAccept(int commentId, bool? isAccepted)
        {
            var goodsComment = await _context.TGoodsComment.FindAsync(commentId);
            if (goodsComment != null) {
                goodsComment.IsAccepted = isAccepted;
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }
    }
}