using System;
using System.Collections.Generic;

namespace MarketPlace.API.Data.Models
{
    public partial class TPerson
    {
        public TPerson()
        {
            TDocumentType = new HashSet<TDocumentType>();
            TShop = new HashSet<TShop>();
        }

        public int PersonTypeId { get; set; }
        public string PersonTypeTitle { get; set; }

        public virtual ICollection<TDocumentType> TDocumentType { get; set; }
        public virtual ICollection<TShop> TShop { get; set; }
    }
}
