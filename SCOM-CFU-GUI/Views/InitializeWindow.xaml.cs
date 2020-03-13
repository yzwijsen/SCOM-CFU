using SCOM_CFU_GUI.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SCOM_CFU_GUI.Views
{
    /// <summary>
    /// Interaction logic for InitializeWindow.xaml
    /// </summary>
    public partial class InitializeWindow : Window
    {

        private bool isDataInitCompleted;

        public InitializeWindow()
        {
            InitializeComponent();

            var vm = Application.Current.Resources["mainViewModel"] as MainViewModel;
            vm.DataInitCompleted += async (s, e) => await CloseWindowDelayed();
            
        }

        async Task CloseWindowDelayed()
        {
            isDataInitCompleted = true;
            await Task.Delay(1000);
            this.Close();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
             if (!isDataInitCompleted) Application.Current.Shutdown();
        }
    }
}
