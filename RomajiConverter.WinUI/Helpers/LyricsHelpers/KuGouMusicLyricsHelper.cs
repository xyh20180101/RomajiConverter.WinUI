using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RomajiConverter.WinUI.Models;

namespace RomajiConverter.WinUI.Helpers.LyricsHelpers;

public class KuGouMusicLyricsHelper : LyricsHelper
{
    public static readonly Regex SongHashRegex = new("\"hash\":\"(?<songHash>.*?)\"", RegexOptions.Compiled);

    public static readonly Regex SongNameRegex = new("\"audio_name\":\"(?<songName>.*?)\"", RegexOptions.Compiled);

    public static readonly Regex TransRegex = new("\\[(?<trans>[^\\[\\]]*?)\\]", RegexOptions.Compiled);

    public static async Task<List<MultilingualLrc>> GetLrc(string url)
    {
        var httpClient = new HttpClient();

        // 获取歌曲名称,歌曲hash
        var response = await httpClient.GetAsync(url);
        if (response.StatusCode == HttpStatusCode.Redirect)
            response = await httpClient.GetAsync(response.Headers.Location?.AbsoluteUri);
        var content = await response.Content.ReadAsStringAsync();
        var songHash = SongHashRegex.Match(content).Groups["songHash"].Value;
        var songName = SongNameRegex.Match(content).Groups["songName"].Value;

        // 拼接参数
        var ticks = ((DateTime.UtcNow - new DateTime(1970, 1, 1)).Ticks / 1000).ToString();

        var key = "NVPh5oo715z5DIWAeQlhMDsWXXQV4hwt";
        var headers = new Dictionary<string, string>
        {
            { "clienttime", ticks },
            { "clientver", "20000" },
            { "dfid", "-" },
            { "hash", songHash },
            { "keyword", songName },
            { "mid", ticks },
            { "srcappid", "2919" },
            { "timelength", "209000" },
            { "uuid", ticks }
        };

        // 计算签名
        var signature = MD5Helper.GetMD5(key + string.Join("", headers.Select(p => $"{p.Key}={p.Value}")) + key);

        var query = string.Join("&", headers.Select(p => $"{p.Key}={p.Value}")) + $"&signature={signature}";

        JObject songInfo;
        var lrc = new List<MultilingualLrc>();
        try
        {
            var songInfoJson =
                await (await httpClient.GetAsync($"https://m3ws.kugou.com/api/v1/krc/get_lyrics?{query}"))
                    .Content
                    .ReadAsStringAsync();
            songInfo = JsonConvert.DeserializeObject<JObject>(songInfoJson);
            var jpnLrcText = (string)songInfo["data"]["lrc"];
            lrc = ParseLrc(jpnLrcText, string.Empty);
        }
        catch (Exception e)
        {
            throw new Exception(ResourceLoader.GetForViewIndependentUse().GetString("GetLyricsError"));
        }

        try
        {
            // 这个接口没有直接的中文lrc,而是提供了翻译数组
            var transString = (string)songInfo["data"]["landata"].FirstOrDefault(p => (int)p["type"] == 1)["content"];
            var matches = TransRegex.Matches(transString);

            for (var i = 0; i < lrc.Count; i++) lrc[i].CLrc = matches[i].Groups["trans"].Value.Trim();
        }
        catch (Exception a)
        {
            // 中文获取失败就忽略了
        }

        return lrc;
    }
}