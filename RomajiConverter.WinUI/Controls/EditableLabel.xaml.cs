using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using RomajiConverter.Core.Models;
using RomajiConverter.WinUI.Enums;

namespace RomajiConverter.WinUI.Controls;

public sealed partial class EditableLabel : UserControl, INotifyPropertyChanged
{
    public static readonly DependencyProperty SelectedTextProperty = DependencyProperty.Register(nameof(SelectedText),
        typeof(ReplaceString),
        typeof(EditableLabel), new PropertyMetadata(new ReplaceString(0, string.Empty, false)));

    public static readonly DependencyProperty ReplaceTextProperty = DependencyProperty.Register(nameof(ReplaceText),
        typeof(ObservableCollection<ReplaceString>),
        typeof(EditableLabel), new PropertyMetadata(new ObservableCollection<ReplaceString>()));

    public static readonly DependencyProperty MyFontSizeProperty =
        DependencyProperty.Register(nameof(MyFontSize), typeof(double), typeof(EditableLabel),
            new PropertyMetadata(14d));

    public static readonly DependencyProperty BorderVisibilitySettingProperty =
        DependencyProperty.Register(nameof(BorderVisibilitySetting), typeof(BorderVisibilitySetting),
            typeof(EditableLabel),
            new PropertyMetadata(BorderVisibilitySetting.Hidden));

    private bool _isEdit;

    public EditableLabel()
    {
        InitializeComponent();
        IsEdit = false;
    }

    public ReplaceString SelectedText
    {
        get => (ReplaceString)GetValue(SelectedTextProperty);
        set => SetValue(SelectedTextProperty, value);
    }

    public ObservableCollection<ReplaceString> ReplaceText
    {
        get => (ObservableCollection<ReplaceString>)GetValue(ReplaceTextProperty);
        set => SetValue(ReplaceTextProperty, value);
    }

    public double MyFontSize
    {
        get => (double)GetValue(MyFontSizeProperty);
        set => SetValue(MyFontSizeProperty, value);
    }

    public bool IsEdit
    {
        get => _isEdit;
        set
        {
            if (value == _isEdit) return;
            _isEdit = value;
            OnPropertyChanged(nameof(EditLabelVisibility));
            OnPropertyChanged(nameof(EditBoxVisibility));
        }
    }

    public SolidColorBrush BorderBrushColor
    {
        get
        {
            if (IsEdit || BorderVisibilitySetting == BorderVisibilitySetting.Hidden)
                return new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
            if (BorderVisibilitySetting == BorderVisibilitySetting.Visible || ReplaceText.Count > 1)
                return new SolidColorBrush(Color.FromArgb(0xAA, 0x99, 0x99, 0x99));
            return new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
        }
    }

    public BorderVisibilitySetting BorderVisibilitySetting
    {
        get => (BorderVisibilitySetting)GetValue(BorderVisibilitySettingProperty);
        set
        {
            SetValue(BorderVisibilitySettingProperty, value);
            OnPropertyChanged(nameof(BorderBrushColor));
        }
    }

    public Visibility EditLabelVisibility => IsEdit ? Visibility.Collapsed : Visibility.Visible;

    public Visibility EditBoxVisibility => IsEdit ? Visibility.Visible : Visibility.Collapsed;

    public event PropertyChangedEventHandler PropertyChanged = delegate { };

    public async void ToEdit()
    {
        IsEdit = true;
        await Task.Delay(10);
        EditBox.IsDropDownOpen = true;
    }

    public void ToSave()
    {
        IsEdit = false;
    }

    public void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private void EditLabel_OnDoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
    {
        ToEdit();
        e.Handled = true;
    }

    private void EditBox_OnDropDownClosed(object sender, object e)
    {
        ToSave();
    }

    private void EditBox_OnTextSubmitted(ComboBox sender, ComboBoxTextSubmittedEventArgs args)
    {
        if (ReplaceText.Any(p => p.Value == args.Text)) return;
        var newText = new ReplaceString(0, args.Text, false);
        ReplaceText.Insert(0, newText);
        SelectedText = newText;
        args.Handled = true;
    }

    public void Destroy()
    {
        EditLabel.DoubleTapped -= EditLabel_OnDoubleTapped;
        EditBox.DropDownClosed -= EditBox_OnDropDownClosed;
        EditBox.TextSubmitted -= EditBox_OnTextSubmitted;
        Bindings.StopTracking();
        ClearValue(SelectedTextProperty);
        ClearValue(ReplaceTextProperty);
        ClearValue(MyFontSizeProperty);
        ClearValue(BorderVisibilitySettingProperty);
    }
}