using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Text;

namespace WinIPv4Backup.Classes
{
    /// <summary>
    /// This class encapsulates the IPv4 registry entries of a network interface and provides methods for loading from or saving to the registry
    /// </summary>
    /// <remarks>
    /// field types reflect the value types in the registry (Int32: DWORD, String[]: REG_MULTI_SZ, String: REG_SZ.
    /// fields are nullable, to distinguish between empty(or zero) values and missing values.
    /// 
    /// The intention is to set values as they were found, e.g. an IPAddress that was empty during backup should be set as [""] when restoring.
    /// A value that was missing during backup should be deleted(if existing) during restore.
    /// </remarks>
    public class IPv4RegistryEntries
    {
        static private readonly string interfacesKey = @"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\Tcpip\Parameters\Interfaces";

        [JsonProperty]
        public Int32? EnableDHCP { get; private set; }
        [JsonProperty]
        public string[]? IPAddress { get; private set; }
        [JsonProperty]
        public string[]? DefaultGateway { get; private set; }
        [JsonProperty]
        public string? NameServer { get; private set; }
        [JsonProperty]
        public string[]? SubnetMask { get; private set; }


        /// <summary>
        /// Sets all fields to null
        /// </summary>
        public IPv4RegistryEntries()
        {
            EnableDHCP = null;
            IPAddress = null;
            DefaultGateway = null;
            NameServer = null;
            SubnetMask = null;
        }


        /// <summary>
        /// Formats the entries to a text, for purpose of displaying them to a user
        /// </summary>
        /// <returns></returns>
        public string FormatEntriesToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(FormatEntryToString("DHCP", EnableDHCP));
            sb.AppendLine(FormatEntryToString("IP Address", IPAddress));
            sb.AppendLine(FormatEntryToString("Gateway", DefaultGateway));
            sb.AppendLine(FormatEntryToString("DNS", NameServer));
            sb.AppendLine(FormatEntryToString("Subnet", SubnetMask));
            return sb.ToString();
        }


        /// <summary>
        /// Formats a entry field to a string
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        static private string FormatEntryToString(string name, object? value)
        {
            Func<string, string> FormatLine = (string funcValue) =>
            {
                return String.Format("{0, -30} \t:\t {1,-30}", name, funcValue);
            };

            if (value == null)
                return FormatLine("NULL");

            if (value.GetType().IsArray)
            {
                object[] array = (object[])value;
                StringBuilder sb = new StringBuilder();
                foreach (object o in array)
                {
                    sb.Append(o.ToString());
                }
                return FormatLine(sb.ToString());
            }

            string? s = value.ToString();
            if(s == null)
                return FormatLine("NULL");
            return FormatLine(s);
        }


        /// <summary>
        /// Trys to get the registry key (comprised of the standard interface key and the provided guid) and loads the values from there
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public void LoadFromRegisty(string guid)
        {
            if (guid == null)
                throw new ArgumentNullException(nameof(guid));

            using RegistryKey key = GetKey(interfacesKey + @"\" + guid.ToLower());
            EnableDHCP = (Int32?)key.GetValue("EnableDHCP", null);
            IPAddress = (string[]?)key.GetValue("IPAddress", null);
            DefaultGateway = (string[]?)key.GetValue("DefaultGateway", null);
            NameServer = (string?)key.GetValue("NameServer", null);
            SubnetMask = (string[]?)key.GetValue("SubnetMask", null);
        }


        /// <summary>
        /// Trys to get the registry key (comprised of the standard interface key and the provided guid) and applies the values to it
        /// </summary>
        /// <param name="guid"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public void SaveToRegistry(string guid)
        {
            if (guid == null)
                throw new ArgumentNullException(nameof(guid));

            using RegistryKey key = GetKey(interfacesKey + @"\" + guid.ToLower());
            SetRegistryValue(key, "EnableDHCP", EnableDHCP, RegistryValueKind.DWord);
            SetRegistryValue(key, "IPAddress", IPAddress, RegistryValueKind.MultiString);
            SetRegistryValue(key, "DefaultGateway", DefaultGateway, RegistryValueKind.MultiString);
            SetRegistryValue(key, "NameServer", NameServer, RegistryValueKind.String);
            SetRegistryValue(key, "SubnetMask", SubnetMask, RegistryValueKind.MultiString);
        }


        /// <summary>
        /// Sets a registry value or deletes it if the provided value object is null
        /// </summary>
        /// <param name="key"></param>
        /// <param name="valuename"></param>
        /// <param name="value"></param>
        /// <param name="valueKind"></param>
        /// <exception cref="ArgumentNullException"></exception>
        static private void SetRegistryValue(RegistryKey key, string valuename, object? value, RegistryValueKind valueKind)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));
            if (valuename == null)
                throw new ArgumentNullException(nameof(valuename));

            if (value != null)
                key.SetValue(valuename, value, valueKind);
            else
                key.DeleteValue(valuename, false);
        }


        /// <summary>
        /// Gets the RegistryKey described by the key string, if found
        /// </summary>
        /// <param name="keyDescription"></param>
        /// <returns>RegistryKey if found, else null</returns>
        /// <exception cref="ArgumentNullException"></exception>
        static private RegistryKey GetKey(string keyDescription)
        {
            if (keyDescription == null)
                throw new ArgumentNullException(nameof(keyDescription));

            RegistryKey? registryKey = null;
            if (keyDescription.Contains("HKEY_LOCAL_MACHINE"))
            {
                string subKey = keyDescription.Split("HKEY_LOCAL_MACHINE").Last().Trim('\\'); //removes HKLM and any leading or tailing '\'
                registryKey = Registry.LocalMachine.OpenSubKey(subKey, true);


            }

            if (registryKey == null)
                throw new Exception(keyDescription + " does not exist");

            return registryKey;
        }
    }


}
