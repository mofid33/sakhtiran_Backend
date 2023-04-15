using System;
using MarketPlace.API.Data.Dtos.Header;
using MarketPlace.API.Data.Enums;
using Microsoft.EntityFrameworkCore.Query;
using Newtonsoft.Json.Linq;

namespace MarketPlace.API.Helper
{
    public static class JsonExtensions
    {
        public static string JsonValue(string column, [NotParameterized] string path)
        {
            throw new NotSupportedException();
        }
        public static string JsonAdd(string value, HeaderParseDto header)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }
            var jsonObject = new JObject();
            jsonObject.Add(header.LanguageNum.ToString(), value);

            if (header.LanguageNum.ToString() == LanguageEnum.Fa.ToString())
            {
                jsonObject.Add(LanguageEnum.Ar.ToString(), value);
                jsonObject.Add(LanguageEnum.En.ToString(), value);
            }
            else if (header.LanguageNum.ToString() == LanguageEnum.En.ToString())
            {
                jsonObject.Add(LanguageEnum.Fa.ToString(), value);
                jsonObject.Add(LanguageEnum.Ar.ToString(), value);
            }
            else if (header.LanguageNum.ToString() == LanguageEnum.Ar.ToString())
            {
                jsonObject.Add(LanguageEnum.Fa.ToString(), value);
                jsonObject.Add(LanguageEnum.En.ToString(), value);
            }
            return jsonObject.ToString();
        }

        public static string JsonGet(string json, HeaderParseDto header)
        {

            if (string.IsNullOrWhiteSpace(json))
            {
                return null;
            }
            try
            {
                var jsonObject = JObject.Parse(json);
                return jsonObject.GetValue(header.LanguageNum.ToString()).ToString();
            }
            catch (System.Exception)
            {
                return null;

            }

        }

        public static string JsonGet(string json, string lang)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                return null;
            }

            var jsonObject = JObject.Parse(json);

            if (lang.ToString() == LanguageEnum.Fa.ToString())
            {
                return jsonObject.GetValue(LanguageEnum.Fa.ToString()).ToString();
            }
            else if (lang.ToString() == LanguageEnum.En.ToString())
            {
                return jsonObject.GetValue(LanguageEnum.En.ToString()).ToString();
            }
            else if (lang.ToString() == LanguageEnum.Ar.ToString())
            {
                return jsonObject.GetValue(LanguageEnum.Ar.ToString()).ToString();
            }

            return jsonObject.GetValue(LanguageEnum.En.ToString()).ToString();
        }

        public static string JsonEdit(string value, string json, HeaderParseDto header)
        {
            var jsonObject = new JObject();
            if (string.IsNullOrWhiteSpace(json) && string.IsNullOrWhiteSpace(value))
            {
                return null;
            }
            if (!string.IsNullOrWhiteSpace(json))
            {
                jsonObject = JObject.Parse(json);
            }
            if (string.IsNullOrWhiteSpace((jsonObject[header.LanguageNum.ToString()] ?? String.Empty).ToString()))
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    jsonObject.Remove(header.LanguageNum.ToString());
                    if (IsNullOrEmpty(jsonObject))
                    {
                        return null;
                    }
                }
                else
                {
                    jsonObject.Add(header.LanguageNum.ToString(), value);
                }
            }
            else
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    jsonObject.Remove(header.LanguageNum.ToString());
                    if (IsNullOrEmpty(jsonObject))
                    {
                        return null;
                    }
                }
                else
                {
                    jsonObject[header.LanguageNum.ToString()] = value;
                }
            }
            return jsonObject.ToString();
        }


        public static bool IsNullOrEmpty(this JToken token)
        {
            return (token == null) ||
                   (token.Type == JTokenType.Array && !token.HasValues) ||
                   (token.Type == JTokenType.Object && !token.HasValues) ||
                   (token.Type == JTokenType.String && token.ToString() == String.Empty) ||
                   (token.Type == JTokenType.Null);
        }

    }
}