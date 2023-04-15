namespace MarketPlace.API.Data.Dtos.DocumentType
{
    public class DocumentTypeDto
    {
        public int DocumentTypeId { get; set; }
        public string DocumentTitle { get; set; }
        public int FkGroupd { get; set; }
        public int? FkPersonId { get; set; }
        public bool Status { get; set; }
    }
}