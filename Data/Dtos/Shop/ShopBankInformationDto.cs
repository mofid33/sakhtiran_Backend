using System.Collections.Generic;

namespace MarketPlace.API.Data.Dtos.Shop
{
    public class ShopBankInformationDto
    {
        public int ShopId { get; set; }
        public string BankBeneficiaryName { get; set; }
        public string BankName { get; set; }
        public string BankBranch { get; set; }
        public string BankAccountNumber { get; set; }
        public string BankIban { get; set; }
        public string BankSwiftCode { get; set; }
        public int? FkCurrencyId { get; set; }
        public string BankAdditionalInformation { get; set; }
        public List<ShopFileDto> TShopFiles { get; set; }
    }
}