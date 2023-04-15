using System.Collections.Generic;

namespace MarketPlace.API.Data.Dtos.Order
{
    public class LiveCartDetailGetDto
    {
        public long OrderId { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public List<LiveCartDetailDto> Items { get; set; }
    }
}