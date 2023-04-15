using System.Collections.Generic;
using System.Threading.Tasks;
using MarketPlace.API.Data.Dtos.Accept;
using MarketPlace.API.Data.Dtos.Help;
using MarketPlace.API.Data.Dtos.Pagination;
using MarketPlace.API.Data.Models;
using MarketPlace.API.Helper;

namespace MarketPlace.API.Data.Repositories.IRepository
{
    public interface IHelpRepository
    {
         Task<THelpTopic> AddHelpTopic(THelpTopic helpTopicDto);
         Task<bool> EditHelpTopic(THelpTopic helpTopicDto);
         Task<List<HelpTopicListDto>> GetHelpTopicList(PaginationFormDto pagination);
         Task<int> GetHelpTopicListCount(PaginationFormDto pagination);
         Task<HelpTopicGetDto> GetHelpTopicById(int topicId);         
         Task<bool> AddHelpArticle(THelpArticle helpArticleDto);
         Task<bool> EditHelpArticle(THelpArticle helpArticleDto);
         Task<List<HelpArticleListDto>> GetHelpArticleList(PaginationFormDto pagination);
         Task<int> GetHelpArticleListCount(PaginationFormDto pagination);
         Task<HelpArticleGetDto> GetHelpArticleById(int articleId);


        Task<List<HelpArticleFormDto>> HelpAutoComplete(string search);
        Task<List<HomeHelpTopicListDto>> GetHomeHelpTopic();
        Task<HomeHelpTopicChildDto> GetHelpTopic(int topicId);
        Task<HomeHelpTopicDto> GetHelpParentTopic(int topicId);
        Task<HomeHelpArticleDto> GetHelpArticle(int articleId);
        Task<bool> AddHelpFul(AcceptDto accept);


        Task<RepRes<THelpTopic>> TopicDelete(int id);
        Task<RepRes<THelpArticle>> ArticleDelete(int id);
        Task<string> GetHelpImage();
    }
}