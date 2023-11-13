using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Opportunity.LrcParser;

namespace RomajiConverter.WinUI.Helpers
{
    public static class CloudMusicHelper
    {
        public static string HistoryPath { get; set; }

        public static void Init()
        {
            HistoryPath = $"{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}\\AppData\\Local\\Netease\\CloudMusic\\webdata\\file\\history";
        }

        public static string GetLastSongId()
        {
            var history = JArray.Parse(File.ReadAllText(HistoryPath));
            return history[0]["track"]["id"].ToString();
        }

        public static async Task<List<ReturnLrc>> GetLrc(string songId)
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri($"http://music.163.com/");
            var jpnLrcResponse = await client.GetAsync($"api/song/media?id={songId}");
            var content = JObject.Parse(await jpnLrcResponse.Content.ReadAsStringAsync());
            var jpnLrc = Lyrics.Parse(content["lyric"].ToString());
            var chnLrcResponse = await client.GetAsync($"api/song/lyric?os=pc&id={songId}&tv=-1");
            content = JObject.Parse(await chnLrcResponse.Content.ReadAsStringAsync());
            if ((int?) content["code"] != 200)
                throw new Exception("获取歌词出错");
            var chnLrc = Lyrics.Parse(content["tlyric"]["lyric"].ToString());
            var lrcList = jpnLrc.Lyrics.Lines.Select(line => new ReturnLrc { Time = line.Timestamp.ToString("mm:ss.fff"), JLrc = line.Content }).ToList();
            foreach (var line in chnLrc.Lyrics.Lines)
            {
                foreach (var lrc in lrcList.Where(lrc => lrc.Time == line.Timestamp.ToString("mm:ss.fff")))
                {
                    lrc.CLrc = line.Content;
                }
            }
            return lrcList;
        }
    }

    public class ReturnLrc
    {
        /// <summary>
        /// 时间
        /// </summary>
        public string Time { get; set; }

        /// <summary>
        /// 日词
        /// </summary>
        public string JLrc { get; set; }

        /// <summary>
        /// 中词
        /// </summary>
        public string CLrc { get; set; }

        public ReturnLrc()
        {
            Time = "";
            JLrc = "";
            CLrc = "";
        }
    }
}
