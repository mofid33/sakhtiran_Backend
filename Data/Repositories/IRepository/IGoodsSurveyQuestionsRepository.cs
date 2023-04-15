using System.Collections.Generic;
using System.Threading.Tasks;
using MarketPlace.API.Data.Dtos;
using MarketPlace.API.Data.Models;
using MarketPlace.API.Helper;

namespace MarketPlace.API.Data.Repositories.IRepository
{
    public interface IGoodsSurveyQuestionsRepository
    {
        Task<TGoodsSurveyQuestions> GoodsSurveyQuestionsAdd(TGoodsSurveyQuestions GoodsSurveyQuestions);
        Task<TGoodsSurveyQuestions> GoodsSurveyQuestionsEdit(TGoodsSurveyQuestions GoodsSurveyQuestions);
        Task<RepRes<TGoodsSurveyQuestions>> GoodsSurveyQuestionsDelete(int id);
        Task<bool> GoodsSurveyQuestionsExist(int id);
        Task<List<GoodsSurveyQuestionsDto>> GoodsSurveyQuestionsGetAll(int categoryId);
    }
}