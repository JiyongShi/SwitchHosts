namespace SwitchHosts
{
    public class GithubHostsService
    {
        private static string _gitHubHostsUpdateUrl = "https://hosts.gitcdn.top/hosts.txt";
        private static HttpClient _httpClient = new HttpClient();

        private readonly ILogger<GithubHostsService> _logger;

        /// <summary>
        /// 
        /// </summary>
        public GithubHostsService(ILogger<GithubHostsService> logger)
        {
            _logger = logger;
        }

        public string Execute()
        {
            lock (HostsWriteLock.LockObj)
            {
                var githubHostsContentLines = GetGithubHostsContent(_gitHubHostsUpdateUrl);

                UpdateHostNameInHostsFile(githubHostsContentLines);

                return string.Join(Environment.NewLine, githubHostsContentLines);
            }
        }

        public List<string> GetGithubHostsContent(string url)
        {
            try
            {
                var getTask = _httpClient.GetStringAsync(url);
                getTask.Wait(TimeSpan.FromSeconds(30));
                if (getTask.IsCompletedSuccessfully)
                {
                    var getResult = getTask.Result;
                    return getResult.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                _logger.LogError("GetGithubHostsContent: " + ex.Message + ex.StackTrace);
            }

            return new List<string>();
        }

        private void UpdateHostNameInHostsFile(List<string> githubHostsContentFromUrl)
        {
            if (githubHostsContentFromUrl.Count == 0)
            {
                return;
            }

            const string sectionStartStr = "# Added github hosts by SwitchHosts service";
            const string sectionEndStr = "# End github hosts of SwitchHosts section";

            var hostsAllLines = File.ReadAllLines(HostsWriteLock.HostsFilePath);
            var hostsAllLinesList = hostsAllLines.ToList();

            // get subList of github hosts lines
            var githubHostsLineStartIndex = hostsAllLinesList.FindIndex(t => t.StartsWith(sectionStartStr));
            var githubHostsLineEndIndex = hostsAllLinesList.FindIndex(t => t.StartsWith(sectionEndStr));
            var githubHostsLines = githubHostsLineStartIndex == -1
                ? new List<string>()
                : hostsAllLinesList.GetRange(
                githubHostsLineStartIndex,
                githubHostsLineEndIndex - githubHostsLineStartIndex + 1);

            githubHostsContentFromUrl.Insert(0, sectionStartStr);
            githubHostsContentFromUrl.Add(sectionEndStr);
            if (githubHostsLines.SequenceEqual(githubHostsContentFromUrl))
            {
                _logger.LogInformation("UpdateHostNameInHostsFile content no change, not modify hosts now.");
                return;
            }

            // content changed, update hosts file.
            if (githubHostsLineStartIndex > 0
                && githubHostsLineEndIndex > 0)
            {
                hostsAllLinesList.RemoveRange(githubHostsLineStartIndex,
                    githubHostsLineEndIndex - githubHostsLineStartIndex + 1);
            }

            hostsAllLinesList.AddRange(githubHostsContentFromUrl);

            // check hosts file content changed again.
            if (!hostsAllLines.SequenceEqual(hostsAllLinesList))
            {
                File.WriteAllLines(HostsWriteLock.HostsFilePath, hostsAllLinesList);
            }
        }
    }
}
