using System.Collections.Generic;

namespace MarketPlace.API.Data.Dtos.Help
{
    public class HomeHelpArticleDto
    {
        public int ArticleId { get; set; }
        public int TopicId { get; set; }
        public string TopicTitle { get; set; }
        public int? ParentTopicId { get; set; }
        public string ParentTopicTitle { get; set; }
        public string Subject { get; set; }
        public string Description { get; set; }
        public List<HelpArticleFormDto> Articles { get; set; }
    }
}