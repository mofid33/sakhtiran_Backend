using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Serialization.Formatters.Binary;
using LinqKit;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Vonage.Verify;
using Vonage;
using Vonage.Request;
using Microsoft.Extensions.Configuration;
using MarketPlace.API.Data.Dtos.Customer;
using System.Threading.Tasks;
using System.Collections.Specialized;
using System.Globalization;

namespace MarketPlace.API.Helper
{
    public static class Extentions
    {
        public static void AddApplicationError(this HttpResponse response, string message)
        {
            response.Headers.Add("Application-Error", message);
            response.Headers.Add("Access-Control-Allow-Origin", "*");
            response.Headers.Add("Access-Control-Expose-Header", "Application-Error");
        }

        public static T Deserialize<T>(string json)
        {
            try
            {

                using (JsonTextReader reader = new JsonTextReader(new StringReader(json)))
                {
                    JsonSerializer serializer = new JsonSerializer();

                    var model = serializer.Deserialize(reader, typeof(T));

                    return (T)model;
                }
            }
            catch (System.Exception)
            {
                return default(T);
            }
        }

        public static string GetUniqueFileName(string fileName)
        {
            fileName = Path.GetFileName(fileName);
            return Path.GetFileNameWithoutExtension(fileName)
                      + "_"
                      + Guid.NewGuid().ToString().Substring(0, 4)
                      + Path.GetExtension(fileName);
        }

        public static T DeepClone<T>(T instance)
        {
            return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(instance));
        }

        // // Deep clone
        // public static T DeepClone<T>(this T a)
        // {
        //     using (MemoryStream stream = new MemoryStream())
        //     {
        //         BinaryFormatter formatter = new BinaryFormatter();
        //         formatter.Serialize(stream, a);
        //         stream.Position = 0;
        //         return (T)formatter.Deserialize(stream);
        //     }
        // }

        public static T CloneObjectSerializable<T>(this T obj) where T : class
        {
            MemoryStream ms = new MemoryStream();
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(ms, obj);
            ms.Position = 0;
            object result = bf.Deserialize(ms);
            ms.Close();
            return (T)result;
        }


        public static DataTable CreateDataTable(List<int> ids)
        {
            DataTable table = new DataTable();
            table.Columns.Add("Id", typeof(int));
            foreach (long id in ids)
            {
                table.Rows.Add(id);
            }
            return table;
        }

        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            HashSet<TKey> seenKeys = new HashSet<TKey>();
            foreach (TSource element in source)
            {
                if (seenKeys.Add(keySelector(element)))
                {
                    yield return element;
                }
            }
        }

        // public static double GetRange(GeoPointDto origin, GeoPointDto destination)
        // {
        //     var R = 6371e3; // meteres
        //     var φ1 = ToRadians(origin.Latitude);
        //     var φ2 = ToRadians(destination.Latitude);
        //     var Δφ = ToRadians((destination.Latitude - origin.Latitude));
        //     var Δλ = ToRadians((origin.Longitude - destination.Longitude));

        //     var a = Math.Sin(Δφ / 2) * Math.Sin(Δφ / 2) +
        //             Math.Cos(φ1) * Math.Cos(φ2) *
        //             Math.Sin(Δλ / 2) * Math.Sin(Δλ / 2);
        //     var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        //     double d = Math.Ceiling(R * c);

        //     return d;
        // }


        public static string GetRandomString()
        {
            var chars = "o2d1XUCDxfhGH53g9IqiJamOPQMFstKNp870crjeAvLnkw6VWyzubEYZlB4RST";
            var stringChars = new char[8];
            var random = new Random();

            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }

            var finalString = new String(stringChars);

            return finalString;
        }

        public static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        public static List<int> GetParentIds(string text)
        {
            return text.Substring(1, text.Length - 2).Split('/').Select(Int32.Parse).Distinct().OrderBy(x => x).ToList();
        }

        public static IQueryable<T> WhereAny<T>(this IQueryable<T> q, params Expression<Func<T, bool>>[] predicates)
        {
            var orPredicate = PredicateBuilder.New<T>();
            foreach (var predicate in predicates)
            {
                orPredicate = orPredicate.Or(predicate);
            }
            return q.AsExpandable().Where(orPredicate);
        }

        public static double CalculateBalance(double import, double export)
        {
            return import - export;
        }

        public static decimal CalculateBalancePrice(decimal import, decimal export)
        {
            return import - export;
        }

        public static decimal DecimalRound(decimal price)
        {
            return decimal.Round(price, 2, MidpointRounding.AwayFromZero);
        }
        public static decimal DecimalRoundWithZiro(decimal price)
        {
            return decimal.Round(price, 0, MidpointRounding.AwayFromZero);
        }

        public static String BuildJsonFromNVC(NameValueCollection nvc)
        {
            // create base dictorary for building request structure in
            Dictionary<string, object> dict = new Dictionary<string, object>();

            // repeat for each key/value pair in list
            foreach (string key in nvc)
            {
                // split key into unique field name parts
                String[] parts = key.Split('.');

                // how many parts in total
                int count = parts.Length;

                // at beginning reset dictionary working with to base dictionary
                Dictionary<string, object> curdict = dict;

                // work way down dictionary structure for each level
                for (int i = 0; i < count; i++)
                {
                    String part = parts[i];

                    if (i == (count - 1))
                    {
                        // if at end of section, just add part name and value
                        curdict.Add(part, nvc[key]);
                    }
                    else
                    {
                        // if new level doesnt already exist, create a new nested dictionary
                        if (!curdict.ContainsKey(part))
                            curdict.Add(part, new Dictionary<string, object>());

                        // use this dictionary on next pass
                        curdict = (Dictionary<string, object>)curdict[part];
                    }
                }
            }

            // return serialized JSON result
            return JsonConvert.SerializeObject(dict, Formatting.Indented);
        }



        public static string PersianDateString(DateTime d)
        {
            try
            {
                CultureInfo faIR = new CultureInfo("fa-IR");
                return d.ToString("yyyy/MM/dd", faIR);
            }
            catch (System.Exception)
            {
                return null;
            }

        }



        public static async Task<CustomerSMSDto> SendSMS(string mobileNumber, string apiKey, string apiSecret)
        {

            try
            {
                var credentials = Credentials.FromApiKeyAndSecret(apiKey, apiSecret);
                var client = new VonageClient(credentials);
                var request = new VerifyRequest() { Brand = "sakhtiran.com", Number = mobileNumber };
                var response = await client.VerifyClient.VerifyRequestAsync(request);
                var result = new CustomerSMSDto();
                result.ErrorText = response.ErrorText;
                result.RequestId = response.RequestId;
                result.Status = response.Status;
                return result;
            }
            catch (System.Exception)
            {
                var result = new CustomerSMSDto();
                result.ErrorText = "";
                result.RequestId = "";
                result.Status = "-1";
                return result;
            }

        }
        public static async Task<CustomerSMSDto> VerifySMS(string code, string RequestId, string apiKey, string apiSecret)
        {

            try
            {
                var credentials = Credentials.FromApiKeyAndSecret(apiKey, apiSecret);
                var client = new VonageClient(credentials);
                var request = new VerifyCheckRequest() { Code = code, RequestId = RequestId };
                var response = await client.VerifyClient.VerifyCheckAsync(request);
                var result = new CustomerSMSDto();
                result.ErrorText = response.ErrorText;
                result.RequestId = response.RequestId;
                result.Status = response.Status;
                return result;
            }
            catch (System.Exception)
            {
                var result = new CustomerSMSDto();
                result.ErrorText = "";
                result.RequestId = "";
                result.Status = "-1";
                return result;
            }

        }



        public const string ApiKey = "334C446845695A50387956517553726237556275484351412B7264436C7334674D756C324D4F75687742413D";
        public static void SendPodinisVerficationCode(string verficationCode, string receptor)
        {
            var api = new Kavenegar.KavenegarApi(ApiKey);
            var result = api.VerifyLookup(receptor, verficationCode, "verifyPodinis");
        }

        public static void SendPodinisSmsForProvider(string message, string receptor)
        {
            var api = new Kavenegar.KavenegarApi(ApiKey);
            var result = api.Send("1000600660066", receptor, message);
        }


        // read xml file

        public static string getBetweenXmlFile(string strSource, string strStart, string strEnd)
        {
            int Start, End;
            if (strSource.Contains(strStart) && strSource.Contains(strEnd))
            {
                Start = strSource.IndexOf(strStart, 0) + strStart.Length;
                End = strSource.IndexOf(strEnd, Start);
                return strSource.Substring(Start, End - Start);
            }
            else
            {
                return "";
            }
        }


        /// <summary>
        /// Send sms to the number with text
        /// </summary>
        /// <param name="smsText"></param>
        /// <param name="toNumber"></param>
        public static void SendSmsText(string smsText, string toNumber)
        {
            try
            {
                var VONAGE_BRAND_NAME = Environment.GetEnvironmentVariable("VONAGE_BRAND_NAME") ?? "VONAGE_BRAND_NAME";
                var VONAGE_API_KEY = Environment.GetEnvironmentVariable("VONAGE_API_KEY") ?? "VONAGE_API_KEY";
                var VONAGE_API_SECRET = Environment.GetEnvironmentVariable("VONAGE_API_SECRET") ?? "VONAGE_API_SECRET";

                var credentials = Credentials.FromApiKeyAndSecret(
                    VONAGE_API_KEY,
                    VONAGE_API_SECRET
                    );

                var VonageClient = new VonageClient(credentials);

                var response = VonageClient.SmsClient.SendAnSms(new Vonage.Messaging.SendSmsRequest()
                {
                    To = toNumber,
                    From = VONAGE_BRAND_NAME,
                    Text = smsText
                });
            }
            catch (System.Exception)
            {
            }

        }


        public static double ToRadians(double degrees) => degrees * Math.PI / 180.0;

        public static double distanceInMiles(double lon1d, double lat1d, double lon2d , double lat2d )
        {
            try
            {
                var lon1 = ToRadians(lon1d);
                var lat1 = ToRadians(lat1d);
                var lon2 = ToRadians(lon2d);
                var lat2 = ToRadians(lat2d);

                var deltaLon = lon2 - lon1;
                var c = Math.Acos(Math.Sin(lat1) * Math.Sin(lat2) + Math.Cos(lat1) * Math.Cos(lat2) * Math.Cos(deltaLon));
                var earthRadius = 3958.76;
                var distInMiles = earthRadius * c;

                return distInMiles;
            }
            catch (System.Exception)
            {
                return 0;

            }

        }

        public static double DistanceTo(double lat1, double lon1, double lat2, double lon2, char unit = 'K')
        {
            double rlat1 = Math.PI * lat1 / 180;
            double rlat2 = Math.PI * lat2 / 180;
            double theta = lon1 - lon2;
            double rtheta = Math.PI * theta / 180;
            double dist =
                Math.Sin(rlat1) * Math.Sin(rlat2) + Math.Cos(rlat1) *
                Math.Cos(rlat2) * Math.Cos(rtheta);
            dist = Math.Acos(dist);
            dist = dist * 180 / Math.PI;
            dist = dist * 60 * 1.1515;

            switch (unit)
            {
                case 'K': //Kilometers -> default
                    return dist * 1.609344;
                case 'N': //Nautical Miles 
                    return dist * 0.8684;
                case 'M': //Miles
                    return dist;
            }
            return dist;
        }

    }
}