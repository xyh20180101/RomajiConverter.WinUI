using System;
using System.Collections.Generic;
using Windows.ApplicationModel.Resources;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using RomajiConverter.Core.Models;
using RomajiConverter.WinUI.Models;
using UnhandledExceptionEventArgs = Microsoft.UI.Xaml.UnhandledExceptionEventArgs;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace RomajiConverter.WinUI;

/// <summary>
/// Provides application-specific behavior to supplement the default Application class.
/// </summary>
public partial class App : Application
{
    public const string ConfigFileName = "config.json";
    public static MainWindow MainWindow;

    public static MyConfig Config = new();

    public static List<ConvertedLine> ConvertedLineList = new();

    /// <summary>
    /// Initializes the singleton application object.  This is the first line of authored code
    /// executed, and as such is the logical equivalent of main() or WinMain().
    /// </summary>
    public App()
    {
        InitializeComponent();
        UnhandledException += App_UnhandledException;
    }

    /// <summary>
    /// Invoked when the application is launched.
    /// </summary>
    /// <param name="args">Details about the launch request and process.</param>
    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        MainWindow = new MainWindow();
        MainWindow.Activate();
    }

    /// <summary>
    /// 全局异常捕获
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void App_UnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        e.Handled = true;
        var resourceLoader = ResourceLoader.GetForViewIndependentUse();
        await new ContentDialog
        {
            XamlRoot = MainWindow.Content.XamlRoot,
            Title = resourceLoader.GetString("Exception"),
            Content = e.Message,
            CloseButtonText = resourceLoader.GetString("Close"),
            DefaultButton = ContentDialogButton.Close
        }.ShowAsync();
    }
}