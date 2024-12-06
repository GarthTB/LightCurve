using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace LightCurve
{
    /// <summary> Interaction logic for MainWindow.xaml </summary>
    public partial class MainWindow : Window
    {
        #region 加载和帮助

        public MainWindow() => InitializeComponent();

        private void MW_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F1)
                Core.Help.Show();
        }

        private void BtHelp_Click(object sender, RoutedEventArgs e)
            => Core.Help.Show();

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
            => BtRun.IsEnabled = BtRemoveFiles.IsEnabled && TBOutputPath.Text.Length > 0;

        #endregion
    }
}
