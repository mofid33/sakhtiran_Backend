using System;
using System.Collections.Generic;

namespace MarketPlace.API.Data.Models
{
    public partial class TShopCommentLike
    {
        public int LikeId { get; set; }
        public int FkCustomerId { get; set; }
        public int FkCommentId { get; set; }
        public DateTime LikeDate { get; set; }

        public virtual TShopComment FkComment { get; set; }
        public virtual TCustomer FkCustomer { get; set; }
    }
}
