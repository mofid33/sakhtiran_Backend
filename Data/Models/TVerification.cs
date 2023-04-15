using System;
using System.Collections.Generic;

namespace MarketPlace.API.Data.Models
{
    public partial class TVerification
    {
        public long VarificationId { get; set; }
        public int VerificationCode { get; set; }
        public int VerificationType { get; set; }
        public int Verified { get; set; }
        public DateTime InsertDateTime { get; set; }
        public string Email { get; set; }
        public DateTime ControlDateTime { get; set; }
        public int? ControlNumber { get; set; }
        public string MobileNumber { get; set; }
    }
}
