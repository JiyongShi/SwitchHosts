using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwitchHosts
{
    internal class HostsWriteLock
    {
        public static string HostsFilePath = "C:\\Windows\\System32\\drivers\\etc\\hosts";

        public static object LockObj = new object();
    }
}
