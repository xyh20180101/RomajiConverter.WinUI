using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace RomajiConverter.WinUI.Models;

public class ConvertedUnit : INotifyPropertyChanged
{
    private string _hiragana;
    private bool _isKanji;
    private string _japanese;
    private string _romaji;
    private ObservableCollection<string> _replaceRomaji;
    private ObservableCollection<string> _replaceHiragana;

    public ConvertedUnit(string japanese, string hiragana, string romaji, bool isKanji)
    {
        Japanese = japanese;
        Romaji = romaji;
        Hiragana = hiragana;
        IsKanji = isKanji;
        ReplaceHiragana =new ObservableCollection<string> { hiragana };
        ReplaceRomaji = new ObservableCollection<string> { romaji };
    }

    public string Japanese
    {
        get => _japanese;
        set
        {
            if (value == _japanese) return;
            _japanese = value;
            OnPropertyChanged();
        }
    }

    public string Romaji
    {
        get => _romaji;
        set
        {
            if (value == _romaji) return;
            _romaji = value;
            OnPropertyChanged();
        }
    }

    public ObservableCollection<string> ReplaceRomaji
    {
        get => _replaceRomaji;
        set
        {
            if (Equals(value, _replaceRomaji)) return;
            _replaceRomaji = value;
            OnPropertyChanged();
        }
    }

    public string Hiragana
    {
        get => _hiragana;
        set
        {
            if (value == _hiragana) return;
            _hiragana = value;
            OnPropertyChanged();
        }
    }

    public ObservableCollection<string> ReplaceHiragana
    {
        get => _replaceHiragana;
        set
        {
            if (Equals(value, _replaceHiragana)) return;
            _replaceHiragana = value;
            OnPropertyChanged();
        }
    }

    public bool IsKanji
    {
        get => _isKanji;
        set
        {
            if (value == _isKanji) return;
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