using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;

namespace WinIPv4Backup.Classes
{
    /// <summary>
    /// This class encapsulates the IPv4 registry entries and select information of a network interface, and provides methods to write/read from/to file
    /// </summary>
    class IPv4JsonFile
    {
        /// <summary>
        /// Encapsulates what should be written to the file
        /// </summary>
        private class FileContent
        {
            [JsonProperty]
            public Dictionary<string, object> NetworkInterfaceInfo { get; private set; }
            [JsonProperty]
            public IPv4RegistryEntries Ipv4RegistryEntries { get; private set; }

            public FileContent(Dictionary<string, object> networkInterfaceInfo, IPv4RegistryEntries ipv4RegistryEntries)
            {
                this.NetworkInterfaceInfo = networkInterfaceInfo;
                this.Ipv4RegistryEntries = ipv4RegistryEntries;
            }
        }

        public Dictionary<string, object> NetworkInterfaceInfo { get; private set; }
        public IPv4RegistryEntries Ipv4RegistryEntries { get; private set; }


        #region constructors
        /// <summary>
        /// 
        /// </summary>
        /// <param name="networkInterfaceInfo"></param>
        /// <param name="ipv4RegistryEntries"></param>
        public IPv4JsonFile(Dictionary<string, object> networkInterfaceInfo, IPv4RegistryEntries ipv4RegistryEntries)
        {
            NetworkInterfaceInfo = networkInterfaceInfo;
            this.Ipv4RegistryEntries = ipv4RegistryEntries;
        }


        /// <summary>
        /// Loads ipv4 registry entries and interface info from a file
        /// </summary>
        /// <param name="fileInfo"></param>
        #nullable disable //attributes are either set in load method or exception is thrown
        public IPv4JsonFile(FileInfo fileInfo)
        {
            LoadFromFile(fileInfo);
        }
        #nullable restore

        #endregion


        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileInfo"></param>
        public void SaveToFile(FileInfo fileInfo)
        {
            FileContent fileContent = new FileContent(NetworkInterfaceInfo, Ipv4RegistryEntries);

            string data = JsonConvert.SerializeObject(fileContent, Formatting.Indented);

            File.WriteAllText(fileInfo.FullName, data);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileInfo"></param>
        public void LoadFromFile(FileInfo fileInfo)
        {
            string data = File.ReadAllText(fileInfo.FullName);

            FileContent? fileContent = JsonConvert.DeserializeObject<FileContent>(data, new JsonSerializerSettings
            {
                MissingMemberHandling = MissingMemberHandling.Ignore
            });

            if (fileContent == null)
                throw new JsonException(fileInfo.FullName);
            if (fileContent.NetworkInterfaceInfo == null)
                throw new JsonException(nameof(fileContent.NetworkInterfaceInfo));
            if (fileContent.Ipv4RegistryEntries == null)
                throw new JsonException(nameof(fileContent.Ipv4RegistryEntries));

            NetworkInterfaceInfo = fileContent.NetworkInterfaceInfo;
            Ipv4RegistryEntries = fileContent.Ipv4RegistryEntries;

        }
    }
}
