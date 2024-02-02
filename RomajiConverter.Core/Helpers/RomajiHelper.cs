using System.Collections.ObjectModel;
using System.Text;
using System.Text.RegularExpressions;
using MeCab;
using MeCab.Extension.UniDic;
using RomajiConverter.Core.Extensions;
using RomajiConverter.Core.Models;

namespace RomajiConverter.Core.Helpers;

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

    #region 主逻辑

    /// <summary>
    /// 生成转换结果列表(此处主要实现区分中文,识别变体)
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
                    var regex = new Regex("[^a-zA-Z0-9 ]", RegexOptions.Compiled);

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
                                tempSentence.Replace(match.Value[0],
                                    VariantHelper.GetVariant(match.Value[0])); //尝试替换后的句子
                            convertedLine.Japanese =
                                convertedLine.Japanese.Replace(match.Value[0],
                                    VariantHelper.GetVariant(match.Value[0])); //顺便更新一下这个字段

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

            convertedLine.Index = (ushort)convertedText.Count;
            convertedText.Add(convertedLine);
        }

        return convertedText;
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
        {
            ConvertedUnit unit = null;
            if (item.CharType > 0)
            {
                var features = CustomSplit(item.Feature);
                if (TryCustomConvert(item.Surface, out var customResult))
                {
                    //用户自定义词典
                    unit = new ConvertedUnit(item.Surface,
                        customResult,
                        KanaHelper.KatakanaToRomaji(customResult),
                        true);
                }
                else if (features.Length > 0 && item.GetPos1() != "助詞" && IsJapanese(item.Surface))
                {
                    //纯假名
                    unit = new ConvertedUnit(item.Surface,
                        KanaHelper.ToHiragana(item.Surface),
                        KanaHelper.KatakanaToRomaji(item.Surface),
                        false);
                }
                else if (features.Length <= 6 || new[] { "補助記号" }.Contains(item.GetPos1()))
                {
                    //标点符号或无法识别的字
                    unit = new ConvertedUnit(item.Surface,
                        item.Surface,
                        item.Surface,
                        false);
                }
                else if (IsEnglish(item.Surface))
                {
                    //英文
                    unit = new ConvertedUnit(item.Surface,
                        item.Surface,
                        item.Surface,
                        false);
                }
                else
                {
                    //汉字或助词
                    var kana = GetKana(item);

                    unit = new ConvertedUnit(item.Surface,
                        KanaHelper.ToHiragana(kana),
                        KanaHelper.KatakanaToRomaji(kana),
                        !IsJapanese(item.Surface));
                    var (replaceHiragana, replaceRomaji) = GetReplaceData(item);
                    unit.ReplaceHiragana = replaceHiragana;
                    unit.ReplaceRomaji = replaceRomaji;
                }
            }
            else if (item.Stat != MeCabNodeStat.Bos && item.Stat != MeCabNodeStat.Eos)
            {
                unit = new ConvertedUnit(item.Surface,
                    item.Surface,
                    item.Surface,
                    false);
            }

            if (unit != null)
                result.Add(unit);
        }

        return result.ToArray();
    }

    #endregion

    #region 帮助方法

    /// <summary>
    /// 自定义分隔方法(Feature可能存在如 a,b,c,"d,e",f 格式的数据,此处不能把双引号中的内容也分隔开)
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    private static string[] CustomSplit(string str)
    {
        var list = new List<string>();
        var item = new List<char>();
        var haveMark = false;
        foreach (var c in str)
            if (c == ',' && !haveMark)
            {
                list.Add(new string(item.ToArray()));
                item.Clear();
            }
            else if (c == '"')
            {
                item.Add(c);
                haveMark = !haveMark;
            }
            else
            {
                item.Add(c);
            }

        return list.ToArray();
    }

    /// <summary>
    /// 获取所有发音
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    private static (ObservableCollection<ReplaceString> replaceHiragana, ObservableCollection<ReplaceString>
        replaceRomaji) GetReplaceData(MeCabNode node)
    {
        var length = node.Length;
        var replaceNodeList = new List<MeCabNode>();

        GetAllReplaceNode(replaceNodeList, node);

        void GetAllReplaceNode(List<MeCabNode> list, MeCabNode node)
        {
            if (node != null && !list.Contains(node) && node.Length == length)
            {
                list.Add(node);
                GetAllReplaceNode(list, node.BNext);
                GetAllReplaceNode(list, node.ENext);
            }
        }

        var replaceHiragana = new ObservableCollection<ReplaceString>();
        var replaceRomaji = new ObservableCollection<ReplaceString>();

        ushort i = 1;
        foreach (var meCabNode in replaceNodeList.DistinctBy(GetKana))
        {
            var kana = GetKana(meCabNode);
            if (kana != null)
            {
                replaceHiragana.Add(new ReplaceString(i, KanaHelper.ToHiragana(kana), true));
                replaceRomaji.Add(new ReplaceString(i, KanaHelper.KatakanaToRomaji(kana), true));
                i++;
            }
        }

        return (replaceHiragana, replaceRomaji);
    }

    private static string GetKana(MeCabNode node)
    {
        return node.GetPos1() == "助詞" ? node.GetPron() : node.GetKana();
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

    /// <summary>
    /// 判断字符串(句子)是否简体中文
    /// </summary>
    /// <param name="str"></param>
    /// <param name="rate">容错率(0-1)</param>
    /// <returns></returns>
    private static bool IsChinese(string str, float rate)
    {
        if (str.Length < 2)
            return false;

        var wordArray = str.ToCharArray();
        var total = wordArray.Length;
        var chCount = 0f;
        var enCount = 0f;

        foreach (var word in wordArray)
        {
            if (word != 'ー' && IsJapanese(word.ToString()))
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
    private static bool IsEnglish(string str)
    {
        return new Regex("^[\x20-\x7E]+$", RegexOptions.Compiled).IsMatch(str);
    }

    /// <summary>
    /// 判断字符串是否全为假名
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    private static bool IsJapanese(string str)
    {
        return Regex.IsMatch(str, @"^[\u3040-\u30ff]+$", RegexOptions.Compiled);
    }

    #endregion
}