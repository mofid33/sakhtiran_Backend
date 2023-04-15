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
using MarketPlace.API.Data.Dtos.Accept;
using MarketPlace.API.Data.Dtos.Setting;

namespace MarketPlace.API.Data.Repositories.Repository
{
    public class ShopSurveyQuestionsRepository : IShopSurveyQuestionsRepository
    {
        public MarketPlaceDbContext _context { get; }
        public HeaderParseDto header { get; set; }


        public ShopSurveyQuestionsRepository(MarketPlaceDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            header = new HeaderParseDto(httpContextAccessor);
            this._context = context;
        }
        public async Task<TShopSurveyQuestions> ShopSurveyQuestionsAdd(TShopSurveyQuestions ShopSurveyQuestions)
        {
            try
            {
                ShopSurveyQuestions.QuestionText = JsonExtensions.JsonAdd(ShopSurveyQuestions.QuestionText, header);
                await _context.TShopSurveyQuestions.AddAsync(ShopSurveyQuestions);
                await _context.SaveChangesAsync();
                ShopSurveyQuestions.QuestionText = JsonExtensions.JsonGet(ShopSurveyQuestions.QuestionText, header);
                return ShopSurveyQuestions;
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<RepRes<TShopSurveyQuestions>> ShopSurveyQuestionsDelete(int id)
        {
            try
            {
                var existInAnswers = await _context.TShopSurveyAnswers.AsNoTracking().AnyAsync(x => x.FkQuestionId == id);
                if (existInAnswers)
                {
                    return new RepRes<TShopSurveyQuestions>(Message.ShopSurveyQuestionsCantDelete, false, null);
                }
                var data = await _context.TShopSurveyQuestions.FindAsync(id);
                _context.TShopSurveyQuestions.Remove(data);
                await _context.SaveChangesAsync();
                return new RepRes<TShopSurveyQuestions>(Message.Successfull, true, null);
            }
            catch (System.Exception)
            {
                return new RepRes<TShopSurveyQuestions>(Message.ShopSurveyQuestionsDelete, false, null);
            }
        }

        public async Task<TShopSurveyQuestions> ShopSurveyQuestionsEdit(TShopSurveyQuestions ShopSurveyQuestions)
        {
            try
            {
                var data = await _context.TShopSurveyQuestions.FindAsync(ShopSurveyQuestions.QueId);
                ShopSurveyQuestions.QuestionText = JsonExtensions.JsonEdit(ShopSurveyQuestions.QuestionText, data.QuestionText, header);
                data.QuestionText = ShopSurveyQuestions.QuestionText;
                await _context.SaveChangesAsync();
                ShopSurveyQuestions.QuestionText = JsonExtensions.JsonGet(ShopSurveyQuestions.QuestionText, header);
                return ShopSurveyQuestions;
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<bool> ShopSurveyQuestionsExist(int id)
        {
            try
            {
                var result = await _context.TShopSurveyQuestions.AsNoTracking().AnyAsync(x => x.QueId == id);
                return result;
            }
            catch (System.Exception)
            {
                return false;
            }
        }

        public async Task<List<ShopSurveyQuestionsDto>> ShopSurveyQuestionsGetAll(bool getActive = false)
        {
            try
            {
                var data = await _context.TShopSurveyQuestions
                .Where(c=> getActive == true ?  c.Status == true : true)
                .Select(x => new ShopSurveyQuestionsDto()
                {
                    Status = x.Status,
                    QueId = x.QueId,
                    QuestionText = JsonExtensions.JsonValue(x.QuestionText, header.Language)
                })
                .AsNoTracking().ToListAsync();
                return data;
            }
            catch (System.Exception)
            {
                return null;
            }
        }


        public async Task<bool> ChangeAccept(AcceptDto accept)
        {
            try
            {
                var data = await _context.TShopSurveyQuestions.FindAsync(accept.Id);
                data.Status = accept.Accept;
                await _context.SaveChangesAsync();
                return true;
            }
            catch (System.Exception)
            {
                return false;
            }
        }




        public async Task<SettingDescriptionDto> EditShopCalculateComment(SettingDescriptionDto settingDto)
        {
            try
            {
                var data = await _context.TSetting.FirstOrDefaultAsync();
                data.ShopCalculateComment = JsonExtensions.JsonEdit(settingDto.Description, data.ShopCalculateComment, header);

                await _context.SaveChangesAsync();
                return settingDto;

            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<SettingDescriptionDto> GetShopCalculateComment()
        {
            try
            {
                return await _context.TSetting.Select(x => new SettingDescriptionDto()
                {
                    Description = JsonExtensions.JsonValue(x.ShopCalculateComment, header.Language)
                }).AsNoTracking().FirstOrDefaultAsync();
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<ShopSurveyQuestionsDto> ShopSurveyQuestionsGetOne(int id)
        {
            try
            {
                var data = await _context.TShopSurveyQuestions.Where(c => c.QueId == id)
                .Select(x => new ShopSurveyQuestionsDto()
                {
                    Status = x.Status,
                    QueId = x.QueId,
                    QuestionText = JsonExtensions.JsonValue(x.QuestionText, header.Language)
                })
                .AsNoTracking().FirstOrDefaultAsync();
                return data;
            }
            catch (System.Exception)
            {
                return null;
            }

        }
    }
}