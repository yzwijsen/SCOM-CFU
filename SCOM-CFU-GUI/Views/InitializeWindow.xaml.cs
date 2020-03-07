using SCOM_CFU_GUI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        private ScomDataViewModel viewModel;
        public InitializeWindow()
        {
            InitializeComponent();
        }
    }
}
