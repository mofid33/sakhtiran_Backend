using System;

namespace MarketPlace.API.Data.Dtos.OrderReturning
{
    public class OrderReturningLogDto
    {
        public int LogId { get; set; }
        public int FkReturningId { get; set; }
        public int FkStatusId { get; set; }
        public string StatusTitle { get; set; }
        public Guid FkUserId { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string LogComment { get; set; }
        public string LogDateTime { get; set; }
    }
}