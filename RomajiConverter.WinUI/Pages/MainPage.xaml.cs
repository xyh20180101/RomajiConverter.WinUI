using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using RomajiConverter.WinUI.Helpers;
using RomajiConverter.WinUI.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using Windows.Foundation;
using Windows.Foundation.Collections;
using RomajiConverter.WinUI.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace RomajiConverter.WinUI.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        /// <summary>
        /// 当前的转换结果集合
        /// </summary>
        private List<ConvertedLine> _convertedLineList = new List<ConvertedLine>();

        /// <summary>
        /// 简易模式最窄宽度
        /// </summary>
        private const int _simpleModeMinWidth = 900;

        /// <summary>
        /// 详细模式最窄宽度
        /// </summary>
        private const int _detailModeMinWidth = 1200;

        public MainPage()
        {
            this.InitializeComponent();

            SpaceCheckBox.Toggled += ThirdCheckBox_OnToggled;
            NewLineCheckBox.Toggled += ThirdCheckBox_OnToggled;
            RomajiCheckBox.Toggled += ThirdCheckBox_OnToggled;
            HiraganaCheckBox.Toggled += ThirdCheckBox_OnToggled;
            JPCheckBox.Toggled += ThirdCheckBox_OnToggled;
            KanjiHiraganaCheckBox.Toggled += ThirdCheckBox_OnToggled;
            CHCheckBox.Toggled += ThirdCheckBox_OnToggled;

            IsDetailMode = true;
        }

        private async void ImportCloudMusicButton_OnClick(object sender, RoutedEventArgs e)
        {
            ShowLrc(await CloudMusicHelper.GetLrc(CloudMusicHelper.GetLastSongId()));
        }

        private void ShowLrc(List<ReturnLrc> lrc)
        {
            var stringBuilder = new StringBuilder();
            foreach (var item in lrc)
            {
                stringBuilder.AppendLine(item.JLrc);
                stringBuilder.AppendLine(item.CLrc);
            }

            InputTextBox.Text = stringBuilder.ToString();
        }

        private void ConvertButton_OnClick(object sender, RoutedEventArgs e)
        {
            _convertedLineList = RomajiHelper.ToRomaji(InputTextBox.Text, SpaceCheckBox.IsOn, AutoVariantCheckBox.IsChecked.Value);

            if (IsDetailMode)
                RenderEditPanel();
            else
                OutputTextBox.Text = GetResultText();
        }

        private void RenderEditPanel()
        {
            EditPanel.Children.Clear();
            for (var i = 0; i < _convertedLineList.Count; i++)
            {
                var item = _convertedLineList[i];

                var line = new VariableSizedWrapGrid { };
                foreach (var unit in item.Units)
                {
                    var group = new EditableLabelGroup(unit);
                    group.RomajiVisibility = EditRomajiCheckBox.IsOn ? Visibility.Visible : Visibility.Collapsed;
                    if (EditHiraganaCheckBox.IsOn == true)
                    {
                        if (IsOnlyShowKanjiCheckBox.IsOn == true && group.Unit.IsKanji == false)
                        {
                            group.HiraganaVisibility = Visibility.Collapsed;
                        }
                        else
                        {
                            group.HiraganaVisibility = Visibility.Visible;
                        }
                    }
                    else
                    {
                        group.HiraganaVisibility = Visibility.Collapsed;
                    }
                    line.Children.Add(group);
                }

                EditPanel.Children.Add(line);
                if (item.Units.Any() && i < _convertedLineList.Count - 1)
                    EditPanel.Children.Add(new Button());
            }
        }

        private bool _isDetailMode;
        public bool IsDetailMode
        {
            get => _isDetailMode;
            set
            {
                _isDetailMode = value;
                SwitchMode(_isDetailMode);
            }
        }

        private ColumnDefinition _editColumnDefinition = new ColumnDefinition();

        private void SwitchMode(bool isDetailMode)
        {
            if (this.IsLoaded)
            {
                if (isDetailMode)
                {
                    this.Width = _detailModeMinWidth;
                    this.MinWidth = _detailModeMinWidth;
                    if (MainGrid.ColumnDefinitions.Count == 2) MainGrid.ColumnDefinitions.Insert(1, _editColumnDefinition);
                    ReadButton.Visibility = Visibility.Visible;
                    SaveButton.Visibility = Visibility.Visible;
                    EditHiraganaCheckBox.Visibility = Visibility.Visible;
                    EditRomajiCheckBox.Visibility = Visibility.Visible;
                    IsOnlyShowKanjiCheckBox.Visibility = Visibility.Visible;
                    ConvertPictureButton.Visibility = Visibility.Visible;
                    ConvertTextButton.Visibility = Visibility.Visible;
                }
                else
                {
                    ReadButton.Visibility = Visibility.Collapsed;
                    SaveButton.Visibility = Visibility.Collapsed;
                    EditHiraganaCheckBox.Visibility = Visibility.Collapsed;
                    EditRomajiCheckBox.Visibility = Visibility.Collapsed;
                    IsOnlyShowKanjiCheckBox.Visibility = Visibility.Collapsed;
                    ConvertPictureButton.Visibility = Visibility.Collapsed;
                    ConvertTextButton.Visibility = Visibility.Collapsed;
                    _editColumnDefinition = MainGrid.ColumnDefinitions[1];
                    if (MainGrid.ColumnDefinitions.Count == 3) MainGrid.ColumnDefinitions.RemoveAt(1);
                    this.MinWidth = _simpleModeMinWidth;
                    this.Width = _simpleModeMinWidth;
                }
            }
        }

        private string GetResultText()
        {
            string GetString(IEnumerable<string> array)
            {
                return string.Join(SpaceCheckBox.IsOn ? " " : "", array);
            }

            var output = new StringBuilder();
            for (var i = 0; i < _convertedLineList.Count; i++)
            {
                var item = _convertedLineList[i];
                if (RomajiCheckBox.IsOn)
                    output.AppendLine(GetString(item.Units.Select(p => p.Romaji)));
                if (HiraganaCheckBox.IsOn)
                    output.AppendLine(GetString(item.Units.Select(p => p.Hiragana)));
                if (JPCheckBox.IsOn)
                {
                    if (KanjiHiraganaCheckBox.IsOn)
                    {
                        var japanese = item.Japanese;
                        var leftParenthesis = "(";
                        var rightParenthesis = ")";

                        var kanjiUnitList = item.Units.Where(p => p.IsKanji);
                        foreach (var kanjiUnit in kanjiUnitList)
                        {
                            var kanjiIndex = japanese.IndexOf(kanjiUnit.Japanese);
                            var hiraganaIndex = kanjiIndex + kanjiUnit.Japanese.Length;
                            japanese = japanese.Insert(hiraganaIndex, $"{leftParenthesis}{kanjiUnit.Hiragana}{rightParenthesis}");
                        }
                        output.AppendLine(japanese);
                    }
                    else
                    {
                        output.AppendLine(item.Japanese);
                    }
                }
                if (CHCheckBox.IsOn && !string.IsNullOrWhiteSpace(item.Chinese))
                    output.AppendLine(item.Chinese);
                if (NewLineCheckBox.IsOn && i < _convertedLineList.Count - 1)
                    output.AppendLine();
            }
            if (_convertedLineList.Any()) output.Remove(output.Length - Environment.NewLine.Length, Environment.NewLine.Length);
            return output.ToString();
        }

        private void ThirdCheckBox_OnToggled(object sender, RoutedEventArgs e)
        {
            OutputTextBox.Text = GetResultText();
        }
    }
}
