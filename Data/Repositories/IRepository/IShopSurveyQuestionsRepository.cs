using System.Collections.Generic;
using System.Threading.Tasks;
using MarketPlace.API.Data.Dtos;
using MarketPlace.API.Data.Dtos.Accept;
using MarketPlace.API.Data.Dtos.Setting;
using MarketPlace.API.Data.Models;
using MarketPlace.API.Helper;

namespace MarketPlace.API.Data.Repositories.IRepository
{
    public interface IShopSurveyQuestionsRepository
    {
        Task<TShopSurveyQuestions> ShopSurveyQuestionsAdd(TShopSurveyQuestions ShopSurveyQuestions);
        Task<TShopSurveyQuestions> ShopSurveyQuestionsEdit(TShopSurveyQuestions ShopSurveyQuestions);
        Task<RepRes<TShopSurveyQuestions>> ShopSurveyQuestionsDelete(int id);
        Task<bool> ShopSurveyQuestionsExist(int id);
        Task<bool> ChangeAccept(AcceptDto accept);

        Task<List<ShopSurveyQuestionsDto>> ShopSurveyQuestionsGetAll(bool getActive = false);
        Task<ShopSurveyQuestionsDto> ShopSurveyQuestionsGetOne(int id);

        Task<SettingDescriptionDto> EditShopCalculateComment(SettingDescriptionDto settingDto);
        Task<SettingDescriptionDto> GetShopCalculateComment();
    }
}