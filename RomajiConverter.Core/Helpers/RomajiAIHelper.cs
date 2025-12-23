using OpenAI;
using OpenAI.Chat;
using RomajiConverter.Core.Models;
using System;
using System.ClientModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace RomajiConverter.Core.Helpers
{
    public static class RomajiAIHelper
    {
        private static string _prompt = @"上面是一段日文歌词，你需要逐词转换为以下格式：
- 每行输出必须严格对应每行输入，禁止额外添加换行，禁止输出空行，不能因为遇到标点符号而换行，换行符必须使用单个\n
- 对每行日文进行分词处理，每个分词之间用空格分隔
- 如果一个分词是日文且包含汉字，则需要给出平假名，用小写括号在原文后标注，格式为：日文分词(平假名)
- 遇到英文单词/字母、数字、标点符号、特殊符号、等非日文的unicode字符时，必须保留且单独作为一个分词，必须只输出原文，不能给出平假名
- 不要包含任何解释、注释、Markdown、额外字段或文本
- 示例仅供参考，不能直接输出，任何时候都需要根据上面给出的文本进行转换
示例：
输入：その陽が落ちた瞬間を
输出：その 陽(ひ) が 落ち(おち) た 瞬間(しゅんかん) を";

        private static Regex _formatRegex = new Regex(@"^(.*?)\((.*?)\)$", RegexOptions.Compiled);

        private static ChatClient GetChatClient(AIServiceProvider aiServiceProvider, string apiKey)
        {
            var url = "";
            var modelName = "";
            switch (aiServiceProvider)
            {
                case AIServiceProvider.DeepSeek:
                    url = "https://api.deepseek.com";
                    modelName = "deepseek-chat";
                    break;
                case AIServiceProvider.Zhipu:
                    url = "https://open.bigmodel.cn/api/paas/v4/";
                    modelName = "glm-4.5-flash";
                    break;
            }
            return new ChatClient(
                model: modelName,
                credential: new ApiKeyCredential(apiKey),
                options: new OpenAIClientOptions
                {
                    Endpoint = new Uri(url)
                }
            );
        }
        public static async Task ToRomaji(ObservableCollection<ConvertedLine> convertedLines, string text, AIServiceProvider aiServiceProvider, string apiKey, CancellationToken cancellationToken = default, float chineseRate = 1f)
        {
            var lineTextList = text.Split(Environment.NewLine.ToArray())
                .Where(p => !string.IsNullOrWhiteSpace(p)).ToArray();

            var cacheList = new List<ConvertedLine>();
            for (var index = 0; index < lineTextList.Length; index++)
            {
                var line = lineTextList[index];

                if (RomajiHelper.IsChinese(line, chineseRate)) continue;

                var convertedLine = new ConvertedLine
                {
                    Japanese = line.Replace("\0", "")
                };

                if (index + 1 < lineTextList.Length &&
                    RomajiHelper.IsChinese(lineTextList[index + 1], chineseRate))
                    convertedLine.Chinese = lineTextList[index + 1];

                convertedLine.Index = (ushort)cacheList.Count;
                cacheList.Add(convertedLine);
            }

            if (cacheList.Count == 0) return;

            //获取ai结果
            var client = GetChatClient(aiServiceProvider, apiKey);

            var messages = new List<ChatMessage>
            {
                new UserChatMessage($"{string.Join("\n",cacheList.Select(p => p.Japanese))}\n{_prompt}")
            };

            Debug.WriteLine($"{string.Join("\n", cacheList.Select(p => p.Japanese))}\n{_prompt}");

            var completion = await client.CompleteChatAsync(messages, cancellationToken: cancellationToken);

            var resultLines = completion.Value.Content[0].Text.Replace("\r", "").Split("\n", StringSplitOptions.RemoveEmptyEntries);
            for (ushort i = 0; i < resultLines.Length; i++)
            {
                Debug.Write(resultLines[i]);
                var line = new ConvertedLine
                {
                    Chinese = i >= cacheList.Count ? string.Empty : cacheList[i].Chinese,
                    Index = i,
                    Japanese = i >= cacheList.Count ? string.Empty : cacheList[i].Japanese
                };
                convertedLines.Add(line);
                var units = new ObservableCollection<ConvertedUnit>(resultLines[i].Split(" ", StringSplitOptions.RemoveEmptyEntries).Select(u => GetUnit(i, u)));
                foreach (var unit in units)
                {
                    convertedLines[i].Units.Add(unit);
                }
            }
        }

        public static async Task ToRomajiStreaming(ObservableCollection<ConvertedLine> convertedLines, string text, AIServiceProvider aiServiceProvider, string apiKey, CancellationToken cancellationToken = default, float chineseRate = 1f)
        {
            var lineTextList = text.Split(Environment.NewLine.ToArray())
                .Where(p => !string.IsNullOrWhiteSpace(p)).ToArray();

            var cacheList = new List<ConvertedLine>();
            for (var index = 0; index < lineTextList.Length; index++)
            {
                var line = lineTextList[index];

                if (RomajiHelper.IsChinese(line, chineseRate)) continue;

                var convertedLine = new ConvertedLine
                {
                    Japanese = line.Replace("\0", "")
                };

                if (index + 1 < lineTextList.Length &&
                    RomajiHelper.IsChinese(lineTextList[index + 1], chineseRate))
                    convertedLine.Chinese = lineTextList[index + 1];

                convertedLine.Index = (ushort)cacheList.Count;
                cacheList.Add(convertedLine);
            }

            if (cacheList.Count == 0) return;

            //获取ai结果
            var client = GetChatClient(aiServiceProvider, apiKey);

            var messages = new List<ChatMessage>
            {
                new UserChatMessage($"{string.Join("\n",cacheList.Select(p => p.Japanese))}\n{_prompt}")
            };

            Debug.WriteLine($"{string.Join("\n", cacheList.Select(p => p.Japanese))}\n{_prompt}");

            var completionUpdates = client.CompleteChatStreamingAsync(messages, cancellationToken: cancellationToken);

            var stringBuilder = new StringBuilder();
            ushort lineIndex = 0;
            var l = 0;
            var r = 0;

            var firstLine = new ConvertedLine
            {
                Chinese = lineIndex >= cacheList.Count ? string.Empty : cacheList[lineIndex].Chinese,
                Index = lineIndex,
                Japanese = lineIndex >= cacheList.Count ? string.Empty : cacheList[lineIndex].Japanese
            };
            convertedLines.Add(firstLine);
            await foreach (var completionUpdate in completionUpdates)
            {
                if (completionUpdate.ContentUpdate.Count > 0)
                {
                    var delta = completionUpdate.ContentUpdate[0].Text.Replace("\r", "");
                    if (string.IsNullOrEmpty(delta)) continue;
                    stringBuilder.Append(delta);
                    Debug.Write(completionUpdate.ContentUpdate[0].Text);

                    while (r < stringBuilder.Length)
                    {
                        if (stringBuilder[r] == '\n')
                        {
                            var lastLine = convertedLines.Last();
                            lastLine.Units.Add(GetUnit(lineIndex, stringBuilder.ToString(l, r - l)));

                            lineIndex++;
                            var newLine = new ConvertedLine
                            {
                                Chinese = lineIndex >= cacheList.Count ? string.Empty : cacheList[lineIndex].Chinese,
                                Index = lineIndex,
                                Japanese = lineIndex >= cacheList.Count ? string.Empty : cacheList[lineIndex].Japanese
                            };
                            convertedLines.Add(newLine);
                            r++;
                            l = r;
                        }
                        else if (stringBuilder[r] == ' ')
                        {
                            var lastLine = convertedLines.Last();
                            lastLine.Units.Add(GetUnit(lineIndex, stringBuilder.ToString(l, r - l)));

                            r++;
                            l = r;
                        }
                        else
                        {
                            r++;
                        }
                    }
                }
            }

            if (l != r)
            {
                var lastLine = convertedLines.Last();
                lastLine.Units.Add(GetUnit(lineIndex, stringBuilder.ToString(l, r - l)));
            }
        }

        private static ConvertedUnit GetUnit(ushort lineIndex, string unitString)
        {
            var match = _formatRegex.Match(unitString);

            return match.Success
                ? new ConvertedUnit(lineIndex, match.Groups[1].Value, match.Groups[2].Value, KanaHelper.KatakanaToRomaji(match.Groups[2].Value),
                    true)
                : new ConvertedUnit(lineIndex, unitString, KanaHelper.ToHiragana(unitString), KanaHelper.KatakanaToRomaji(unitString), false);
        }
    }

    public enum AIServiceProvider
    {
        DeepSeek,
        Zhipu
    }
}
