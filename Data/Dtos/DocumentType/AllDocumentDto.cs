using System.Collections.Generic;

namespace MarketPlace.API.Data.Dtos.DocumentType
{
    public class AllDocumentDto
    {
        public int DocumentTypeId { get; set; }
        public string DocumentTypeTitle { get; set; }
        public List<DocumentTypeFormDto>  DocumentType { get ; set; }
    }
}