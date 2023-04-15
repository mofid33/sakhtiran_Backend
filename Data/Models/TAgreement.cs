using System;
using System.Collections.Generic;

namespace MarketPlace.API.Data.Models
{
    public partial class TAgreement
    {
        public int TypeId { get; set; }
        public string TypeTitle { get; set; }
        public string Comment { get; set; }
    }
}
