namespace MarketPlace.API.Data.Dtos.Message
{
    public class MessageRecipientDto
    {
        public int RecipientId { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public bool ViewFlag { get; set; }
    }
}