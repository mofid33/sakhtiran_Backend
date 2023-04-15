using System;
using System.Collections.Generic;

namespace MarketPlace.API.Data.Models
{
    public partial class TShop
    {
        public TShop()
        {
            TDiscountPlan = new HashSet<TDiscountPlan>();
            TDiscountShops = new HashSet<TDiscountShops>();
            TGoods = new HashSet<TGoods>();
            TGoodsProvider = new HashSet<TGoodsProvider>();
            TOrderItem = new HashSet<TOrderItem>();
            TPaymentTransaction = new HashSet<TPaymentTransaction>();
            TShopActivityCity = new HashSet<TShopActivityCity>();
            TShopActivityCountry = new HashSet<TShopActivityCountry>();
            TShopCategory = new HashSet<TShopCategory>();
            TShopComment = new HashSet<TShopComment>();
            TShopFiles = new HashSet<TShopFiles>();
            TShopPlanExclusive = new HashSet<TShopPlanExclusive>();
            TShopSlider = new HashSet<TShopSlider>();
            TShopSurveyAnswers = new HashSet<TShopSurveyAnswers>();
            TShopWithdrawalRequest = new HashSet<TShopWithdrawalRequest>();
            TUser = new HashSet<TUser>();
        }

        public int ShopId { get; set; }
        public short FkStatusId { get; set; }
        public int FkCountryId { get; set; }
        public int? FkCityId { get; set; }
        public int FkPersonId { get; set; }
        public int? FkPlanId { get; set; }
        public DateTime RegisteryDateTime { get; set; }
        public string VendorUrlid { get; set; }
        public string StoreName { get; set; }
        public decimal Credit { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string CompanyName { get; set; }
        public string FullName { get; set; }
        public double? LocationX { get; set; }
        public double? LocationY { get; set; }
        public string Address { get; set; }
        public string LogoImage { get; set; }
        public string ProfileImage { get; set; }
        public string AboutShop { get; set; }
        public string TermCondition { get; set; }
        public double? SurveyScore { get; set; }
        public string BankBeneficiaryName { get; set; }
        public string BankName { get; set; }
        public string BankBranch { get; set; }
        public string BankAccountNumber { get; set; }
        public string BankIban { get; set; }
        public string BankSwiftCode { get; set; }
        public int? FkCurrencyId { get; set; }
        public string BankAdditionalInformation { get; set; }
        public bool GoodsIncludedVat { get; set; }
        public string TaxRegistrationNumber { get; set; }
        public bool ShippingPossibilities { get; set; }
        public bool ShippingPermission { get; set; }
        public int? ShippingBaseWeight { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public short? MaxSliderForShopWebPage { get; set; }
        public int? MaxCategory { get; set; }
        public int? MaxProduct { get; set; }
        public bool? Microstore { get; set; }
        public double? CommissionFraction { get; set; }
        public decimal? RentPerMonth { get; set; }
        public bool? AutoAccountRecharge { get; set; }
        public int? FkProvinceId { get; set; }
        public string InactiveShopMessage { get; set; }
        public string ShopShippingCode { get; set; }

        public virtual TCity FkCity { get; set; }
        public virtual TCountry FkCountry { get; set; }
        public virtual TPerson FkPerson { get; set; }
        public virtual TShopPlan FkPlan { get; set; }
        public virtual TProvince FkProvince { get; set; }
        public virtual TShopStatus FkStatus { get; set; }
        public virtual ICollection<TDiscountPlan> TDiscountPlan { get; set; }
        public virtual ICollection<TDiscountShops> TDiscountShops { get; set; }
        public virtual ICollection<TGoods> TGoods { get; set; }
        public virtual ICollection<TGoodsProvider> TGoodsProvider { get; set; }
        public virtual ICollection<TOrderItem> TOrderItem { get; set; }
        public virtual ICollection<TPaymentTransaction> TPaymentTransaction { get; set; }
        public virtual ICollection<TShopActivityCity> TShopActivityCity { get; set; }
        public virtual ICollection<TShopActivityCountry> TShopActivityCountry { get; set; }
        public virtual ICollection<TShopCategory> TShopCategory { get; set; }
        public virtual ICollection<TShopComment> TShopComment { get; set; }
        public virtual ICollection<TShopFiles> TShopFiles { get; set; }
        public virtual ICollection<TShopPlanExclusive> TShopPlanExclusive { get; set; }
        public virtual ICollection<TShopSlider> TShopSlider { get; set; }
        public virtual ICollection<TShopSurveyAnswers> TShopSurveyAnswers { get; set; }
        public virtual ICollection<TShopWithdrawalRequest> TShopWithdrawalRequest { get; set; }
        public virtual ICollection<TUser> TUser { get; set; }
    }
}
