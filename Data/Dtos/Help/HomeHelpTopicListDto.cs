namespace MarketPlace.API.Data.Dtos.Help
{
    public class HomeHelpTopicListDto
    {
        public int TopicId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string IconUrl { get; set; }
        public int ArticleCount { get; set; }
    }
}