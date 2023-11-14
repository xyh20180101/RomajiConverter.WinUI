﻿using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace RomajiConverter.WinUI.Models;

public class ConvertedUnit : INotifyPropertyChanged
{
    private string _hiragana;

    private bool _isKanji;

    private string _japanese;

    private string _romaji;

    public ConvertedUnit(string japanese, string hiragana, string romaji, bool isKanji)
    {
        Japanese = japanese;
        Romaji = romaji;
        Hiragana = hiragana;
        IsKanji = isKanji;
    }

    public string Japanese
    {
        get => _japanese;
        set
        {
            _japanese = value;
            OnPropertyChanged();
        }
    }

    public string Romaji
    {
        get => _romaji;
        set
        {
            _romaji = value;
            OnPropertyChanged();
        }
    }

    public string Hiragana
    {
        get => _hiragana;
        set
        {
            _hiragana = value;
            OnPropertyChanged();
        }
    }

    public bool IsKanji
    {
        get => _isKanji;
        set
        {
            _isKanji = value;
            OnPropertyChanged();
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}