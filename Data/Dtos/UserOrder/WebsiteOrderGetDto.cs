using System.Collections.Generic;

namespace MarketPlace.API.Data.Dtos.UserOrder
{
    public class WebsiteOrderGetDto
    {
        public long OrderId { get; set; }
        public double ItemsCount { get; set; }
        public decimal Shipping { get; set; }
        public decimal Discount { get; set; }
        public decimal Vat { get; set; }
        public decimal TotalWithOutDiscountCode { get; set; }
        public decimal Total { get; set; }
        public int CountryId { get; set; }
        public int ProvinceId { get; set; }
        public string CountryTitle { get; set; }
        public string ProvinceTitle { get; set; }
        public int CityId { get; set; }
        public int PayerId { get; set; }
        public string CityTitle { get; set; }
        public string TransfereeName { get; set; }
        public string TransfereeFamily { get; set; }
        public string TransfereeMobile { get; set; }
        public string Iso { get; set; }
        public string TransfereeEmail { get; set; }
        public string Address { get; set; }
        public string TrackingCode { get; set; }
        public string PhoneCode { get; set; }
        public bool IsDownloadable { get; set; }

        public List<WebsiteOrderItemGetDto> Items { get; set; }
    }
}