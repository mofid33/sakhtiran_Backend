using System;
using System.Collections.Generic;

namespace MarketPlace.API.Data.Models
{
    public partial class TDocumentType
    {
        public TDocumentType()
        {
            TShopFiles = new HashSet<TShopFiles>();
        }

        public int DocumentTypeId { get; set; }
        public string DocumentTitle { get; set; }
        public int FkGroupd { get; set; }
        public int? FkPersonId { get; set; }
        public bool Status { get; set; }

        public virtual TDocumentGroup FkGroupdNavigation { get; set; }
        public virtual TPerson FkPerson { get; set; }
        public virtual ICollection<TShopFiles> TShopFiles { get; set; }
    }
}
