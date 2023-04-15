using System;

namespace MarketPlace.API.Data.Dtos.UserOrder
{
    public class OrderCountDto
    {
        public OrderCountDto(int count,Guid? cookieId , bool setOneClick)
        {
            this.Count = count;
            this.CookieId = cookieId;
            this.SetOneClick = setOneClick;
        }
        public int Count { get; set; }
        public Guid? CookieId { get; set; }
        public bool SetOneClick { get; set; }
    }
}