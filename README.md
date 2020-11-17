# WinIPv4Backup
I needed an easy way to get or set the ipv4 settings of a network interface under windows and also be able to save those to a file.

Microsoft.Management.Infrastructure and System.Management are not only clunky but the also do not work on disconnected or disabled adapters.

My approach just uses System.Net.NetworkInformation to get some information about the available adapters, like their names, guid, mac address and also filters out virtual adapters.

I then use regedit to get or set the ipv4 registry values corresponding to the networkinterface, using the key:
Computer\HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\Tcpip\Parameters\Interfaces\ + the interfaces guid
Since using regedit requires admin privileges, running this does as well.

The gateway only properly sets after a restart of the adapter, which I do through netsh.

The only none microsoft tool/library used here is the Newtonsoft.Json library, which I use to save/load the ipv4 settings and some additional information to/from a Json file.

I´ve made a small wpf gui for testing.

Tested with Windows 7 x86 and Windows 10 x64 but it should work with Windows 8/8.1 as well. Published as framework independent, this worked with .net framework 3.5.

Feel free to re-use however you like
