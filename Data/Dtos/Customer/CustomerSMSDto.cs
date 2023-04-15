namespace MarketPlace.API.Data.Dtos.Customer
{
    public class CustomerSMSDto
    {
        public string ErrorText { get; set; }
        public string RequestId { get; set; }
        public string Status { get; set; }
        public string MobileNumber { get; set; }
        public int  AddressId { get; set; }
    }
}