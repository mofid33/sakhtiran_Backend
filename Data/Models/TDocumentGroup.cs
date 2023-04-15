using System;
using System.Collections.Generic;

namespace MarketPlace.API.Data.Models
{
    public partial class TDocumentGroup
    {
        public TDocumentGroup()
        {
            TDocumentType = new HashSet<TDocumentType>();
        }

        public int DocumentTypeId { get; set; }
        public string DocumentTypeTitle { get; set; }

        public virtual ICollection<TDocumentType> TDocumentType { get; set; }
    }
}
