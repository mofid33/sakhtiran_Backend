namespace MarketPlace.API.Data.Dtos.Discount
{
    public class DiscountCodeDto
    {

        public DiscountCodeDto(int codeId, long fkDiscountPlanId, string discountCode, int maxUse, int useCount,bool isValid)
        {
            this.CodeId = codeId;
            this.FkDiscountPlanId = fkDiscountPlanId;
            this.DiscountCode = discountCode;
            this.MaxUse = maxUse;
            this.UseCount = useCount;
            this.IsValid = isValid;

        }
        public int CodeId { get; set; }
        public long FkDiscountPlanId { get; set; }
        public string DiscountCode { get; set; }
        public int MaxUse { get; set; }
        public int UseCount { get; set; }
        public bool? IsValid { get; set; }

    }
}