using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI.Core;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace RomajiConverter.WinUI.Controls
{
    public sealed partial class EditableLabel : UserControl, INotifyPropertyChanged
    {
        public EditableLabel()
        {
            InitializeComponent();
            DataContext = this;
            IsEdit = false;
        }

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(EditableLabel), new PropertyMetadata(default(string)));

        [Category("Extension")]
        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public static readonly DependencyProperty MyFontSizeProperty = DependencyProperty.Register("MyFontSize", typeof(double), typeof(EditableLabel), new PropertyMetadata(12d));

        [Category("Extension")]
        public double MyFontSize
        {
            get => (double)GetValue(MyFontSizeProperty);
            set => SetValue(MyFontSizeProperty, value);
        }

        private bool _isEdit;
        public bool IsEdit
        {
            get => _isEdit;
            set
            {
                _isEdit = value;
                OnPropertyChanged("EditLabelVisibility");
                OnPropertyChanged("EditBoxVisibility");
            }
        }

        public Visibility EditLabelVisibility => IsEdit ? Visibility.Collapsed : Visibility.Visible;

        public Visibility EditBoxVisibility => IsEdit ? Visibility.Visible : Visibility.Collapsed;

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

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
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
                EditBox.Focus(FocusState.Unfocused);
            }
        }
    }
}
