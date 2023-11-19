using System.ComponentModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace RomajiConverter.WinUI.Controls;

public sealed partial class ScaleLabel : UserControl
{
    public static readonly DependencyProperty ScaleTextProperty = DependencyProperty.Register("ScaleText",
        typeof(string), typeof(ScaleLabel), new PropertyMetadata(default(string)));

    public ScaleLabel()
    {
        InitializeComponent();
        DataContext = this;
    }

    public string ScaleText
    {
        get => (string)GetValue(ScaleTextProperty);
        set
        {
            SetValue(ScaleTextProperty, value);
            VisualStateManager.GoToState(this, "ScaleTextChanged", true);
            VisualStateManager.GoToState(this, "ScaleTextNormal", true);
        }
    }
}