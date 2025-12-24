using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace RomajiConverter.WinUI.Models;

public class OpenAIConfig : INotifyPropertyChanged
{
    private string _name;
    private string _baseUrl;
    private string _model;
    private string _apiKey;
    private bool _isSelected;

    public string Name
    {
        get => _name;
        set
        {
            if (value == _name) return;
            _name = value;
            OnPropertyChanged();
        }
    }

    public string BaseUrl
    {
        get => _baseUrl;
        set
        {
            if (value == _baseUrl) return;
            _baseUrl = value;
            OnPropertyChanged();
        }
    }

    public string Model
    {
        get => _model;
        set
        {
            if (value == _model) return;
            _model = value;
            OnPropertyChanged();
        }
    }

    public string ApiKey
    {
        get => _apiKey;
        set
        {
            if (value == _apiKey) return;
            _apiKey = value;
            OnPropertyChanged();
        }
    }

    public bool IsSelected
    {
        get => _isSelected;
        set
        {
            if (value == _isSelected) return;
            _isSelected = value;
            OnPropertyChanged();
        }
    }

    public event PropertyChangedEventHandler PropertyChanged = delegate { };

    public void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}