using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SwitchHosts
{
    /// <summary>
    /// Configuration format:
    ///     domain
    ///         [ip1,ip2,...,ipN]
    /// </summary>
    public class HostConfiguration : Dictionary<string, string[]>
    {
    }
}
