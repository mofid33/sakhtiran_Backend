using System;
using System.Collections.Generic;

namespace MarketPlace.API.Data.Models
{
    public partial class TCallRequest
    {
        public int RequestId { get; set; }
        public int FkCustomerId { get; set; }
        public int FkGoodsId { get; set; }
        public int FkGoodsProviderId { get; set; }
        public int FkStatusId { get; set; }
        public DateTime RequestDateTime { get; set; }

        public virtual TCustomer FkCustomer { get; set; }
        public virtual TGoods FkGoods { get; set; }
        public virtual TGoodsProvider FkGoodsProvider { get; set; }
        public virtual TCallRequestStatus FkStatus { get; set; }
    }
}
