using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using Microsoft.Data.Sqlite;
using Newtonsoft.Json.Linq;
using Opportunity.LrcParser;

namespace RomajiConverter.WinUI.Helpers;

public static class CloudMusicHelper
{
    /// <summary>
    /// 旧版本的历史文件路径
    /// </summary>
    public static string HistoryPath { get; set; }

    /// <summary>
    /// 3.0版本的历史文件路径
    /// </summary>
    public static string New3ClientHistoryPath { get; set; }

    public static void Init()
    {
        HistoryPath =
            $"{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}\\AppData\\Local\\Netease\\CloudMusic\\webdata\\file\\history";
        New3ClientHistoryPath =
            $"{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}\\AppData\\Local\\Netease\\CloudMusic\\Library\\webdb.dat";
    }

    public static string GetLastSongId()
    {
        try
        {
            //3.0版本获取songId方法
            using var connection = new SqliteConnection($"Data Source={New3ClientHistoryPath}");
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = @"SELECT id FROM historyTracks ORDER BY playtime DESC LIMIT 1";

            using var reader = command.ExecuteReader();
            reader.Read();
            var id = reader.GetString(0);
            return id;
        }
        catch (Exception e)
        {
            //旧版本获取songId方法
            var history = JArray.Parse(File.ReadAllText(HistoryPath));
            return history[0]["track"]["id"].ToString();
        }
    }

    public static async Task<List<ReturnLrc>> GetLrc(string songId)
    {
        var client = new HttpClient();
        client.BaseAddress = new Uri("http://music.163.com/");
        var jpnLrcResponse = await client.GetAsync($"api/song/media?id={songId}");
        var content = JObject.Parse(await jpnLrcResponse.Content.ReadAsStringAsync());
        var jpnLrcText = content["lyric"].ToString();

        var chnLrcResponse = await client.GetAsync($"api/song/lyric?os=pc&id={songId}&tv=-1");
        content = JObject.Parse(await chnLrcResponse.Content.ReadAsStringAsync());
        if ((int?)content["code"] != 200)
        {
            var resourceLoader = ResourceLoader.GetForViewIndependentUse();
            throw new Exception(resourceLoader.GetString("GetLyricsError"));
        }
        var chnLrcText = content["tlyric"]["lyric"].ToString();

        return ParseLrc(jpnLrcText, chnLrcText);
    }

    private static List<ReturnLrc> ParseLrc(string jpnLrcText, string chnLrcText)
    {
        if (App.Config.IsUseOldLrcParser)
        {
            var jpnLrc = Lyrics.Parse(jpnLrcText);
            var chnLrc = Lyrics.Parse(chnLrcText);

            var lrcList = jpnLrc.Lyrics.Lines.Select(line => new ReturnLrc
            { Time = line.Timestamp - DateTime.MinValue, JLrc = line.Content }).ToList();
            foreach (var line in chnLrc.Lyrics.Lines)
                foreach (var lrc in lrcList.Where(lrc => lrc.Time == line.Timestamp - DateTime.MinValue))
                    lrc.CLrc = line.Content;

            return lrcList;
        }
        else
        {
            var jpnLrc = LrcParser.Parse(jpnLrcText);
            var chnLrc = LrcParser.Parse(chnLrcText);

            var lrcList = jpnLrc.Select(line => new ReturnLrc
            { Time = line.Time, JLrc = line.Text }).ToList();
            foreach (var line in chnLrc)
                foreach (var lrc in lrcList.Where(lrc => lrc.Time == line.Time))
                    lrc.CLrc = line.Text;

            return lrcList;
        }
    }
}

public class ReturnLrc
{
    public ReturnLrc()
    {
        Time = TimeSpan.Zero;
        JLrc = "";
        CLrc = "";
    }

    /// <summary>
    /// 时间
    /// </summary>
    public TimeSpan Time { get; set; }

    /// <summary>
    /// 日词
    /// </summary>
    public string JLrc { get; set; }

    /// <summary>
    /// 中词
    /// </summary>
    public string CLrc { get; set; }
}