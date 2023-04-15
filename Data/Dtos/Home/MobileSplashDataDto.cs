using MarketPlace.API.Data.Models;

namespace MarketPlace.API.Data.Dtos.Home
{
    public class MobileSplashDataDto
    {
        public int CartCount { get; set; }
        
        public string DefualtLanguage { get; set; }
        public string Domain { get; set; }

        public string DefualtCurrency { get; set; }
        public bool IsRtl { get; set; }
        public string HeaderLogo { get; set; }
        public string NotificationKey { get; set; }
        public bool IsForceUpdate { get; set; }
        public TForceUpdate ForceUpdateObj { get; set; }
    }
}