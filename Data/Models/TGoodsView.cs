﻿using System;
using System.Collections.Generic;

namespace MarketPlace.API.Data.Models
{
    public partial class TGoodsView
    {
        public long ViewId { get; set; }
        public int FkCustomerId { get; set; }
        public int FkGoodsId { get; set; }
        public DateTime ViewDate { get; set; }
        public string IpAddress { get; set; }

        public virtual TCustomer FkCustomer { get; set; }
        public virtual TGoods FkGoods { get; set; }
    }
}
