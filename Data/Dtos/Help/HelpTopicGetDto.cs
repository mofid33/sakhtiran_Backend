namespace MarketPlace.API.Data.Dtos.Help
{
    public class HelpTopicGetDto
    {
        public int TopicId { get; set; }
        public int? FkTopicId { get; set; }
        public string FkTopicTitle { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string IconUrl { get; set; }
        public bool Status { get; set; }
    }
}