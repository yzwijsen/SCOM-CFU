using SCOM_CFU_GUI.ViewModels;
using System.Windows;

namespace SCOM_CFU_GUI.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var initWindow = new InitializeWindow();
            initWindow.ShowDialog();
        }
    }
}
