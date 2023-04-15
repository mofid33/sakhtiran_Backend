using System;
using System.Collections.Generic;

namespace MarketPlace.API.Data.Models
{
    public partial class TSystemConfiguration
    {
        public int ConfigurationId { get; set; }
        public string ConfigurationItem { get; set; }
        public string Comment { get; set; }
        public bool Status { get; set; }
    }
}
