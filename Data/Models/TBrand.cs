using System;
using System.Collections.Generic;

namespace MarketPlace.API.Data.Models
{
    public partial class TBrand
    {
        public TBrand()
        {
            TCategoryBrand = new HashSet<TCategoryBrand>();
            TGoods = new HashSet<TGoods>();
        }

        public int BrandId { get; set; }
        public string BrandTitle { get; set; }
        public string BrandLogoImage { get; set; }
        public string Description { get; set; }
        public bool? IsAccepted { get; set; }

        public virtual ICollection<TCategoryBrand> TCategoryBrand { get; set; }
        public virtual ICollection<TGoods> TGoods { get; set; }
    }
}
