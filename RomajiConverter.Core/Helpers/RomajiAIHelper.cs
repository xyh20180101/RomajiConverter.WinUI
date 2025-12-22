using OpenAI;
using OpenAI.Chat;
using RomajiConverter.Core.Models;
using System;
using System.ClientModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RomajiConverter.Core.Helpers
{
    public static class RomajiAIHelper
    {
        private static string _prompt = @"上面是一段日文歌词(可能包含中文翻译,在一行日文的下面一行)逐字/逐词转换为以下格式：
- 输入的一行对应输出的一行，需要对每行日文进行分词处理，每个分词格式为：日文原字/词(平假名)(读音)。每个分词用空格分隔
- 如果一个分词不含汉字，则不需要给出平假名和读音，只输出原文即可
- 遇到英文单词/字母、数字、标点符号时，需要单独作为一个分词，也不需要给出平假名和读音
- 不要包含任何解释、注释、Markdown、额外字段或文本
示例：
输入：その陽が落ちた瞬間を
输出：その 陽(ひ)(hi) が 落ち(おち)(ochi) た 瞬間(しゅんかん)(shunkan) を";

        private static Regex _formatRegex = new Regex(@"^(.*?)\((.*?)\)\((.*?)\)$", RegexOptions.Compiled);

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

        private static void InitConvertedLineList(ObservableCollection<ConvertedLine> convertedLines, string text, float chineseRate)
        {
            var lineTextList = text.Split(Environment.NewLine.ToArray())
                .Where(p => !string.IsNullOrWhiteSpace(p)).ToArray();

            convertedLines.Clear();
            for (var index = 0; index < lineTextList.Length; index++)
            {
                var line = lineTextList[index];

                var convertedLine = new ConvertedLine();

                if (RomajiHelper.IsChinese(line, chineseRate)) continue;

                convertedLine.Japanese = line.Replace("\0", ""); //文本中如果包含\0，会导致复制只能粘贴到第一个\0处，需要替换为空，以下同理
                convertedLine.Units = new ObservableCollection<ConvertedUnit>();

                if (index + 1 < lineTextList.Length &&
                    RomajiHelper.IsChinese(lineTextList[index + 1], chineseRate))
                    convertedLine.Chinese = lineTextList[index + 1];

                convertedLine.Index = (ushort)convertedLines.Count;
                convertedLines.Add(convertedLine);
            }
        }

        public static async Task ToRomaji(ObservableCollection<ConvertedLine> convertedLines, string text, AIServiceProvider aiServiceProvider, string apiKey, float chineseRate = 1f)
        {
            InitConvertedLineList(convertedLines, text, chineseRate);

            //获取ai结果
            var client = GetChatClient(aiServiceProvider, apiKey);

            var messages = new List<ChatMessage>
            {
                new UserChatMessage(string.Join("\n",convertedLines.Select(p => p.Japanese))),
                new UserChatMessage(_prompt)
            };

            var completion = client.CompleteChat(messages).Value;

            var resultLines = completion.Content[0].Text.Split("\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            for (ushort i = 0; i < resultLines.Length; i++)
            {
                var units = new ObservableCollection<ConvertedUnit>(resultLines[i].Split(" ".ToCharArray()).Select(u =>
                {
                    var match = _formatRegex.Match(u);
                    return match.Success
                        ? new ConvertedUnit(i, match.Groups[1].Value, match.Groups[2].Value, match.Groups[3].Value,
                            true)
                        : new ConvertedUnit(i, u, KanaHelper.ToHiragana(u), KanaHelper.KatakanaToRomaji(u), false);
                }));
                foreach (var unit in units)
                {
                    convertedLines[i].Units.Add(unit);
                    await Task.Delay(1);
                }
            }
        }

        public static void ToRomajiStreaming(ObservableCollection<ConvertedLine> convertedLines, string text, AIServiceProvider aiServiceProvider, string apiKey, float chineseRate = 1f)
        {
            InitConvertedLineList(convertedLines, text, chineseRate);

            //获取ai结果
            var client = GetChatClient(aiServiceProvider, apiKey);

            var messages = new List<ChatMessage>
            {
                new UserChatMessage(text),
                new UserChatMessage(_prompt)
            };

            var completionUpdates = client.CompleteChatStreaming(messages);

            foreach (var completionUpdate in completionUpdates)
            {
                if (completionUpdate.ContentUpdate.Count > 0)
                {
                    Debug.WriteLine(completionUpdate.ContentUpdate[0].Text);
                }
            }
        }
    }

    public enum AIServiceProvider
    {
        DeepSeek,
        Zhipu
    }
}
