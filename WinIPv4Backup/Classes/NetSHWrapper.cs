using System.Diagnostics;

namespace WinIPv4Backup.Classes
{
    /// <summary>
    /// Wraps netsh
    /// </summary>
    class NetSHWrapper
    {
        /// <summary>
        /// starts a new netsh process with: interface set interface "name" disable
        /// </summary>
        /// <param name="networkInterface"></param>
        /// <returns></returns>
        static public bool NetshDisableInterface(string interfaceName)
        {
            return Netsh("interface set interface \"" + interfaceName + "\" disable");
        }


        /// <summary>
        /// starts a new netsh process with: interface set interface "name" enable
        /// </summary>
        /// <param name="networkInterface"></param>
        /// <returns></returns>
        static public bool NetshEnableInterface(string interfaceName)
        {
            return Netsh("interface set interface \"" + interfaceName + "\" enable");
        }

        /// <summary>
        /// starts a new netsh process with the given argument
        /// </summary>
        /// <param name="networkInterface"></param>
        /// <returns></returns>
        static private bool Netsh(string argument)
        {
            Process process = new Process();
            process.StartInfo.FileName = "netsh";
            process.StartInfo.Arguments = argument;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            process.Start();
            if (process.WaitForExit(5000))
            {
                if (process.ExitCode == 0)
                    return true;
            }
            return false;
        }
    }
}
