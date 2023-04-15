namespace MarketPlace.API.Data.Dtos.Help
{
    public class HelpTopicAddDto
    {
        public int TopicId { get; set; }
        public int? FkTopicId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string IconUrl { get; set; }
        public bool Status { get; set; }
    }
}