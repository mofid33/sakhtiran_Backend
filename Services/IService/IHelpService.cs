using System.Threading.Tasks;
using MarketPlace.API.Data.Dtos.Help;
using MarketPlace.API.Data.Dtos.Pagination;
using MarketPlace.API.Helper;

namespace MarketPlace.API.Services.IService
{
    public interface IHelpService
    {
         Task<ApiResponse<bool>> AddHelpTopic(HelpTopicSerializeDto helpTopicDto);
         Task<ApiResponse<bool>> EditHelpTopic(HelpTopicSerializeDto helpTopicDto);
         Task<ApiResponse<Pagination<HelpTopicListDto>>> GetHelpTopicList(PaginationFormDto pagination);
         Task<ApiResponse<HelpTopicGetDto>> GetHelpTopicById(int topicId);         
         Task<ApiResponse<bool>> AddHelpArticle(HelpArticleAddDto helpArticleDto);
         Task<ApiResponse<bool>> EditHelpArticle(HelpArticleAddDto helpArticleDto);
         Task<ApiResponse<Pagination<HelpArticleListDto>>> GetHelpArticleList(PaginationFormDto pagination);
         Task<ApiResponse<HelpArticleGetDto>> GetHelpArticleById(int articleId);
        Task<ApiResponse<bool>> TopicDelete(int topicId);
        Task<ApiResponse<bool>> ArticleDelete(int articleId);

    }
}