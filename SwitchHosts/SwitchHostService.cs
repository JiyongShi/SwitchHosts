using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Net.NetworkInformation;
using System.Text.Json;

namespace SwitchHosts
{
    /// <summary>
    /// auto update system hosts(C:\Windows\System32\drivers\etc\hosts)'s hostName to special ip by check ip array reachable
    /// host.xxx.com: [ip1, ip2]
    /// ip array sorted priority desc.
    /// </summary>
    public class SwitchHostService
    {
        private readonly ILogger<SwitchHostService> _logger;

        private readonly HostConfiguration? _hostConfig;

        /// <summary>
        /// 
        /// </summary>
        [RequiresDynamicCode("JsonDeSerializer")]
        public SwitchHostService(ILogger<SwitchHostService> logger)
        {
            _logger = logger;
            var configStr = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "HostConfigs.json"));
            _hostConfig = JsonSerializer.Deserialize<HostConfiguration>(configStr, JsonSourceGenerationContext.Default.HostConfiguration);
        }

        /// <summary>
        /// /
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public string Execute()
        {
            lock (HostsWriteLock.LockObj)
            {
                string resultStr = string.Empty;
                if (_hostConfig == null || _hostConfig.Count == 0)
                {
                    return "HostConfigs.json is empty!";
                }

                try
                {
                    foreach (var kvItem in _hostConfig)
                    {
                        var hostName = kvItem.Key;
                        string? hostIP = null;
                        foreach (var ip in kvItem.Value)
                        {
                            if (IPReachable(ip))
                            {
                                hostIP = ip;
                                break;
                            }
                        }

                        if (string.IsNullOrEmpty(hostIP))
                        {
                            RemoveHostNameInHostsFile(hostName);
                            resultStr += $"remove host: {hostName}\r\n";
                        }
                        else
                        {
                            UpdateHostNameInHostsFile(hostName, hostIP);
                        }

                        resultStr += $"update host: {hostName} {hostIP}\r\n";
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
                return resultStr;
            }
        }

        private void UpdateHostNameInHostsFile(string hostName, string? hostIp)
        {
            const string sectionStartStr = "# Added by SwitchHosts service";
            const string sectionEndStr = "# End of SwitchHosts section";
            string toUpdateHostNameLine = $"{hostIp} {hostName}";

            var hostsAllLines = File.ReadAllLines(HostsWriteLock.HostsFilePath);
            var hostsAllLinesList = hostsAllLines.ToList();
            bool hostNameExits = false;
            for (int i = 0; i < hostsAllLinesList.Count; i++)
            {
                if (hostsAllLinesList[i].Contains(hostName))
                {
                    hostNameExits = true;

                    // update line from hosts
                    if (hostsAllLinesList[i] != toUpdateHostNameLine)
                    {
                        hostsAllLinesList[i] = $"{hostIp} {hostName}";
                    }
                }
            }

            // hostName not exits in system hosts, add it
            if (!hostNameExits)
            {
                if (!hostsAllLinesList.Contains(sectionStartStr))
                {
                    // not contains section, first add, will add section start and end descriptions
                    hostsAllLinesList.Add(sectionStartStr);
                    hostsAllLinesList.Add(toUpdateHostNameLine);
                    hostsAllLinesList.Add(sectionEndStr);
                }
                else
                {
                    var sectionEndIndex = hostsAllLinesList.LastIndexOf(sectionEndStr);
                    hostsAllLinesList.Insert(sectionEndIndex, toUpdateHostNameLine);
                }
            }

            if (!hostsAllLines.SequenceEqual(hostsAllLinesList))
            {
                File.WriteAllLines(HostsWriteLock.HostsFilePath, hostsAllLinesList);
            }
        }

        private void RemoveHostNameInHostsFile(string hostName)
        {
            var hostsAllLines = File.ReadAllLines(HostsWriteLock.HostsFilePath);
            var hostsAllLinesList = hostsAllLines.ToList();
            for (int i = 0; i < hostsAllLinesList.Count; i++)
            {
                if (hostsAllLinesList[i].Contains(hostName))
                {
                    // remove line from hosts
                    hostsAllLinesList.RemoveAt(i);
                }
            }

            if (!hostsAllLines.SequenceEqual(hostsAllLinesList))
            {
                File.WriteAllLines(HostsWriteLock.HostsFilePath, hostsAllLinesList);
            }
        }

        public bool IPReachable(string? ip)
        {
            if (ip == null)
            {
                return false;
            }

            try
            {
                Ping ping = new Ping();
                PingReply pingReply = ping.Send(ip);

                if (pingReply.Status == IPStatus.Success)
                {
                    //Server is alive
                    return true;
                }

                return false;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }
    }
}
