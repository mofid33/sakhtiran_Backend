using System;
using System.Collections.Generic;

namespace MarketPlace.API.Data.Models
{
    public partial class TOrderReturningLog
    {
        public int LogId { get; set; }
        public int FkReturningId { get; set; }
        public int FkStatusId { get; set; }
        public Guid FkUserId { get; set; }
        public string LogComment { get; set; }
        public DateTime LogDateTime { get; set; }

        public virtual TOrderReturning FkReturning { get; set; }
        public virtual TReturningStatus FkStatus { get; set; }
        public virtual TUser FkUser { get; set; }
    }
}
