using System.Collections.Generic;
using System.Threading.Tasks;
using MarketPlace.API.Data.Dtos;
using MarketPlace.API.Helper;

namespace MarketPlace.API.Services.IService
{
    public interface IGoodsSurveyQuestionsService
    {
        Task<ApiResponse<GoodsSurveyQuestionsDto>> GoodsSurveyQuestionsAdd(GoodsSurveyQuestionsDto GoodsSurveyQuestions);
        Task<ApiResponse<GoodsSurveyQuestionsDto>> GoodsSurveyQuestionsEdit(GoodsSurveyQuestionsDto GoodsSurveyQuestions);
        Task<ApiResponse<bool>> GoodsSurveyQuestionsDelete(int id);
        Task<ApiResponse<bool>> GoodsSurveyQuestionsExist(int id);
        Task<ApiResponse<List<GoodsSurveyQuestionsDto>>> GoodsSurveyQuestionsGetAll(int categoryId);
    }
}