﻿using System.IO;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace LightCurve
{
    /// <summary> Interaction logic for MainWindow.xaml </summary>
    public partial class MainWindow : Window
    {
        #region 加载、绑定和帮助

        /// <summary> 待处理的所有文件 </summary>
        private List<FileInfo> files = [];

        /// <summary> 在窗口上显示的待处理文件名 </summary>
        private List<string> paths = [];

        /// <summary> 0为无效，1为照片，2为视频 </summary>
        private byte mode = 0;

        public MainWindow() => InitializeComponent();

        private void MW_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F1)
                ShowHelp();
        }

        private static void ShowHelp()
        {
            var version = Assembly.GetExecutingAssembly()
                                  .GetName()
                                  .Version?
                                  .ToString()
                                  ?? "未知";
            Core.Tools.MsgB.OkInfo(new StringBuilder()
                .AppendLine("欢迎使用像素值统计工具！\n")
                .AppendLine("本工具会统计一组图片或一段视频中，")
                .AppendLine("特定区域像素的被测指标的平均值，")
                .AppendLine("然后绘制为一个折线图，或输出为一个列表。")
                .AppendLine("详见README.md。\n")
                .AppendLine($"版本号：{version}")
                .AppendLine("作者：GarthTB\n")
                .ToString(), "帮助");
        }

        #endregion

        #region 选区

        private void CBFullFrame_Checked(object sender, RoutedEventArgs e) => RefreshRange();

        private void CBFullFrame_Unchecked(object sender, RoutedEventArgs e) => RefreshRange();

        private void RefreshRange()
        {
            if (TBRangeX is null
                || TBRangeY is null
                || TBRangeW is null
                || TBRangeH is null)
                return;

            TBRangeX.IsEnabled =
            TBRangeY.IsEnabled =
            TBRangeW.IsEnabled =
            TBRangeH.IsEnabled = CBFullFrame.IsChecked == false;
        }

        private void TBRangeX_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (TBRangeX.Text.Length == 0 || !uint.TryParse(TBRangeX.Text, out _))
                TBRangeX.Text = "0";
        }

        private void TBRangeY_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (TBRangeY.Text.Length == 0 || !uint.TryParse(TBRangeY.Text, out _))
                TBRangeY.Text = "0";
        }

        private void TBRangeW_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (TBRangeW.Text.Length == 0 || !uint.TryParse(TBRangeW.Text, out var w) || w == 0)
                TBRangeW.Text = "1";
        }

        private void TBRangeH_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (TBRangeH.Text.Length == 0 || !uint.TryParse(TBRangeH.Text, out var h) || h == 0)
                TBRangeH.Text = "1";
        }

        #endregion

        #region 选择输出文件夹

        private void BtSelOutputDir_Click(object sender, RoutedEventArgs e)
            => TBOutputDir.Text = Core.Tools.File.SelectOutputDir(CBOutputType.SelectedIndex);

        private void TBOutputDir_TextChanged(object sender, TextChangedEventArgs e)
            => BtRun.IsEnabled = files.Count > 0 && TBOutputDir.Text.Length > 0;

        #endregion

        #region 添加和移除文件

        private void BtAddPaths_Click(object sender, RoutedEventArgs e)
        {
            var fileNames = Core.Tools.File.PickInputs();
            if (fileNames.Length == 0) return;

            MatchType(fileNames);
        }

        private void LBPaths_SelectionChanged(object sender, SelectionChangedEventArgs e)
            => BtRemovePaths.IsEnabled = LBPaths.SelectedItems.Count > 0;

        private void BtRemovePaths_Click(object sender, RoutedEventArgs e)
        {
            var sel_paths = LBPaths.SelectedItems.Cast<string>().ToArray();
            _ = files.RemoveAll(x => sel_paths.Contains(x.FullName));
            paths = files.Select(x => x.FullName).ToList();
            LBPaths.ItemsSource = paths;
            if (files.Count == 0)
            {
                mode = 0;
                TBOutputDir.Text = "";
                BtRemovePaths.IsEnabled =
                BtRun.IsEnabled =
                CBOrder.IsEnabled =
                CBDescending.IsEnabled = false;
            }
        }

        private void MatchType(string[] fileNames)
        {
            if (Core.Tools.File.IsImages(fileNames))
            {
                if (mode == 2)
                    Core.Tools.MsgB.OkInfo("添加的文件与原先的类型不一致，未添加！", "提示");
                else
                {
                    if (mode == 0) mode = 1;
                    LoadAndActivate(fileNames);
                }
            }
            else if (Core.Tools.File.IsVideos(fileNames))
            {
                if (mode == 1)
                    Core.Tools.MsgB.OkInfo("添加的文件与原先的类型不一致，未添加！", "提示");
                else
                {
                    if (mode == 0) mode = 2;
                    LoadAndActivate(fileNames);
                }
            }
            else Core.Tools.MsgB.OkErr("添加的文件不全为图片也不全为视频，未添加！", "错误");

            void LoadAndActivate(string[] fileNames)
            {
                files.AddRange(fileNames.Select(fileName => new FileInfo(fileName)));
                ReOrderFiles(CBOrder.SelectedIndex, CBDescending.IsChecked == true);

                if (TBOutputDir.Text.Length == 0)
                    TBOutputDir.Text = files[0].DirectoryName ?? "";

                CBOrder.IsEnabled = CBDescending.IsEnabled = true;
                BtRun.IsEnabled = TBOutputDir.Text.Length > 0;
            }
        }

        #endregion

        #region 拖动文件

        private void LBPaths_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                MatchType((string[])e.Data.GetData(DataFormats.FileDrop));
        }

        #endregion

        #region 排序文件

        private void CBOrder_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CBDescending is not null)
                ReOrderFiles(CBOrder.SelectedIndex, CBDescending.IsChecked == true);
        }

        private void CBDescending_Checked(object sender, RoutedEventArgs e)
        {
            if (CBOrder is not null)
                ReOrderFiles(CBOrder.SelectedIndex, CBDescending.IsChecked == true);
        }

        private void CBDescending_Unchecked(object sender, RoutedEventArgs e)
        {
            if (CBOrder is not null)
                ReOrderFiles(CBOrder.SelectedIndex, CBDescending.IsChecked == true);
        }

        private void ReOrderFiles(int orderIndex, bool descending)
        {
            Func<FileInfo, object> judge = orderIndex switch
            {
                1 => x => x.CreationTime,
                2 => x => x.LastWriteTime,
                3 => x => x.Length,
                _ => x => x.Name, // 0也是按文件名
            };
            files = [.. descending
                ? files.OrderByDescending(judge)
                : files.OrderBy(judge)];
            paths = files.Select(x => x.FullName).ToList();
            LBPaths.ItemsSource = paths;
        }

        #endregion

        #region 开始分析

        private void BtRun_Click(object sender, RoutedEventArgs e)
        {
            MW.Title = "LightCurve 处理中，请勿关闭...";
            if (mode == 1) // 图片
            {
                Core.ImgAna analyzer = CBFullFrame.IsChecked == true
                    ? new(files,
                        CBChannel.SelectedIndex,
                        null,
                        null,
                        null,
                        null,
                        CBOutputType.SelectedIndex,
                        TBOutputDir.Text)
                    : new(files,
                        CBChannel.SelectedIndex,
                        uint.Parse(TBRangeX.Text),
                        uint.Parse(TBRangeY.Text),
                        uint.Parse(TBRangeW.Text),
                        uint.Parse(TBRangeH.Text),
                        CBOutputType.SelectedIndex,
                        TBOutputDir.Text);
                analyzer.Run();
            }
            if (mode == 2) // 视频
            {
                Core.VidAna analyzer = CBFullFrame.IsChecked == true
                    ? new(files,
                        CBChannel.SelectedIndex,
                        null,
                        null,
                        null,
                        null,
                        CBOutputType.SelectedIndex,
                        TBOutputDir.Text)
                    : new(files,
                        CBChannel.SelectedIndex,
                        uint.Parse(TBRangeX.Text),
                        uint.Parse(TBRangeY.Text),
                        uint.Parse(TBRangeW.Text),
                        uint.Parse(TBRangeH.Text),
                        CBOutputType.SelectedIndex,
                        TBOutputDir.Text);
                analyzer.Run();
            }
            MW.Title = "LightCurve";
        }

        #endregion
    }
}
