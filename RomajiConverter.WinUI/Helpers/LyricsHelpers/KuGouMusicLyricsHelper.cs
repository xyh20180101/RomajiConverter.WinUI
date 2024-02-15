using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RomajiConverter.WinUI.Models;

namespace RomajiConverter.WinUI.Helpers.LyricsHelpers;

public static class KuGouMusicLyricsHelper
{
    public static readonly Regex
        SongIdRegex = new("\"encode_album_audio_id\":\"(?<songId>.*?)\"", RegexOptions.Compiled);

    public static readonly Regex UnicodeRegex = new Regex(@"\\u([0-9a-fA-F]{4})", RegexOptions.Compiled);

    public static async Task<List<MultilingualLrc>> GetLrc(string url)
    {
        var httpClientHandler = new HttpClientHandler { AllowAutoRedirect = true };
        var httpClient = new HttpClient(httpClientHandler);

        // 获取歌曲id
        var response = await httpClient.GetAsync(url);
        if (response.StatusCode == HttpStatusCode.Redirect)
        {
            response = await httpClient.GetAsync(response.Headers.Location?.AbsoluteUri);
        }
        var content = await response.Content.ReadAsStringAsync();
        var match = SongIdRegex.Match(content);
        var songId = match.Groups["songId"].Value;

        // 获取歌词
        var ticks = (DateTime.UtcNow - new DateTime(1970, 1, 1)).Ticks / 10000;

        var key = "NVPh5oo715z5DIWAeQlhMDsWXXQV4hwt";
        var headers = new Dictionary<string, string>
        {
            { "appid", "1014" },
            { "clienttime", ticks.ToString() },
            { "clientver", "20000" },
            { "dfid", "2UHq9y3tQKsE0XjULn0MCRoh" },
            { "encode_album_audio_id", songId },
            { "mid", "b8cb3d9fa1fc4871348c0e436aac8cfe" },
            { "platid", "4" },
            { "srcappid", "2919" },
            { "token", "" },
            { "userid", "0" },
            { "uuid", "b8cb3d9fa1fc4871348c0e436aac8cfe" }
        };
        var signature = MD5Helper.GetMD5(key + string.Join("", headers.Select(p => $"{p.Key}={p.Value}")) + key);

        var query = string.Join("&", headers.Select(p => $"{p.Key}={p.Value}")) + $"&signature={signature}";

        var songInfoJson = await (await httpClient.GetAsync($"https://wwwapi.kugou.com/play/songinfo?{query}")).Content
            .ReadAsStringAsync();
        var songInfo = JsonConvert.DeserializeObject<JObject>(songInfoJson);
        var jpnLrcText = UnicodeRegex.Replace((string)songInfo["data"]["lyrics"],
            m => ((char)int.Parse(m.Groups[1].Value, System.Globalization.NumberStyles.HexNumber)).ToString());

        var lrc = LrcParser.Parse(jpnLrcText);
        return lrc.Select(line => new MultilingualLrc { Time = line.Time, JLrc = line.Text }).ToList();
    }
}