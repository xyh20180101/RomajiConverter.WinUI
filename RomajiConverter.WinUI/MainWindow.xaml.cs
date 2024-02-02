using System;
using System.IO;
using Windows.Graphics;
using Microsoft.UI.Xaml;
using Newtonsoft.Json;
using RomajiConverter.Core.Helpers;
using RomajiConverter.WinUI.Helpers;
using RomajiConverter.WinUI.Models;
using RomajiConverter.WinUI.Pages;

namespace RomajiConverter.WinUI;

public sealed partial class MainWindow : Window
{
    public MainWindow()
    {
        InitConfig();//必须在InitializeComponent前执行

        InitializeComponent();

        ExtendsContentIntoTitleBar = true;
        SetTitleBar(AppTitleBar);
        AppWindow.SetIcon("Assets/icon.ico");
        AppWindow.Resize(new SizeInt32(App.Config.WindowWidth, App.Config.WindowHeight));

        InitHelper();

        MainFrame.Navigate(typeof(MainPage));
    }

    /// <summary>
    /// 初始化应用设置
    /// </summary>
    private void InitConfig()
    {
        var configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, App.ConfigFileName);
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

    private void InitHelper()
    {
        CloudMusicHelper.Init();
        RomajiHelper.Init();
        VariantHelper.Init();
    }

    /// <summary>
    /// 窗口关闭事件(保存应用设置)
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    private void MainWindow_OnClosed(object sender, WindowEventArgs args)
    {
        File.WriteAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, App.ConfigFileName),
            JsonConvert.SerializeObject(App.Config, Formatting.Indented));
    }
}