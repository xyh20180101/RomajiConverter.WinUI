using System.ComponentModel;
using System.Runtime.CompilerServices;
using Microsoft.UI.Xaml.Controls;
using RomajiConverter.WinUI.Models;

namespace RomajiConverter.WinUI.Dialogs;

public sealed partial class AddOpenAIConfigContentDialog : ContentDialog, INotifyPropertyChanged
{
    private OpenAIConfig _openAIConfig;

    public AddOpenAIConfigContentDialog()
    {
        InitializeComponent();

        _openAIConfig = new OpenAIConfig
        {
            Name = $"Config {App.Config.OpenAIConfigs.Count + 1}"
        };
    }

    public OpenAIConfig OpenAIConfig
    {
        get => _openAIConfig;
        set
        {
            if (value == _openAIConfig) return;
            _openAIConfig = value;
            OnPropertyChanged();
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}