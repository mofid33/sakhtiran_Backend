using System;
using System.Collections.Generic;

namespace MarketPlace.API.Data.Models
{
    public partial class TMobileVerification
    {
        public long MobileVarificationId { get; set; }
        public int VerificationCode { get; set; }
        public int VerificationType { get; set; }
        public int Verified { get; set; }
        public string InsertDateTime { get; set; }
        public string Mobile { get; set; }
        public string ControlDateTime { get; set; }
        public int? ControlNumber { get; set; }
    }
}
