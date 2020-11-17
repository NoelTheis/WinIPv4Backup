using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Text.RegularExpressions;

namespace WinIPv4Backup.Classes
{
    /// <summary>
    /// Wraps System.Net.NetworkInformation
    /// </summary>
    static class NetInfoWrapper
    {
        /// <summary>
        /// Gets a selection of property values from a NetworkInterface instance as KeyValuePairs consisting of a descriptive name and their value
        /// </summary>
        /// <param name="networkInterface"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        static public Dictionary<string, object> GetValues(NetworkInterface networkInterface)
        {
            if (networkInterface == null)
                throw new ArgumentNullException(nameof(networkInterface));

            Dictionary<string, object> keyValuePairs = new Dictionary<string, object>()
            {
                { "Name" ,networkInterface.Name },
                { "Description", networkInterface.Description },
                { "Status", networkInterface.OperationalStatus },
                { "ID", networkInterface.Id },
                { "MAC", FormatMACToString(networkInterface.GetPhysicalAddress())}
            };
            return keyValuePairs;
        }


        /// <summary>
        /// Formats values of a NetworkInterface instance to a string
        /// </summary>
        /// <param name="networkInterface"></param>
        /// <returns></returns>
        static public string FormatValuesToString(NetworkInterface networkInterface)
        {
            StringBuilder propertyText = new StringBuilder();

            foreach(KeyValuePair<string, object> keyValuePair in GetValues(networkInterface))
            {
                propertyText.AppendLine(FormatSingleValueToString(keyValuePair));
            }           

            return propertyText.ToString();
        }


        /// <summary>
        /// Formats a combination of name and a value object to an aligned string
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        static private string FormatSingleValueToString(KeyValuePair<string, object> keyValuePair)
        {
            if (keyValuePair.Key == null)
                throw new ArgumentNullException(nameof(keyValuePair));

            Func<string, string> LineFormat = (string value) =>
            {
                return String.Format("{0, -30} \t:\t {1,-30}", keyValuePair.Key, value);
            };

            if (keyValuePair.Value != null)
            {
                string? s = keyValuePair.Value.ToString();
                if (s != null)
                    return LineFormat(s);
            }

            return LineFormat("NULL");
        }


        /// <summary>
        /// Formats a PhysicalAddress instance to a readible MAC address
        /// </summary>
        /// <param name="physicalAddress"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        static public string FormatMACToString(PhysicalAddress physicalAddress)
        {
            if (physicalAddress == null)
                throw new ArgumentNullException(nameof(physicalAddress));
            
            byte[] AddressBytes = physicalAddress.GetAddressBytes();
            if (AddressBytes == null)
                return "NULL";

            if (AddressBytes.Length == 0)
                return "";

            StringBuilder sb = new StringBuilder();
            foreach (byte b in AddressBytes)
            {
                sb.Append(b.ToString("X2"));
                sb.Append(':');
            }
            sb.Remove(sb.Length - 1, 1);
            return sb.ToString();
        }


        /// <summary>
        /// Gets a list of all interfaces, filters them by lists of valid types and a regex of invalid descriptions
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <returns>returns all interfaces matching the type lists and not matching the regex</returns>
        static public List<NetworkInterface> GetNetworkInterfaces(List<NetworkInterfaceType>? validTypes, string? filterRegex) 
        {
            IEnumerable<NetworkInterface> validTypeNetworkInterfaces;
            if (validTypes == null)
                validTypeNetworkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
            else
                validTypeNetworkInterfaces = NetworkInterface.GetAllNetworkInterfaces().Where(n => validTypes.Contains(n.NetworkInterfaceType));

            if (filterRegex == null)
                return new List<NetworkInterface>(validTypeNetworkInterfaces);

            var filteredNetworkInterfaces = new List<NetworkInterface>();
            foreach (NetworkInterface n in validTypeNetworkInterfaces)
            {
                if(!Regex.IsMatch(n.Description, filterRegex, RegexOptions.IgnoreCase))
                    filteredNetworkInterfaces.Add(n);
            }

            return filteredNetworkInterfaces;
        }

    }
}
