using System;

namespace MarketPlace.API.Data.Dtos.Customer
{
    public class CustomerOrderCountDto
    {
         
        public int OrderCount { get; set; }
        public int PaymentCount { get; set; }
        public int ReturnCount { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public bool? EmailValid { get; set; }
    }
}