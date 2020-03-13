using SCOM_CFU_GUI.Models;
using SCOM_CFU_GUI.ViewModels;
using System.Windows;

namespace SCOM_CFU_GUI.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private MainViewModel vm;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var initWindow = new InitializeWindow();
            initWindow.Owner = this;
            initWindow.ShowDialog();

            vm = Application.Current.Resources["mainViewModel"] as MainViewModel;
        }

        private void mpTreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            SetSelectedItem();
        }

        private void SetSelectedItem()
        {
            vm.SelectedConfigTarget = mpTreeView.SelectedItem as IConfigurationTarget;
        }

        private void targetStack_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            SetSelectedItem();
        }

        private void mpStack_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            SetSelectedItem();
        }
    }
}
