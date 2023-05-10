using System.Threading.Tasks;
using System.Net.Http;
using System;
using Steamworks;
using System.Linq;
using Hikaria.GTFO_Anti_Cheat.Lang;
using System.Threading;

namespace Hikaria.GTFO_Anti_Cheat.Utils
{
    internal static class HttpHelper
    {
        public static async Task<string[]> Get(string url)
        {
            HttpClient client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(10); //设置10秒超时
            HttpResponseMessage response = await client.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                string content = await response.Content.ReadAsStringAsync();
                string[] splitContent = content.Split('\n');
                if (splitContent[0] != "404: Not Found")
                {
                    return splitContent;
                }
                return null;
            }
            return null;
        }

        public static void CheckUpdate()
        {
            new Task(delegate ()
            {
                GameEventLogManager.AddLog(EntryPoint.Language.CHECKING_UPDATE);

                string[] content = Get(UpdateURL).Result;

                if (content == null)
                {
                    GameEventLogManager.AddLog(EntryPoint.Language.CHECK_UPDATE_FAILD);
                    return;
                }

                string[] newVersion = content[0].Split(',');

                if (Convert.ToInt32(newVersion[0]) > Convert.ToInt32(LatestInternalVersion))
                {
                    LatestInternalVersion = newVersion[0];
                    LatestVersion = newVersion[1];
                    string changeLog;
                    if (!EntryPoint.IsEnglish)
                    {
                        changeLog = content[1];
                    }
                    else
                    {
                        changeLog = content[2];
                    }
                    
                    GameEventLogManager.AddLog(string.Format(EntryPoint.Language.NEW_VERSION_DETECTED, LatestVersion));
                    GameEventLogManager.AddLog(string.Format(EntryPoint.Language.CHANGE_LOG));
                    GameEventLogManager.AddLogInSplit(changeLog, '|');
                    return;
                }

                GameEventLogManager.AddLog(string.Format(EntryPoint.Language.IS_LATEST_VERSION, LatestVersion));
            }).Start();
        }

        private static string LatestVersion = PluginInfo.PLUGIN_VERSION;

        private static string LatestInternalVersion = PluginInfo.INTERNAL_VERSION;

        private static readonly string UpdateURL = "https://raw.githubusercontent.com/Hikaria0108/GTFO-Anti-Cheat/main/latestversion.txt";
    }
}
