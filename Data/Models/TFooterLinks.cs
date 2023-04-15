using System;
using System.Collections.Generic;

namespace MarketPlace.API.Data.Models
{
    public partial class TFooterLinks
    {
        public TFooterLinks()
        {
            InverseFkGroup = new HashSet<TFooterLinks>();
        }

        public short FooterLinkId { get; set; }
        public bool IsGroup { get; set; }
        public string Title { get; set; }
        public short? FkGroupId { get; set; }
        public string LinkToken { get; set; }
        public string LinkContent { get; set; }

        public virtual TFooterLinks FkGroup { get; set; }
        public virtual ICollection<TFooterLinks> InverseFkGroup { get; set; }
    }
}
