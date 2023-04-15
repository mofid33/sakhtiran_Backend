using System;
using System.Collections.Generic;

namespace MarketPlace.API.Data.Models
{
    public partial class Num
    {
        public int Id { get; set; }
        public string Iso { get; set; }
        public string Name { get; set; }
        public string Nicename { get; set; }
        public string Iso3 { get; set; }
        public short? Numcod { get; set; }
        public int Phonecode { get; set; }
    }
}
