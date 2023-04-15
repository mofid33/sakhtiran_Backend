using System;
using System.Collections.Generic;

namespace MarketPlace.API.Data.Dtos.Message
{
    public class MessageFilterDto
    {
        public bool IsSingle { get; set; }
        public int UserType { get; set; }
        public List<int> PlanId { get; set; }
        public List<int> Country { get; set; }
        public List<int> Province { get; set; }
        public List<int> City { get; set; }
        public List<Guid> UserId { get; set; }
    }
}