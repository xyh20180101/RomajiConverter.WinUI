using System.ComponentModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

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

    [Category("Extension")]
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