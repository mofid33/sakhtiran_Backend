using System;
using System.Collections.Generic;
using System.Text;

namespace MarketPlace.API.Data.Constants
{
    public static class AppConstants
    {
        public static class FirebaseNotificationTopics
        {
            public const string CLIENT_MOBILE = "client-mobile"; // all
            public const string CLIENT_MOBILE_ANDROID = "client-mobile-android";
            public const string CLIENT_MOBILE_IOS = "client-mobile-ios";
            public const string CLIENT_MOBILE_LOGGEDIN = "client-mobile-loggedin";
            public const string CLIENT_MOBILE__NOT_LOGGEDIN = "client-mobile-not-loggedin";
            public const string CLIENT_WEB = "client-web"; // all
            public const string CLIENT_WEB_LOGGEDIN = "client-web-loggedin"; // all
            public const string CLIENT_WEB_NOT_LOGGEDIN = "client-web-not-loggedin"; // all
            public const string PROVIDER_PANEL = "provider-panel";

        }

    }
}
