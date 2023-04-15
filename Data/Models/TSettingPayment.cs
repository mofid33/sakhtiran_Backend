using System;
using System.Collections.Generic;

namespace MarketPlace.API.Data.Models
{
    public partial class TSettingPayment
    {
        public int SettingPaymentId { get; set; }
        public long TerminalId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string PaymentTitle { get; set; }
    }
}
