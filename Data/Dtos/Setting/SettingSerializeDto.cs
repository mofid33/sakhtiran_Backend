using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace MarketPlace.API.Data.Dtos.Setting
{
    public class SettingSerializeDto
    {
        public IFormFile ShopHeaderLogoUrl { get; set; }
        public IFormFile ShopFooterLogoUrl { get; set; }
        public IFormFile LoginPageLogoUrl { get; set; }
        public IFormFile WebLoginBkUrl { get; set; }
        public IFormFile WebHelpBkUrl { get; set; }
        public IFormFile ShopDefaultBanner { get; set; }
    }
}