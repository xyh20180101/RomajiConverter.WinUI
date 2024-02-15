using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RomajiConverter.WinUI.Models;

namespace RomajiConverter.WinUI.Helpers.LyricsHelpers;

public class QQMusicLyricsHelper : LyricsHelper
{
    public static readonly Regex SongIdRegex = new("\"songList\":\\[\\{\"id\":(?<songId>\\d*),", RegexOptions.Compiled);

    public static async Task<List<MultilingualLrc>> GetLrc(string url)
    {
        var httpClient = new HttpClient();

        // 获取歌曲Id
        var response = await httpClient.GetAsync(url);
        if (response.StatusCode == HttpStatusCode.Redirect)
            response = await httpClient.GetAsync(response.Headers.Location?.AbsoluteUri);
        var content = await response.Content.ReadAsStringAsync();
        var songId = long.Parse(SongIdRegex.Match(content).Groups["songId"].Value);

        // 拼接参数
        var requestBody = new
        {
            comm = new
            {
                cv = 4747474,
                ct = 24,
                format = "json",
                inCharset = "utf-8",
                outCharset = "utf-8",
                notice = 0,
                platform = "yqq.json",
                needNewCode = 1,
                uin = 0,
                g_tk_new_20200303 = 527922476,
                g_tk = 527922476
            },
            req_1 = new
            {
                module = "music.musichallSong.PlayLyricInfo",
                method = "GetPlayLyricInfo",
                param = new
                {
                    songID = songId,
                    trans = 1
                }
            }
        };

        try
        {
            var songInfoJson = await (await httpClient.PostAsync("https://u.y.qq.com/cgi-bin/musicu.fcg",
                    new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8))).Content
                .ReadAsStringAsync();
            var songInfo = JsonConvert.DeserializeObject<JObject>(songInfoJson);
            var jpnLrcText =
                Encoding.UTF8.GetString(Convert.FromBase64String((string)songInfo["req_1"]["data"]["lyric"]));
            var chnLrcText =
                Encoding.UTF8.GetString(Convert.FromBase64String((string)songInfo["req_1"]["data"]["trans"]));
            chnLrcText = chnLrcText.Replace("//", string.Empty); //qq的歌词空行是"//"

            var lrc = ParseLrc(jpnLrcText, chnLrcText);
            return lrc;
        }
        catch (Exception e)
        {
            throw new Exception(ResourceLoader.GetForViewIndependentUse().GetString("GetLyricsError"));
        }
    }
}