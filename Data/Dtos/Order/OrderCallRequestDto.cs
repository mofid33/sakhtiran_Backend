using System;
using System.Collections.Generic;
using MarketPlace.API.Data.Dtos.Shop;

namespace MarketPlace.API.Data.Dtos.Order
{
    public class OrderCallRequestDto
    {
        public long RequestId { get; set; }
        public string Date { get; set; }
        public string Customer { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Vendor { get; set; }
        public string ProductName { get; set; }
        public int Status { get; set; }
    }
}