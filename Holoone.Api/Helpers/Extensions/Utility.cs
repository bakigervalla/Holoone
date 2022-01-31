using Holoone.Api.Helpers.Constants;
using Holoone.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Holoone.Api.Helpers.Extensions
{
    public static class Utility
    {
        public static string ToEncodedUrl(this string url)
        {
            return System.Net.WebUtility.UrlEncode(url);
        }

        public static string UrlDecode(this string url)
        {
            return System.Net.WebUtility.UrlDecode(url);
        }

        public static string HtmlDecode(this string url)
        {
            return System.Text.RegularExpressions.Regex.Unescape(url);
            // url.Replace(@"\u0026", "&");
        }


        public static string DecodeUrlString(this string url)
        {
            string newUrl;
            while ((newUrl = Uri.UnescapeDataString(url)) != url)
                url = newUrl;
            return newUrl;
        }

        public static string GetTextBetween(this string strSource, string strStart, string strEnd)
        {
            if (strSource.Contains(strStart) && strSource.Contains(strEnd))
            {
                int Start, End;
                Start = strSource.IndexOf(strStart, 0) + strStart.Length;
                End = strSource.IndexOf(strEnd, Start);
                return strSource.Substring(Start, End - Start);
            }

            return "";
        }

        public static string GetBaseUrl(UserLogin login)
        {
            return login.LoginType.Type == "LCP"
                                    ? RequestConstants.LenovoBaseUrls[login.LoginType.Region]
                                    : RequestConstants.SphereBaseUrls[login.LoginType.Region];
        }

    }
}
