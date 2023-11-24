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

namespace RomajiConverter.WinUI.Controls;

public sealed partial class EditableLabel : UserControl, INotifyPropertyChanged
{
    public static readonly DependencyProperty SelectedIndexProperty = DependencyProperty.Register(nameof(SelectedIndex), typeof(int),
        typeof(EditableLabel), new PropertyMetadata(0));

    public static readonly DependencyProperty ReplaceTextProperty = DependencyProperty.Register(nameof(ReplaceText), typeof(ObservableCollection<string>),
        typeof(EditableLabel), new PropertyMetadata(new ObservableCollection<string>()));

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

    public ObservableCollection<string> ReplaceText
    {
        get => (ObservableCollection<string>)GetValue(ReplaceTextProperty);
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

    private void EditBox_OnTextSubmitted(ComboBox sender, ComboBoxTextSubmittedEventArgs args)
    {
        if (ReplaceText.Contains(args.Text)) return;
        ReplaceText.Insert(0, args.Text);
        SelectedIndex = 0;
    }
}