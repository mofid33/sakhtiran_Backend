using System;
using System.Collections.Generic;

namespace MarketPlace.API.Data.Models
{
    public partial class TShopFiles
    {
        public int FileId { get; set; }
        public int FkShopId { get; set; }
        public int FkDocumentTypeId { get; set; }
        public string FileUrl { get; set; }

        public virtual TDocumentType FkDocumentType { get; set; }
        public virtual TShop FkShop { get; set; }
    }
}
