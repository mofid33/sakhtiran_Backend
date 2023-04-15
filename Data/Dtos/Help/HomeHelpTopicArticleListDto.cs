using System.Collections.Generic;

namespace MarketPlace.API.Data.Dtos.Help
{
    public class HomeHelpTopicArticleListDto
    {
        public int ArticleId { get; set; }
        public int FkTopicId { get; set; }
        public string FkTopicTitle { get; set; }
        public string Subject { get; set; }
        public List<HomeHelpTopicListDto> Childs { get; set; }
    }
}