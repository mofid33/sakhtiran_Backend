using System.ComponentModel.DataAnnotations;

namespace MarketPlace.API.Data.Dtos.Goods
{
    public class GoodsDto
    {
        public int GoodsId { get; set; }
        public int? VendorId { get; set; }
        public int FkCategoryId { get; set; }
        public string CategoryTitle { get; set; }
        public string CategoryPath { get; set; }
        public int? FkBrandId { get; set; }
        public int? FkUnitId { get; set; }
        public string SerialNumber { get; set; }
        public string Title { get; set; }
        public string ImageUrl { get; set; }
        public double? Weight { get; set; }
        public double? Length { get; set; }
        public double? Width { get; set; }
        public double? Heigth { get; set; }
        public bool IsCommonGoods { get; set; }
        public bool? ToBeDisplayed { get; set; }
        public bool? IsAccepted { get; set; }
        public bool? GoodsOwner { get; set; }
        public bool IsDownloadable { get; set; }
        public string DownloadableFileUrl { get; set; }
        public bool HaveVariation { get; set; }
        public bool SaleWithCall { get; set; }

        public GoodsProviderDto GoodsProvider { get; set; }
    }
}