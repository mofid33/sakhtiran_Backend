using System;
using System.Collections.Generic;

namespace MarketPlace.API.Data.Models
{
    public partial class TShopComment
    {
        public TShopComment()
        {
            TShopCommentLike = new HashSet<TShopCommentLike>();
        }

        public int CommentId { get; set; }
        public int FkCustomerId { get; set; }
        public int FkShopId { get; set; }
        public string CommentTitle { get; set; }
        public string CommentText { get; set; }
        public DateTime CommentDate { get; set; }
        public int LikeCount { get; set; }
        public bool? IsAccepted { get; set; }

        public virtual TCustomer FkCustomer { get; set; }
        public virtual TShop FkShop { get; set; }
        public virtual ICollection<TShopCommentLike> TShopCommentLike { get; set; }
    }
}
