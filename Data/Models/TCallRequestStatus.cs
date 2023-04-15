using System;
using System.Collections.Generic;

namespace MarketPlace.API.Data.Models
{
    public partial class TCallRequestStatus
    {
        public TCallRequestStatus()
        {
            TCallRequest = new HashSet<TCallRequest>();
        }

        public int StatusId { get; set; }
        public string StatusTitle { get; set; }

        public virtual ICollection<TCallRequest> TCallRequest { get; set; }
    }
}
