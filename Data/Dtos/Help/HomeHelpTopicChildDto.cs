using System.Collections.Generic;

namespace MarketPlace.API.Data.Dtos.Help
{
    public class HomeHelpTopicChildDto
    {
        public int TopicId { get; set; }
        public string Title { get; set; }
        public int TopicParentId { get; set; }
        public string TopicParentTitle { get; set; }
        public int ArticleCount { get; set; }
        public List<HelpArticleFormDto> Articles { get; set; }
    }
}