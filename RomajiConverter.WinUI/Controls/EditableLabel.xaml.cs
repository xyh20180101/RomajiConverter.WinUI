using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;

namespace RomajiConverter.WinUI.Controls;

public sealed partial class EditableLabel : UserControl, INotifyPropertyChanged
{
    public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string),
        typeof(EditableLabel), new PropertyMetadata(default(string)));

    public static readonly DependencyProperty MyFontSizeProperty =
        DependencyProperty.Register(nameof(MyFontSize), typeof(double), typeof(EditableLabel),
            new PropertyMetadata(14d));

    private bool _isEdit;

    public EditableLabel()
    {
        InitializeComponent();
        IsEdit = false;
    }

    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
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
            OnPropertyChanged("EditLabelVisibility");
            OnPropertyChanged("EditBoxVisibility");
        }
    }

    public Visibility EditLabelVisibility => IsEdit ? Visibility.Collapsed : Visibility.Visible;

    public Visibility EditBoxVisibility => IsEdit ? Visibility.Visible : Visibility.Collapsed;

    public event PropertyChangedEventHandler PropertyChanged = delegate { };

    public void ToEdit()
    {
        IsEdit = true;
        EditBox.Focus(FocusState.Pointer);
        EditBox.SelectionStart = EditBox.Text.Length;
    }

    public void ToSave()
    {
        IsEdit = false;
    }

    private void EditBox_LostFocus(object sender, RoutedEventArgs e)
    {
        ToSave();
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

    private void EditBox_OnKeyDown(object sender, KeyRoutedEventArgs e)
    {
        if (e.Key == VirtualKey.Enter || e.Key == VirtualKey.Escape)
        {
            EditBox.IsEnabled = false;
            EditBox.IsEnabled = true;
        }
    }
}