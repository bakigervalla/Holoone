using Autodesk.Navisworks.Api;
using Holoone.Api.Helpers.Constants;
using Holoone.Api.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Management;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace HolooneNavis.Helpers
{
    class Util
    {
        #region Formatting
        /// <summary>
        /// Return an English plural suffix for the given
        /// number of items, i.e. 's' for zero or more
        /// than one, and nothing for exactly one.
        /// </summary>
        public static string PluralSuffix(int n)
        {
            return 1 == n ? "" : "s";
        }

        /// <summary>
        /// Return an English plural suffix 'ies' or
        /// 'y' for the given number of items.
        /// </summary>
        public static string PluralSuffixY(int n)
        {
            return 1 == n ? "y" : "ies";
        }

        /// <summary>
        /// Return a dot (full stop) for zero
        /// or a colon for more than zero.
        /// </summary>
        public static string DotOrColon(int n)
        {
            return 0 < n ? ":" : ".";
        }

        /// <summary>
        /// Return a string for a real number
        /// formatted to two decimal places.
        /// </summary>
        public static string RealString(double a)
        {
            return a.ToString("0.##");
        }

        /// <summary>
        /// Return a string for a UV point
        /// or vector with its coordinates
        /// formatted to two decimal places.
        /// </summary>
        public static string PointString(
          Point2D p,
          bool onlySpaceSeparator = false)
        {
            string format_string = onlySpaceSeparator
              ? "{0} {1}"
              : "({0},{1})";

            return string.Format(format_string,
              RealString(p.X),
              RealString(p.Y));
        }

        /// <summary>
        /// Return a string for an XYZ point
        /// or vector with its coordinates
        /// formatted to two decimal places.
        /// </summary>
        public static string PointString(
          Point3D p,
          bool onlySpaceSeparator = false)
        {
            string format_string = onlySpaceSeparator
              ? "{0} {1} {2}"
              : "({0},{1},{2})";

            return string.Format(format_string,
              RealString(p.X),
              RealString(p.Y),
              RealString(p.Z));
        }

        /// <summary>
        /// Return a string for this bounding box
        /// with its coordinates formatted to two
        /// decimal places.
        /// </summary>
        public static string BoundingBoxString(
          BoundingBox3D bb,
          bool onlySpaceSeparator = false)
        {
            string format_string = onlySpaceSeparator
              ? "{0} {1}"
              : "({0},{1})";

            return string.Format(format_string,
              PointString(bb.Min, onlySpaceSeparator),
              PointString(bb.Max, onlySpaceSeparator));
        }
        #endregion // Formatting

        public static ObservableCollection<Anchor> Anchors { get; set; } = new ObservableCollection<Anchor>();

        public static string GetDeviceIdentifier(DeviceType device)
        {
            switch (device)
            {
                case DeviceType.MAC_ADDRESS:
                    return GetMacAddress();
                case DeviceType.DEVICE_ID:
                    return ReturnHardWareID().Result;
                default:
                    return null;
            }
        }

        private static string GetMacAddress()
        {
            foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
            {
                // Only consider Ethernet network interfaces
                if (nic.NetworkInterfaceType == NetworkInterfaceType.Ethernet &&
                    nic.OperationalStatus == OperationalStatus.Up)
                {
                    return nic.GetPhysicalAddress().ToString();
                }
            }
            return string.Empty;
        }

        private static async Task<string> ReturnHardWareID()
        {
            string s = "";
            Task task = Task.Run(() =>
            {
                ManagementObjectSearcher bios = new ManagementObjectSearcher("SELECT * FROM Win32_BIOS");
                ManagementObjectCollection bios_Collection = bios.Get();
                foreach (ManagementObject obj in bios_Collection)
                {
                    s = obj["SerialNumber"].ToString();
                    break; //break just to get the first found object data only
                }
            });
            Task.WaitAll(task);

            return await Task.FromResult(s);
        }

        public static string MarkerPath(string markerName)
        {
            return Path.Combine(Path.GetTempPath(), "HolooneNavis", $"{markerName}.wrl");   // $"sphere_anchor_{Name}";
        }

    }

    enum DeviceType
    {
        MAC_ADDRESS = 1,
        DEVICE_ID = 2
    }
}
