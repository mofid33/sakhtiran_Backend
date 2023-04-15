using System;
using System.Collections.Generic;

namespace MarketPlace.API.Data.Models
{
    public partial class TWorkingShift
    {
        public int ShiftId { get; set; }
        public int HourFrom { get; set; }
        public int HourTo { get; set; }
    }
}
