using System.Windows;
using System.Windows.Input;

namespace LightCurve
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
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
    }
}