using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Windows.System;
using CommunityToolkit.WinUI.UI.Controls.TextToolbarSymbols;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using System.Collections.ObjectModel;
using Windows.UI;
using Microsoft.UI.Xaml.Media;
using RomajiConverter.WinUI.Models;

namespace RomajiConverter.WinUI.Controls;

public sealed partial class EditableLabel : UserControl, INotifyPropertyChanged
{
    public static readonly DependencyProperty SelectedIndexProperty = DependencyProperty.Register(nameof(SelectedIndex), typeof(int),
        typeof(EditableLabel), new PropertyMetadata(0));

    public static readonly DependencyProperty ReplaceTextProperty = DependencyProperty.Register(nameof(ReplaceText), typeof(ObservableCollection<ReplaceString>),
        typeof(EditableLabel), new PropertyMetadata(new ObservableCollection<ReplaceString>()));

    public static readonly DependencyProperty MyFontSizeProperty =
        DependencyProperty.Register(nameof(MyFontSize), typeof(double), typeof(EditableLabel),
            new PropertyMetadata(14d));

    private bool _isEdit;

    public EditableLabel()
    {
        InitializeComponent();
        IsEdit = false;
    }

    public int SelectedIndex
    {
        get => (int)GetValue(SelectedIndexProperty);
        set => SetValue(SelectedIndexProperty, value);
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
            OnPropertyChanged("EditLabelVisibility");
            OnPropertyChanged("EditBoxVisibility");
        }
    }

    public SolidColorBrush BorderBrushColor => ReplaceText.Count > 1
        ? new SolidColorBrush(Color.FromArgb(0xAA, 0xAA, 0xAA, 0xAA))
        : new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));

    public Visibility EditLabelVisibility => IsEdit ? Visibility.Collapsed : Visibility.Visible;

    public Visibility EditBoxVisibility => IsEdit ? Visibility.Visible : Visibility.Collapsed;

    public event PropertyChangedEventHandler PropertyChanged = delegate { };

    public void ToEdit()
    {
        IsEdit = true;
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

    private void EditBox_OnKeyDown(object sender, KeyRoutedEventArgs e)
    {
        if (e.Key == VirtualKey.Enter || e.Key == VirtualKey.Escape)
        {
            EditBox.IsEnabled = false;
            EditBox.IsEnabled = true;
        }
    }

    private void EditBox_OnDropDownClosed(object sender, object e)
    {
        ToSave();
    }

    private void EditBox_OnLosingFocus(object sender, LosingFocusEventArgs e)
    {
        //if (e.NewFocusedElement is not ComboBoxItem)
        //ToSave();
    }
}