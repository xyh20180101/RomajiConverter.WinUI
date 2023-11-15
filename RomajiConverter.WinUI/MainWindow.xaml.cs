using System;
using System.IO;
using System.Reflection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Newtonsoft.Json;
using RomajiConverter.WinUI.Helpers;
using RomajiConverter.WinUI.Models;
using RomajiConverter.WinUI.Pages;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace RomajiConverter.WinUI;

/// <summary>
/// An empty window that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class MainWindow : Window
{
    private const string ConfigFileName = "config.json";

    public MainWindow()
    {
        InitConfig();
        InitializeComponent();
        ExtendsContentIntoTitleBar = true;
        SetTitleBar(AppTitleBar);
        MainFrame.Navigate(typeof(MainPage));
    }

    private void InitConfig()
    {
        var configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConfigFileName);
        if (File.Exists(configPath))
        {
            App.Config = JsonConvert.DeserializeObject<MyConfig>(File.ReadAllText(configPath));
        }
        else
        {
            App.Config = new MyConfig();
            var file = File.Create(configPath);
            using var sw = new StreamWriter(file);
            sw.Write(JsonConvert.SerializeObject(App.Config, Formatting.Indented));
        }
    }

    private void MainWindow_OnActivated(object sender, WindowActivatedEventArgs args)
    {
        CloudMusicHelper.Init();
        RomajiHelper.Init();
        VariantHelper.Init();
    }

    private void MainWindow_OnClosed(object sender, WindowEventArgs args)
    {
        File.WriteAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConfigFileName),
            JsonConvert.SerializeObject(App.Config, Formatting.Indented));
    }
}