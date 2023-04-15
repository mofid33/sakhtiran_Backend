using System;

namespace MarketPlace.API.Data.Dtos.Help
{
    public class HelpArticleListDto
    {
        public int ArticleId { get; set; }
        public int FkTopicId { get; set; }
        public string FkTopicTitle { get; set; }
        public string LastUpdateDateTime { get; set; }
        public string Subject { get; set; }
        public string Description { get; set; }
        public int HelpfulCount { get; set; }
        public int UnhelpfulCount { get; set; }
        public bool Status { get; set; }
    }
}