using System.Collections.Generic;
using System.Threading.Tasks;
using MarketPlace.API.Data.Dtos;
using MarketPlace.API.Data.Dtos.Accept;
using MarketPlace.API.Data.Dtos.Setting;
using MarketPlace.API.Helper;

namespace MarketPlace.API.Services.IService
{
    public interface IShopSurveyQuestionsService
    {
        Task<ApiResponse<ShopSurveyQuestionsDto>> ShopSurveyQuestionsAdd(ShopSurveyQuestionsDto ShopSurveyQuestions);
        Task<ApiResponse<ShopSurveyQuestionsDto>> ShopSurveyQuestionsEdit(ShopSurveyQuestionsDto ShopSurveyQuestions);
        Task<ApiResponse<bool>> ShopSurveyQuestionsDelete(int id);
        Task<ApiResponse<bool>> ShopSurveyQuestionsExist(int id);
        Task<ApiResponse<bool>>  ChangeAccept(AcceptDto accept);
        Task<ApiResponse<List<ShopSurveyQuestionsDto>>> ShopSurveyQuestionsGetAll();
        Task<ApiResponse<ShopSurveyQuestionsDto>> ShopSurveyQuestionsGetOne(int id);
        Task<ApiResponse<SettingDescriptionDto>> EditShopCalculateComment(SettingDescriptionDto settingDto);
        Task<ApiResponse<SettingDescriptionDto>> GetShopCalculateComment();
   
    }
}