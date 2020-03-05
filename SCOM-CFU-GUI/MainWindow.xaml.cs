using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.ServiceProcess;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.EnterpriseManagement;
using Microsoft.EnterpriseManagement.Common;
using Microsoft.EnterpriseManagement.Configuration;

namespace SCOM_CFU_GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ObservableCollection<ScomWorkflow> scomRulesAndMonitors = new ObservableCollection<ScomWorkflow>();
        public List<ScomMP> ScomMPs = new List<ScomMP>();

        public MainWindow()
        {
            InitializeComponent();
        }

        ManagementGroup ConnectToScom()
        {
            //ManagementGroup mg = new ManagementGroup("localhost");

            var scomHost = Properties.Settings.Default.scomHost;
            ManagementGroup mg;
            
            try
            {
                mg = ManagementGroup.Connect(scomHost);

                if (mg.IsConnected)
                {
                    Debug.Write("Connection succeeded.");
                    statusText.Text = "Connected";
                    return mg;
                }
                else
                {
                    throw new InvalidOperationException("Not connected to an SDK Service.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                statusText.Text = "Failed to connect";
            }
            return null;
        }

        void GetScomWorkflows(ManagementGroup mg)
        {
            //Get All Rules
            IList<ManagementPackRule> rules = mg.Monitoring.GetRules();

            foreach (var rule in rules)
            {
                Console.WriteLine($"ID: {rule.Id}");
                Console.WriteLine($"Name: {rule.DisplayName}");
                Console.WriteLine($"Target: {rule.Target.ToString()}");
                var mp = rule.GetManagementPack();
                Console.WriteLine($"MP: {mp.DisplayName}");
                Console.WriteLine("-----------------------------------------");
            }

            //Get All Monitors
            IList<ManagementPackMonitor> monitors = mg.Monitoring.GetMonitors();

            foreach (var monitor in monitors)
            {
                Console.WriteLine($"ID: {monitor.Id}");
                Console.WriteLine($"Name: {monitor.DisplayName}");
                Console.WriteLine($"Target: {monitor.Target.ToString()}");
                var mp = monitor.GetManagementPack();
                Console.WriteLine($"MP: {mp.DisplayName}");
                Console.WriteLine("-----------------------------------------");
            }
        }

        void GetScomGroups(ManagementGroup mg)
        {

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            statusText.Text = "Connecting...";
            var mg = ConnectToScom();

            if (mg != null)
            {
                GetScomWorkflows(mg);
            }
        }

        private void MPList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MPList.SelectedIndex < 0)
            {
                return;
            }

            scomRulesAndMonitors.Clear();
            statusText.Text = "Getting Rules and Monitors...";
            GetRulesAndMonitors(ScomMPs[MPList.SelectedIndex].ID);
            statusText.Text = "Done";
            AlertList.ItemsSource = scomRulesAndMonitors;
        }

        private void AlertList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // return if nothing is selected
            if (AlertList.SelectedIndex < 0)
            {
                txtDisplayName.Text = "";
                txtName.Text = "";
                txtId.Text = "";
                return;
            }

            txtDisplayName.Text = scomRulesAndMonitors[AlertList.SelectedIndex].Name;
            txtName.Text = scomRulesAndMonitors[AlertList.SelectedIndex].Name;
            txtId.Text = scomRulesAndMonitors[AlertList.SelectedIndex].ID.ToString();
        }
    }
}
