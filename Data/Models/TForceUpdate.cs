using System;
using System.Collections.Generic;

namespace MarketPlace.API.Data.Models
{
    public partial class TForceUpdate
    {
        public int ForceUpdateId { get; set; }
        public int? AndroidVersionCode { get; set; }
        public string AndroidVersionName { get; set; }
        public int? IosBuildNumber { get; set; }
        public string IosVersionName { get; set; }
        public bool? ForceUpdateAndroid { get; set; }
        public bool? ForceUpdateIos { get; set; }
        public bool? ActiveAndroidDirectDownload { get; set; }
        public bool? ActiveAndroidGooglePlayDownload { get; set; }
        public bool? ActiveIosAppStoreDownload { get; set; }
        public string GooglePlayeLink { get; set; }
        public string AppStoreLink { get; set; }
        public string DirectDownloadLink { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
    }
}
