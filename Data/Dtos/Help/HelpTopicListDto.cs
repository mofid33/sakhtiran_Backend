namespace MarketPlace.API.Data.Dtos.Help
{
    public class HelpTopicListDto
    {
        public int TopicId { get; set; }
        public int? FkTopicId { get; set; }
        public string Location { get; set; }
        public string Title { get; set; }
        public string IconUrl { get; set; }
        public int ArticlesCount { get; set; }
    }
}