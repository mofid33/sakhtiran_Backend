using System;
using System.Collections.Generic;

namespace MarketPlace.API.Data.Models
{
    public partial class TMeasurementUnit
    {
        public TMeasurementUnit()
        {
            TGoods = new HashSet<TGoods>();
        }

        public int UnitId { get; set; }
        public string UnitTitle { get; set; }

        public virtual ICollection<TGoods> TGoods { get; set; }
    }
}
