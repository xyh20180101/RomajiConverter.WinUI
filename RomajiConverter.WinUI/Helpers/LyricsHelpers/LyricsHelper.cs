using System;
using System.Collections.Generic;
using System.Linq;
using Opportunity.LrcParser;
using RomajiConverter.WinUI.Models;

namespace RomajiConverter.WinUI.Helpers.LyricsHelpers;

public class LyricsHelper
{
    public static List<MultilingualLrc> ParseLrc(string jpnLrcText, string chnLrcText)
    {
        if (App.Config.IsUseOldLrcParser)
        {
            var jpnLrc = Lyrics.Parse(jpnLrcText);
            var chnLrc = Lyrics.Parse(chnLrcText);

            var lrcList = jpnLrc.Lyrics.Lines.Select(line => new MultilingualLrc
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

            var lrcList = jpnLrc.Select(line => new MultilingualLrc
                { Time = line.Time, JLrc = line.Text }).ToList();
            foreach (var line in chnLrc)
            foreach (var lrc in lrcList.Where(lrc => lrc.Time == line.Time))
                lrc.CLrc = line.Text;

            return lrcList;
        }
    }
}