using System.Collections.Generic;

namespace MarketPlace.API.Data.Dtos.ShippingMethod
{
    public class ShippingMethodDto
    {
        public int Id { get; set; }
        public string ShippingMethodTitle { get; set; }
        public bool CashOnDelivery { get; set; }
        public bool? HaveOnlineService { get; set; }
        public int? BaseWeight { get; set; }
        public bool? Active { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }

        public List<ShippingOnCountryDto> TShippingOnCountry { get; set; }
    }
}