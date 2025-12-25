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
using RomajiConverter.Core.Options;

namespace RomajiConverter.Core.Helpers
{
    public static class RomajiAIHelper
    {
        public const string Prompt = @"用户将输入一段日文歌词，你需要逐词转换为以下格式：
- 每行输出必须严格对应每行输入，禁止额外添加换行，禁止输出空行，不能因为遇到标点符号而换行，换行符必须使用单个\n
- 对每行日文进行分词处理
- 如果一个分词是日文且包含汉字，则需要给出平假名，用小写括号在原文后标注，格式为：日文分词(平假名)
- 遇到作为助词的は/へ/を时，在后面添加“|”以及它的口语化假名，非助词情况下只输出原文
- 遇到英文单词/字母、数字、标点符号、特殊符号、等非日文的unicode字符时，必须保留且单独作为一个分词，必须只输出原文，不能给出平假名
- 每个分词之间必须用空格分隔
- 不要包含任何解释、注释、Markdown、额外字段或文本
- 示例仅供参考，不能直接输出，任何时候都需要根据上面给出的文本进行转换
示例：
输入：はなは綺麗です
输出：はな は|わ 綺麗(きれい) です";

        private static Regex _formatRegex = new Regex(@"^(.*?)(\((.*?)\))*?(\|(.*?))*?$", RegexOptions.Compiled);

        private static ChatCompletionOptions _chatCompletionOptions = new ChatCompletionOptions
        {
            Temperature = 0.2f
        };
        public static async Task ToRomaji(ObservableCollection<ConvertedLine> convertedLines, string text, ToRomajiAIOptions options, CancellationToken cancellationToken = default)
        {
            var lineTextList = text.Split(Environment.NewLine.ToArray())
                .Where(p => !string.IsNullOrWhiteSpace(p)).ToArray();

            var cacheList = new List<ConvertedLine>();
            for (var index = 0; index < lineTextList.Length; index++)
            {
                var line = lineTextList[index];

                if (RomajiHelper.IsChinese(line, options.ChineseRate)) continue;

                var convertedLine = new ConvertedLine
                {
                    Japanese = line.Replace("\0", "")
                };

                if (index + 1 < lineTextList.Length &&
                    RomajiHelper.IsChinese(lineTextList[index + 1], options.ChineseRate))
                    convertedLine.Chinese = lineTextList[index + 1];

                convertedLine.Index = (ushort)cacheList.Count;
                cacheList.Add(convertedLine);
            }

            if (cacheList.Count == 0) return;

            //获取ai结果
            var client = new ChatClient(
                model: options.Model,
                credential: new ApiKeyCredential(options.ApiKey),
                options: new OpenAIClientOptions
                {
                    Endpoint = new Uri(options.BaseUrl)
                }
            );

            var prompt = string.IsNullOrEmpty(options.Prompt) ? Prompt : options.Prompt;
            var content = string.Join("\n", cacheList.Select(p => p.Japanese));

            var messages = new List<ChatMessage>
            {
                new SystemChatMessage(prompt),
                new UserChatMessage(content)
            };

            Debug.WriteLine(prompt);
            Debug.WriteLine(content);

            var completion = await client.CompleteChatAsync(messages, _chatCompletionOptions, cancellationToken: cancellationToken);

            var resultLines = FixFormat(completion.Value.Content[0].Text).Split("\n", StringSplitOptions.RemoveEmptyEntries);
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
                var units = new ObservableCollection<ConvertedUnit>(resultLines[i].Split(" ", StringSplitOptions.RemoveEmptyEntries).Select(u => GetUnit(i, u, options.IsParticleAsPronunciation)));
                foreach (var unit in units)
                {
                    convertedLines[i].Units.Add(unit);
                }
            }
        }

        public static async Task ToRomajiStreamingAsync(ObservableCollection<ConvertedLine> convertedLines, string text, ToRomajiAIOptions options, CancellationToken cancellationToken = default)
        {
            var lineTextList = text.Split(Environment.NewLine.ToArray())
                .Where(p => !string.IsNullOrWhiteSpace(p)).ToArray();

            var cacheList = new List<ConvertedLine>();
            for (var index = 0; index < lineTextList.Length; index++)
            {
                var line = lineTextList[index];

                if (RomajiHelper.IsChinese(line, options.ChineseRate)) continue;

                var convertedLine = new ConvertedLine
                {
                    Japanese = line.Replace("\0", "")
                };

                if (index + 1 < lineTextList.Length &&
                    RomajiHelper.IsChinese(lineTextList[index + 1], options.ChineseRate))
                    convertedLine.Chinese = lineTextList[index + 1];

                convertedLine.Index = (ushort)cacheList.Count;
                cacheList.Add(convertedLine);
            }

            if (cacheList.Count == 0) return;

            //获取ai结果
            var client = new ChatClient(
                model: options.Model,
                credential: new ApiKeyCredential(options.ApiKey),
                options: new OpenAIClientOptions
                {
                    Endpoint = new Uri(options.BaseUrl)
                }
            );

            var prompt = string.IsNullOrEmpty(options.Prompt) ? Prompt : options.Prompt;
            var content = string.Join("\n", cacheList.Select(p => p.Japanese));

            var messages = new List<ChatMessage>
            {
                new SystemChatMessage(prompt),
                new UserChatMessage(content)
            };

            Debug.WriteLine(prompt);
            Debug.WriteLine(content);

            var completionUpdates = client.CompleteChatStreamingAsync(messages, _chatCompletionOptions, cancellationToken: cancellationToken);

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
                    var delta = FixFormat(completionUpdate.ContentUpdate[0].Text);
                    if (string.IsNullOrEmpty(delta)) continue;
                    stringBuilder.Append(delta);
                    Debug.Write(completionUpdate.ContentUpdate[0].Text);

                    while (r < stringBuilder.Length)
                    {
                        if (stringBuilder[r] == '\n')
                        {
                            var lastLine = convertedLines.Last();
                            lastLine.Units.Add(GetUnit(lineIndex, stringBuilder.ToString(l, r - l), options.IsParticleAsPronunciation));

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
                            lastLine.Units.Add(GetUnit(lineIndex, stringBuilder.ToString(l, r - l), options.IsParticleAsPronunciation));

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
                lastLine.Units.Add(GetUnit(lineIndex, stringBuilder.ToString(l, r - l), options.IsParticleAsPronunciation));
            }
        }

        private static string FixFormat(string content)
        {
            content = content.Replace("\r", "");
            content = content.Replace("\\n", "\n");

            return content;
        }

        private static ConvertedUnit GetUnit(ushort lineIndex, string unitString, bool isParticleAsPronunciation)
        {
            var match = _formatRegex.Match(unitString);

            if (!match.Success)
            {
                return new ConvertedUnit(lineIndex, unitString, KanaHelper.ToHiragana(unitString),
                    KanaHelper.KatakanaToRomaji(unitString), false);
            }

            var origin = match.Groups[1].Value;
            var kanji_gana = match.Groups[3].Value;
            var particle_gana = match.Groups[5].Value;

            if (!string.IsNullOrEmpty(kanji_gana))
            {
                return new ConvertedUnit(lineIndex, origin, kanji_gana,
                    KanaHelper.KatakanaToRomaji(kanji_gana), true);
            }
            else if (isParticleAsPronunciation && !string.IsNullOrEmpty(particle_gana))
            {
                return new ConvertedUnit(lineIndex, origin, particle_gana,
                    KanaHelper.KatakanaToRomaji(particle_gana), false);
            }
            else
            {
                return new ConvertedUnit(lineIndex, origin, KanaHelper.ToHiragana(origin),
                    KanaHelper.KatakanaToRomaji(origin), false);
            }
        }
    }
}
