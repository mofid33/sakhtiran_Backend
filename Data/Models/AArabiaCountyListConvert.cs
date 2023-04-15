using System;
using System.Collections.Generic;

namespace MarketPlace.API.Data.Models
{
    public partial class AArabiaCountyListConvert
    {
        public string CityAscii { get; set; }
        public double? Lat { get; set; }
        public double? Lng { get; set; }
        public string Country { get; set; }
        public int Id { get; set; }
    }
}
