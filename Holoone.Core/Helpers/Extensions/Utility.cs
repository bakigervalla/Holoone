using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Holoone.Core.Helpers.Extensions
{
    public static class Utility
    {
        public static string ToEncodedUrl(this string url)
        {
            return System.Net.WebUtility.UrlEncode(url);
        }
    }
}
