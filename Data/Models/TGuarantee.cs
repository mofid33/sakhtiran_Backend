using System;
using System.Collections.Generic;

namespace MarketPlace.API.Data.Models
{
    public partial class TGuarantee
    {
        public TGuarantee()
        {
            TCategoryGuarantee = new HashSet<TCategoryGuarantee>();
            TGoodsProvider = new HashSet<TGoodsProvider>();
        }

        public int GuaranteeId { get; set; }
        public string GuaranteeTitle { get; set; }
        public string Description { get; set; }
        public bool? IsAccepted { get; set; }

        public virtual ICollection<TCategoryGuarantee> TCategoryGuarantee { get; set; }
        public virtual ICollection<TGoodsProvider> TGoodsProvider { get; set; }
    }
}
