using System;
using System.Collections.Generic;

namespace MarketPlace.API.Data.Dtos.Order
{
    public class ShippmentDto
    {
        public int Count { get; set; }
        public List<ShippmentListDto> ShippmentList { get; set; }
        
    }
}