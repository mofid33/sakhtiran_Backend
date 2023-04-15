namespace MarketPlace.API.Data.Dtos.Help
{
    public class HelpArticleAddDto
    {
        public int ArticleId { get; set; }
        public int FkTopicId { get; set; }
        public string Subject { get; set; }
        public string Description { get; set; }
        public bool Status { get; set; }
    }
}