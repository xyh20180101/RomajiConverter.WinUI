# RomajiConverter.Core
用于[RomajiConverter.WinUI](https://github.com/xyh20180101/RomajiConverter.WinUI)项目的罗马音转换相关逻辑的包


## 使用
```C#
List<ConvertedLine> list = RomajiHelper.ToRomaji("");

public class ConvertedLine
{
    public ushort Index { get; set; }

    public string Chinese { get; set; }

    public string Japanese { get; set; }

    public ConvertedUnit[] Units { get; set; }
}

public class ConvertedUnit
{
    public string Japanese { get; set; }

    public string Romaji { get; set; }

    public ObservableCollection<ReplaceString> ReplaceRomaji { get; set; }

    public string Hiragana { get; set; }

    public ObservableCollection<ReplaceString> ReplaceHiragana { get; set; }

    public bool IsKanji { get; set; }

    public ushort SelectId { get; set; }
}

public class ReplaceString
{
    public ushort Id { get; set; }

    public string Value { get; set; }

    public bool IsSystem { get; set; }
}
```