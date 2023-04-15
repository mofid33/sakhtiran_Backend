using System;
using System.Collections.Generic;

namespace MarketPlace.API.Data.Models
{
    public partial class WebCollectionType
    {
        public WebCollectionType()
        {
            WebModuleCollections = new HashSet<WebModuleCollections>();
            WebSlider = new HashSet<WebSlider>();
        }

        public int CollectionTypeId { get; set; }
        public string CollectionTypeTitle { get; set; }
        public bool ForCustomer { get; set; }
        public bool SelectCategory { get; set; }
        public bool SelectGoods { get; set; }
        public bool SelectSpecialSale { get; set; }

        public virtual ICollection<WebModuleCollections> WebModuleCollections { get; set; }
        public virtual ICollection<WebSlider> WebSlider { get; set; }
    }
}
