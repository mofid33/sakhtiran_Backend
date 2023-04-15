using System.Collections.Generic;

namespace MarketPlace.API.Data.Dtos.Guarantee
{
    public class GuaranteeDto
    {
        public int GuaranteeId { get; set; }
        public string GuaranteeTitle { get; set; }
        public string Description { get; set; }
        public bool? IsAccepted { get; set; }
        public List<CategoryGuaranteeDto> TCategoryGuarantee { get; set; }

    }
}