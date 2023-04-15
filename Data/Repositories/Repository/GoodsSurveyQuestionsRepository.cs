using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MarketPlace.API.Data.Models;
using MarketPlace.API.Data.Repositories.IRepository;
using AutoMapper;
using MarketPlace.API.Data.Dtos;
using MarketPlace.API.Helper;
using AutoMapper.QueryableExtensions;
using MarketPlace.API.Data.Dtos.Header;
using Microsoft.AspNetCore.Http;

namespace MarketPlace.API.Data.Repositories.Repository
{
    public class GoodsSurveyQuestionsRepository : IGoodsSurveyQuestionsRepository
    {
        public MarketPlaceDbContext _context { get; }
        public HeaderParseDto header { get; set; }


        public GoodsSurveyQuestionsRepository(MarketPlaceDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            header = new HeaderParseDto(httpContextAccessor);
            this._context = context;
        }
        public async Task<TGoodsSurveyQuestions> GoodsSurveyQuestionsAdd(TGoodsSurveyQuestions GoodsSurveyQuestions)
        {
            try
            {
                GoodsSurveyQuestions.QuestionText = JsonExtensions.JsonAdd(GoodsSurveyQuestions.QuestionText,header);
                await _context.TGoodsSurveyQuestions.AddAsync(GoodsSurveyQuestions);
                await _context.SaveChangesAsync();
                GoodsSurveyQuestions.QuestionText = JsonExtensions.JsonGet(GoodsSurveyQuestions.QuestionText,header);
                return GoodsSurveyQuestions;
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<RepRes<TGoodsSurveyQuestions>> GoodsSurveyQuestionsDelete(int id)
        {
            try
            {
                var existInAnswers = await _context.TGoodsSurveyAnswers.AsNoTracking().AnyAsync(x=>x.FkQuestionId == id);
                if(existInAnswers)
                {
                    return new RepRes<TGoodsSurveyQuestions>(Message.GoodsSurveyQuestionsCantDelete,false,null);
                }
                var data = await _context.TGoodsSurveyQuestions.FindAsync(id);
                _context.TGoodsSurveyQuestions.Remove(data);
                await _context.SaveChangesAsync();
                return new RepRes<TGoodsSurveyQuestions>(Message.Successfull,true,null);
            }
            catch (System.Exception)
            {
                return new RepRes<TGoodsSurveyQuestions>(Message.GoodsSurveyQuestionsDelete,false,null);
            }
        }

        public async Task<TGoodsSurveyQuestions> GoodsSurveyQuestionsEdit(TGoodsSurveyQuestions GoodsSurveyQuestions)
        {
            try
            {
                var data = await _context.TGoodsSurveyQuestions.FindAsync(GoodsSurveyQuestions.QueId);
                GoodsSurveyQuestions.QuestionText = JsonExtensions.JsonEdit(GoodsSurveyQuestions.QuestionText,data.QuestionText,header);
                data.QuestionText = GoodsSurveyQuestions.QuestionText;
                await _context.SaveChangesAsync();
                GoodsSurveyQuestions.QuestionText = JsonExtensions.JsonGet(GoodsSurveyQuestions.QuestionText,header);
                return GoodsSurveyQuestions;
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<bool> GoodsSurveyQuestionsExist(int id)
        {
            try
            {
                var result = await _context.TGoodsSurveyQuestions.AsNoTracking().AnyAsync(x => x.QueId == id);
                return result;
            }
            catch (System.Exception)
            {
                return false;
            }
        }

        public async Task<List<GoodsSurveyQuestionsDto>> GoodsSurveyQuestionsGetAll(int categoryId)
        {
            try
            {
                var data = await _context.TGoodsSurveyQuestions.Where(x => x.FkCategoryId == categoryId)
                .Select(x=>new GoodsSurveyQuestionsDto(){
                    FkCategoryId = x.FkCategoryId,
                    QueId = x.QueId,
                    QuestionText = JsonExtensions.JsonValue(x.QuestionText,header.Language)
                })
                .AsNoTracking().ToListAsync();
                return data;
            }
            catch (System.Exception)
            {
                return null;
            }
        }
    }
}