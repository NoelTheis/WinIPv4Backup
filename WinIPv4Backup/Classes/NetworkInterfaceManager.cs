using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;

namespace WinIPv4Backup.Classes
{
    /// <summary>
    /// This class is simply supposed to provide methods for easy useage of IPv4RegistryEntries, IPv4JsonFile,NetSHWrapper and NetInfoWrapper, by calling their methods in a logical order.
    /// </summary>
    class NetworkInterfaceManager
    {
        #region constants
        static private readonly List<NetworkInterfaceType> ethernetTypes = new List<NetworkInterfaceType>()
        {
            NetworkInterfaceType.Ethernet,
            NetworkInterfaceType.Ethernet3Megabit,
            NetworkInterfaceType.FastEthernetFx,
            NetworkInterfaceType.GigabitEthernet
        };
        static private readonly List<NetworkInterfaceType> wirelessTypes = new List<NetworkInterfaceType>()
        {
            NetworkInterfaceType.Wireless80211,
        };
        static private readonly List<NetworkInterfaceType> broadBandTypes = new List<NetworkInterfaceType>()
        {
            NetworkInterfaceType.Wman,
            NetworkInterfaceType.Wwanpp,
            NetworkInterfaceType.Wwanpp2
        };

        static private readonly string descriptionFilterRegex = @"virtual|(-|_)?\bvpn\b(-|_)?|hyper-v|bluetooth";
        #endregion 

        /// <summary>
        /// Posts select properties of a NetworkInterface instance
        /// </summary>
        /// <param name="networkInterface"></param>
        /// <returns>returns the post as string</returns>
        /// <exception cref="ArgumentNullException"></exception>
        static public string FormatInterfaceToString(NetworkInterface networkInterface)
        {
            if (networkInterface == null)
                throw new ArgumentNullException(nameof(networkInterface));

            StringBuilder sb = new StringBuilder();

            sb.Append(NetInfoWrapper.FormatValuesToString(networkInterface));

            var registryEntries = new IPv4RegistryEntries();
            registryEntries.LoadFromRegisty(networkInterface.Id);
            sb.Append(registryEntries.FormatEntriesToString());

            return sb.ToString();
        }


        /// <summary>
        /// Gets the ipv4 registryEntries of a NetworkInterface instance and saves them to the specified file
        /// </summary>
        /// <param name="networkInterface"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        static public void SaveIPv4ToFile(NetworkInterface networkInterface, FileInfo fileInfo)
        {
            if (networkInterface == null)
                throw new ArgumentNullException(nameof(networkInterface));

            var registryEntries = new IPv4RegistryEntries();
            registryEntries.LoadFromRegisty(networkInterface.Id);

            var ipv4json = new IPv4JsonFile(NetInfoWrapper.GetValues(networkInterface), registryEntries);
            ipv4json.SaveToFile(fileInfo);
        }


        /// <summary>
        /// Gets the ipv4 registryEntries of a NetworkInterface instance and saves them to the specified file
        /// </summary>
        /// <param name="networkInterface"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        static public void LoadIPv4FromFile(NetworkInterface networkInterface, FileInfo fileInfo)
        {
            if (networkInterface == null)
                throw new ArgumentNullException(nameof(networkInterface));

            var ipv4json = new IPv4JsonFile(fileInfo);
            ipv4json.Ipv4RegistryEntries.SaveToRegistry(networkInterface.Id);

            TryRestart(networkInterface);
        }


        /// <summary>
        /// tries to restart the network interface
        /// </summary>
        /// <param name="networkInterface"></param>
        /// <returns></returns>
        static private bool TryRestart(NetworkInterface networkInterface)
        {
            if (NetSHWrapper.NetshDisableInterface(networkInterface.Name))
            {
                Thread.Sleep(500);
                if (NetSHWrapper.NetshEnableInterface(networkInterface.Name))
                    return true;
            }
            return false;
        }


        /// <summary>
        /// Gets a list of all network interfaces that 1.match any of the type lists and 2.don´t match the filter regex
        /// </summary>
        /// <returns></returns>
        static public List<NetworkInterface> GetNetworkInterfaceList()
        {
            List<NetworkInterfaceType> validTypes = new List<NetworkInterfaceType>(ethernetTypes.Count + wirelessTypes.Count + broadBandTypes.Count);
            validTypes.AddRange(ethernetTypes);
            validTypes.AddRange(wirelessTypes);
            validTypes.AddRange(broadBandTypes);

            return NetInfoWrapper.GetNetworkInterfaces(validTypes, descriptionFilterRegex);
        }
    }
}
