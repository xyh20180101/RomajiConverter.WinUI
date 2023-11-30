using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RomajiConverter.WinUI.Helpers;

public static class VariantHelper
{
    private static IReadOnlyDictionary<char, char> _simplifiedVariant;

    private static IReadOnlyDictionary<char, char> _traditionalVariant;

    public static void Init()
    {
        _simplifiedVariant =
            GetVariantDictionary("Variants/kSimplifiedVariant.txt", "Variants/kTraditionalVariant.txt");
        _traditionalVariant =
            GetVariantDictionary("Variants/kTraditionalVariant.txt", "Variants/kSimplifiedVariant.txt");
    }

    private static Dictionary<char, char> GetVariantDictionary(string filePath, string variantPath)
    {
        var variantText = File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, variantPath));
        var variantLines = variantText.Split("\r\n", StringSplitOptions.RemoveEmptyEntries);
        var variantDictionary = variantLines.Select(line => line.Split("\t", StringSplitOptions.RemoveEmptyEntries))
            .ToDictionary(items => items[0].Split(" ")[0], items => items[0].Split(" ")[1]);

        var text = File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, filePath));
        var lines = text.Split("\r\n", StringSplitOptions.RemoveEmptyEntries);
        var dictionary = new Dictionary<char, char>();
        foreach (var line in lines)
        {
            var items = line.Split("\t", StringSplitOptions.RemoveEmptyEntries);
            if (variantDictionary.TryGetValue(items[2], out var variant))
            {
                var c = items[0].Split(" ")[1][0];
                if(dictionary.ContainsKey(c)==false)
                    dictionary.Add(c, variant[0]);
            }
        }

        return dictionary;
    }

    /// <summary>
    /// 获取简体变体（获取失败则返回自身）
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static char GetSimplifiedVariant(char input)
    {
        return _simplifiedVariant.TryGetValue(input, out var variant) ? variant : input;
    }

    /// <summary>
    /// 获取繁体变体（获取失败则返回自身）
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static char GetTraditionalVariant(char input)
    {
        return _traditionalVariant.TryGetValue(input, out var variant) ? variant : input;
    }

    /// <summary>
    /// 获取变体（获取失败则返回自身）
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static char GetVariant(char input)
    {
        var simplifiedVariant = GetSimplifiedVariant(input);
        if (simplifiedVariant != input)
            return simplifiedVariant;
        return GetTraditionalVariant(input);
    }
}