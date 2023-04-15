namespace MarketPlace.API.Data.Dtos.DocumentType
{
    public class DocumentTypeGetDto
    {
        public int DocumentTypeId { get; set; }
        public string DocumentTitle { get; set; }
        public int FkGroupd { get; set; }
        public string GroupTitle { get; set; }
        public int? FkPersonId { get; set; }
        public string PersonTitle { get; set; }
        public bool Status { get; set; }
    }
}