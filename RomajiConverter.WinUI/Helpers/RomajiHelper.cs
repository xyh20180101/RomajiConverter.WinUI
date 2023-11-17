using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using MeCab;
using RomajiConverter.WinUI.Extensions;
using RomajiConverter.WinUI.Models;
using WanaKanaSharp;

namespace RomajiConverter.WinUI.Helpers;

public static class RomajiHelper
{
    /// <summary>
    /// 分词器
    /// </summary>
    private static MeCabTagger _tagger;

    /// <summary>
    /// 自定义词典<原文, 假名>
    /// </summary>
    private static Dictionary<string, string> _customizeDict;

    public static void Init()
    {
        //词典路径
        var dicPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "unidic");
        var parameter = new MeCabParam
        {
            DicDir = dicPath,
            LatticeLevel = MeCabLatticeLevel.Zero
        };
        _tagger = MeCabTagger.Create(parameter);

        var str = File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "customizeDict.txt"));
        var list = str.Split(Environment.NewLine);
        _customizeDict = new Dictionary<string, string>();
        foreach (var item in list)
        {
            if (string.IsNullOrWhiteSpace(item)) continue;
            var array = item.Split(" ");
            if (array.Length < 2) continue;
            if (!_customizeDict.ContainsKey(array[0]))
                _customizeDict.Add(array[0], array[1]);
        }
    }

    /// <summary>
    /// 生成转换结果列表
    /// </summary>
    /// <param name="text"></param>
    /// <param name="isAutoVariant"></param>
    /// <param name="chineseRate"></param>
    /// <returns></returns>
    public static List<ConvertedLine> ToRomaji(string text, bool isAutoVariant = false, float chineseRate = 1f)
    {
        var lineTextList = text.RemoveEmptyLine().Split(Environment.NewLine);

        var convertedText = new List<ConvertedLine>();


        for (var index = 0; index < lineTextList.Length; index++)
        {
            var line = lineTextList[index];

            var convertedLine = new ConvertedLine();

            if (IsChinese(line, chineseRate)) continue;

            convertedLine.Japanese = line.Replace("\0", ""); //文本中如果包含\0，会导致复制只能粘贴到第一个\0处，需要替换为空，以下同理

            var sentences = line.LineToUnits(); //将行拆分为分句
            var multiUnits = new List<ConvertedUnit[]>();
            foreach (var sentence in sentences)
            {
                if (IsEnglish(sentence))
                {
                    multiUnits.Add(new[] { new ConvertedUnit(sentence, sentence, sentence, false) });
                    continue;
                }

                var units = SentenceToRomaji(sentence);

                //变体处理
                if (isAutoVariant)
                {
                    var regex = new Regex("[^a-zA-Z0-9 ]");

                    var romajis = string.Join("", units.Select(p => p.Romaji)); //整个句子的罗马音

                    var hanMatches = regex.Matches(romajis);
                    if (hanMatches.Any(p => p.Success)) //判断这个句子翻译成罗马音后是否有非英文字符，有就尝试替换变体后再翻译
                    {
                        var tempSentence = sentence; //原来的句子
                        var tempRomaji = romajis; //原来的句子的罗马音
                        foreach (Match match in hanMatches)
                        {
                            if (match.Success == false) continue; //遍历非英文字符

                            tempSentence =
                                tempSentence.Replace(match.Value, VariantHelper.GetVariant(match.Value)); //尝试替换后的句子
                            convertedLine.Japanese =
                                convertedLine.Japanese.Replace(match.Value,
                                    VariantHelper.GetVariant(match.Value)); //顺便更新一下这个字段

                            tempRomaji =
                                string.Join("", SentenceToRomaji(tempSentence).Select(p => p.Romaji)); //尝试替换后的句子的罗马音
                            var tempHanMatches = regex.Matches(tempRomaji);
                            if (tempHanMatches.Any(p => p.Success) ==
                                false) //如果这时罗马音已经全英文了，说明这个尝试替换后的句子是没问题的，可以break了；如果还没全英文，就继续替换下一个字符
                                break;
                        }

                        units = SentenceToRomaji(tempSentence);
                    }
                }

                multiUnits.Add(units);
            }

            convertedLine.Units = multiUnits.SelectMany(p => p).ToArray();

            if (index + 1 < lineTextList.Length &&
                IsChinese(lineTextList[index + 1], chineseRate))
                convertedLine.Chinese = lineTextList[index + 1];

            convertedLine.Index = convertedText.Count;
            convertedText.Add(convertedLine);
        }

        return convertedText;
    }

    /// <summary>
    /// 判断字符串(句子)是否简体中文
    /// </summary>
    /// <param name="str"></param>
    /// <param name="rate">容错率(0-1)</param>
    /// <returns></returns>
    public static bool IsChinese(string str, float rate)
    {
        if (str.Length < 2)
            return false;

        var wordArray = str.ToCharArray();

        var total = wordArray.Length;

        var chCount = 0f;

        var enCount = 0f;

        foreach (var word in wordArray)
        {
            if (Regex.IsMatch(word.ToString(), @"^[\u3040-\u30ff]+$"))
                //含有日文直接返回否
                return false;

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            var gbBytes = Encoding.Unicode.GetBytes(word.ToString());

            if (gbBytes.Length == 2) // double bytes char.  
            {
                if (gbBytes[1] >= 0x4E && gbBytes[1] <= 0x9F) //中文
                    chCount++;
                else
                    total--;
            }
            else if (gbBytes.Length == 1)
            {
                var byteAscii = int.Parse(gbBytes[0].ToString());
                if ((byteAscii >= 65 && byteAscii <= 90) || (byteAscii >= 97 && byteAscii <= 122)) //英文字母
                    enCount++;
                else
                    total--;
            }
        }

        if (chCount == 0) return false; //一个简体中文都没有

        return (chCount + enCount) / total >= rate;
    }

    /// <summary>
    /// 判断字符串是否全为单字节
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static bool IsEnglish(string str)
    {
        return new Regex("^[\x20-\x7E]+$").IsMatch(str);
    }

    /// <summary>
    /// 判断字符串是否全为假名
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static bool IsJapanese(string str)
    {
        return Regex.IsMatch(str, @"^[\u3040-\u30ff]+$");
    }

    /// <summary>
    /// 分句转为罗马音
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static ConvertedUnit[] SentenceToRomaji(string str)
    {
        var list = _tagger.ParseToNodes(str).ToArray();

        var result = new List<ConvertedUnit>();

        foreach (var item in list)
            if (item.CharType > 0)
            {
                string[] features;
                features = item.Feature.Split(',');
                if (TryCustomConvert(item.Surface, out var customResult))
                {
                    //用户自定义词典
                    result.Add(new ConvertedUnit(item.Surface, customResult, WanaKana.ToRomaji(customResult), true));
                }
                else if (features.Length > 0 && features[0] != "助詞" && IsJapanese(item.Surface))
                {
                    //纯假名
                    result.Add(new ConvertedUnit(item.Surface, KanaConverter.ToHiragana(item.Surface),
                        WanaKana.ToRomaji(item.Surface), false));
                }
                else if (features.Length <= 6 || new[] { "補助記号" }.Contains(features[0]))
                {
                    //标点符号
                    result.Add(new ConvertedUnit(item.Surface, item.Surface, item.Surface, false));
                }
                else if (IsEnglish(item.Surface))
                {
                    //英文
                    result.Add(new ConvertedUnit(item.Surface, item.Surface, item.Surface, false));
                }
                else
                {
                    //汉字或助词
                    var isKanji = !IsJapanese(item.Surface);
                    result.Add(new ConvertedUnit(item.Surface,
                        KanaConverter.ToHiragana(features[ChooseIndexByType(features[0])]),
                        WanaKana.ToRomaji(features[ChooseIndexByType(features[0])]), isKanji));
                }
            }
            else if (item.Stat != MeCabNodeStat.Bos && item.Stat != MeCabNodeStat.Eos)
            {
                result.Add(new ConvertedUnit(item.Surface, item.Surface, item.Surface, false));
            }

        return result.ToArray();
    }

    /// <summary>
    /// 根据不同的词型选择正确的索引
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    private static int ChooseIndexByType(string type)
    {
        switch (type)
        {
            case "助詞": return 11;
            default: return 19;
        }
    }

    /// <summary>
    /// 自定义转换规则
    /// </summary>
    /// <param name="str"></param>
    /// <param name="result"></param>
    /// <returns></returns>
    private static bool TryCustomConvert(string str, out string result)
    {
        if (_customizeDict.ContainsKey(str))
        {
            result = _customizeDict[str];
            return true;
        }

        result = "";
        return false;
    }
}

public static class KanaConverter
{
    /// <summary>
    /// 转为片假名
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static string ToKatakana(string str)
    {
        var stringBuilder = new StringBuilder();
        foreach (var c in str)
        {
            var bytes = Encoding.Unicode.GetBytes(c.ToString());
            if (bytes.Length == 2 && bytes[1] == 0x30 && bytes[0] >= 0x40 && bytes[0] <= 0x9F)
                stringBuilder.Append(Encoding.Unicode.GetString(new[] { (byte)(bytes[0] + 0x60), bytes[1] }));
            else
                stringBuilder.Append(c);
        }

        return stringBuilder.ToString();
    }

    /// <summary>
    /// 转为平假名
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static string ToHiragana(string str)
    {
        var stringBuilder = new StringBuilder();
        foreach (var c in str)
        {
            var bytes = Encoding.Unicode.GetBytes(c.ToString());
            if (bytes.Length == 2 && bytes[1] == 0x30 && bytes[0] >= 0xA0 && bytes[0] <= 0xFF)
                stringBuilder.Append(Encoding.Unicode.GetString(new[] { (byte)(bytes[0] - 0x60), bytes[1] }));
            else
                stringBuilder.Append(c);
        }

        return stringBuilder.ToString();
    }
}