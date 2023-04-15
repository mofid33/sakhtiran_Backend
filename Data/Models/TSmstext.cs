using System;
using System.Collections.Generic;

namespace MarketPlace.API.Data.Models
{
    public partial class TSmstext
    {
        public int SmsId { get; set; }
        public string Token { get; set; }
        public string SmsText { get; set; }
    }
}
