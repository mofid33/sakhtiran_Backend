namespace MarketPlace.API.Data.Dtos.Message
{
    public class MessageAttachmentDto
    {
        public int AttachmentId { get; set; }
        public int FkMessageId { get; set; }
        public string Title { get; set; }
        public string FileUrl { get; set; }
    }
}