using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RomajiConverter.WinUI.Helpers;

public static class VariantHelper
{
    private static IReadOnlyDictionary<string, string> _simplifiedVariant;

    private static IReadOnlyDictionary<string, string> _traditionalVariant;

    public static void Init()
    {
        _simplifiedVariant =
            GetVariantDictionary("Variants/kSimplifiedVariant.txt", "Variants/kTraditionalVariant.txt");
        _traditionalVariant =
            GetVariantDictionary("Variants/kTraditionalVariant.txt", "Variants/kSimplifiedVariant.txt");
    }

    private static Dictionary<string, string> GetVariantDictionary(string filePath, string variantPath)
    {
        var variantText = File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, variantPath));
        var variantLines = variantText.Split("\r\n", StringSplitOptions.RemoveEmptyEntries);
        var variantDictionary = variantLines.Select(line => line.Split("\t", StringSplitOptions.RemoveEmptyEntries))
            .ToDictionary(items => items[0].Split(" ")[0], items => items[0].Split(" ")[1]);

        var text = File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, filePath));
        var lines = text.Split("\r\n", StringSplitOptions.RemoveEmptyEntries);
        var dictionary = new Dictionary<string, string>();
        foreach (var line in lines)
        {
            var items = line.Split("\t", StringSplitOptions.RemoveEmptyEntries);
            if (variantDictionary.TryGetValue(items[2], out var variant))
                dictionary.Add(items[0].Split(" ")[1], variant);
        }

        return dictionary;
    }

    /// <summary>
    /// 获取简体变体（获取失败则返回自身）
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static string GetSimplifiedVariant(string input)
    {
        return _simplifiedVariant.TryGetValue(input, out var variant) ? variant : input;
    }

    /// <summary>
    /// 获取繁体变体（获取失败则返回自身）
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static string GetTraditionalVariant(string input)
    {
        return _traditionalVariant.TryGetValue(input, out var variant) ? variant : input;
    }

    /// <summary>
    /// 获取变体（获取失败则返回自身）
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static string GetVariant(string input)
    {
        var simplifiedVariant = GetSimplifiedVariant(input);
        if (simplifiedVariant != input)
            return simplifiedVariant;
        return GetTraditionalVariant(input);
    }
}