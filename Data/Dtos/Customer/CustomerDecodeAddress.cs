using System.Collections.Generic;

namespace MarketPlace.API.Data.Dtos.Customer
{
    public class CustomerDecodeAddressDto 
    {
        public string long_name { get; set; }
        public string short_name { get; set; }
        public List<string> types { get; set; }
    }
}