using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HolooneNavis.Helpers.Extensions
{
    public static class Utility
    {
        public static string ToEncodedUrl(this string url)
        {
            return System.Net.WebUtility.UrlEncode(url);
        }

        public static string GetAssemblyPath(string file)
        {
            string assemblyPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            return $"{assemblyPath}\\{file}";
        }

        public static string ExecutionAssemblyPath
        {
            get { return System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location); }
        }

        public static string ProgramDataDirectory
        {
            get
            {
                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), @"Autodesk\Fabrication\Addins\2018\JanCAMduct");
            }
        }

        public static string AddinsDirectory
        {
            get
            {
                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), @"Autodesk\Fabrication\Addins\");
            }
        }

        public static string AppDataDirectory
        {
            get
            {
                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"QRCode");
            }
        }

        public static bool TryGetFromBase64String(string input, out byte[] output)
        {
            output = null;
            try
            {
                output = Convert.FromBase64String(input);
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }

        public static string GenerateUniqueId()
        {
            int maxSize = 10;
            char[] chars = new char[62];
            chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".ToCharArray();
            byte[] data = new byte[1];
            using (RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider())
            {
                crypto.GetNonZeroBytes(data);
                data = new byte[maxSize];
                crypto.GetNonZeroBytes(data);
            }
            StringBuilder result = new StringBuilder(maxSize);
            foreach (byte b in data)
            {
                result.Append(chars[b % (chars.Length)]);
            }
            return result.ToString();
        }


        internal static double StringToNumeric(string input)
        {
            string number = Regex.Match(input, @"\d+.+\d").Value;
            double val = 0;

            if (number.Substring(0, 1) == "0")
                val = double.Parse($"0.{number.Remove(0, 1)}", new CultureInfo("en-US"));
            else
                val = double.Parse(number, new CultureInfo("en-US"));

            return val;
        }

    }
}
