using System;
using System.Collections.Generic;

namespace MarketPlace.API.Data.Models
{
    public partial class TGoodsQueAns
    {
        public TGoodsQueAns()
        {
            InverseFkQa = new HashSet<TGoodsQueAns>();
        }

        public int Qaid { get; set; }
        public int FkGoodsId { get; set; }
        public int FkCustomerId { get; set; }
        public int? FkQaid { get; set; }
        public string Qatext { get; set; }
        public bool NotifyFirstAns { get; set; }
        public DateTime Qadate { get; set; }
        public bool? IsAccepted { get; set; }

        public virtual TCustomer FkCustomer { get; set; }
        public virtual TGoods FkGoods { get; set; }
        public virtual TGoodsQueAns FkQa { get; set; }
        public virtual ICollection<TGoodsQueAns> InverseFkQa { get; set; }
    }
}
