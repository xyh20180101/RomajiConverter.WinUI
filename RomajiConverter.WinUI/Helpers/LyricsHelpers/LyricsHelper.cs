using System.Collections.Generic;
using System.Linq;
using RomajiConverter.WinUI.Models;

namespace RomajiConverter.WinUI.Helpers.LyricsHelpers;

public class LyricsHelper
{
    public static List<MultilingualLrc> ParseLrc(string jpnLrcText, string chnLrcText)
    {
        var jpnLrc = LrcParser.Parse(jpnLrcText);
        var chnLrc = LrcParser.Parse(chnLrcText);

        var lrcList = jpnLrc.Select(line => new MultilingualLrc
            { Time = line.Time, JLrc = line.Text }).ToList();
        foreach (var line in chnLrc)
        foreach (var lrc in lrcList.Where(lrc => lrc.Time == line.Time))
            lrc.CLrc = line.Text;

        return lrcList;
    }
}