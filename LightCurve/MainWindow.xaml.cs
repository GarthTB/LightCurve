using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace LightCurve
{
    /// <summary> Interaction logic for MainWindow.xaml </summary>
    public partial class MainWindow : Window
    {
        #region 加载、绑定和帮助

        private List<FileInfo> files = [];

        public MainWindow() => InitializeComponent();

        private void MW_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F1)
                Core.Help.Show();
        }

        #endregion

        #region 选区

        private void CBRange_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TBRangeX is null
                || TBRangeY is null
                || TBRangeW is null
                || TBRangeH is null)
                return;

            TBRangeX.IsEnabled =
            TBRangeY.IsEnabled =
            TBRangeW.IsEnabled =
            TBRangeH.IsEnabled = CBRange.SelectedIndex == 1;
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

        private void BtSelOutputPath_Click(object sender, RoutedEventArgs e)
            => TBOutputPath.Text = Core.Tools.File.SelectOutputDir(CBOutputType.SelectedIndex);

        private void TBOutputPath_TextChanged(object sender, TextChangedEventArgs e)
            => BtRun.IsEnabled = BtRemovePaths.IsEnabled && TBOutputPath.Text.Length > 0;

        #endregion

        #region 添加和移除文件

        private void BtAddPaths_Click(object sender, RoutedEventArgs e)
        {
            var fileNames = Core.Tools.File.PickInputs();
            if (fileNames.Length == 0) return;

        }

        private void BtRemovePaths_Click(object sender, RoutedEventArgs e)
        {

        }

        #endregion

        #region 排序文件

        private void CBOrder_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CBDescending is not null)
                ReOrderPaths(CBOrder.SelectedIndex, CBDescending.IsChecked == true);
        }

        private void CBDescending_Checked(object sender, RoutedEventArgs e)
        {
            if (CBOrder is not null)
                ReOrderPaths(CBOrder.SelectedIndex, CBDescending.IsChecked == true);
        }

        private void ReOrderPaths(int orderIndex, bool descending)
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
        }

        #endregion
    }
}
