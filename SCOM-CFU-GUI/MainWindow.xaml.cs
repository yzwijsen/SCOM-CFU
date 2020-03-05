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
        public ObservableCollection<ScomRuleMonitor> scomRulesAndMonitors = new ObservableCollection<ScomRuleMonitor>();
        public List<ScomMP> ScomMPs = new List<ScomMP>();

        private ManagementGroup mg;

        public MainWindow()
        {
            InitializeComponent();
        }

        void ConnectToScom()
        {
            //ManagementGroup mg = new ManagementGroup("localhost");

            var scomHost = Properties.Settings.Default.scomHost;
            
            try
            {
                mg = ManagementGroup.Connect(scomHost);

                if (mg.IsConnected)
                {
                    Debug.Write("Connection succeeded.");
                    statusText.Text = "Connected";
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
        }

        void GetManagementPacks()
        {
            statusText.Text = "Loading MP List...";

            // Get the Management Packs
            IList<ManagementPack> managementPacks = mg.ManagementPacks.GetManagementPacks();

            foreach(var mp in managementPacks)
            {
                var displayName = mp.DisplayName;

                //check if mp has a display name
                if (String.IsNullOrEmpty(mp.DisplayName))
                {
                    displayName = mp.Name;
                }

                //if displayname is still empty we don't add this mp to the list
                if (String.IsNullOrEmpty(displayName))
                {
                    return;
                }

                ScomMPs.Add(new ScomMP { DisplayName = displayName, ID = mp.Id, Name = mp.Name});
            }
            ScomMPs.Sort((x, y) => x.DisplayName.CompareTo(y.DisplayName));
            statusText.Text = "Done";
            MPList.ItemsSource = ScomMPs;
        }

        void GetRulesAndMonitors(Guid mpID)
        {
            // Get the Management Pack.
            string query = "Id = '" + mpID.ToString() + "'";
            ManagementPackCriteria mpCriteria = new ManagementPackCriteria(query);
            IList<ManagementPack> managementPacks = mg.ManagementPacks.GetManagementPacks(mpCriteria);
            if (managementPacks.Count != 1)
                throw new InvalidOperationException("Expected one Management Pack with " + query);

            //get all the rules
            ManagementPackElementCollection<ManagementPackRule> rules = managementPacks[0].GetRules();

            if (rules.Count > 0)
            {
                foreach (ManagementPackRule rule in rules)
                {
                    scomRulesAndMonitors.Add(new ScomRuleMonitor { Name = rule.Name, DisplayName = rule.DisplayName, Type = AlertSourceType.Rule, ID = rule.Id });
                }
            }

            //get all the monitors
            ManagementPackElementCollection<ManagementPackMonitor> monitors = managementPacks[0].GetMonitors();

            if (monitors.Count > 0)
            {
                foreach(ManagementPackMonitor monitor in monitors)
                {
                    scomRulesAndMonitors.Add(new ScomRuleMonitor { Name = monitor.Name, DisplayName = monitor.DisplayName, Type = AlertSourceType.Monitor, ID = monitor.Id });
                }
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            statusText.Text = "Connecting...";
            ConnectToScom();

            if (mg != null)
            {
                GetManagementPacks();
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

            txtDisplayName.Text = scomRulesAndMonitors[AlertList.SelectedIndex].DisplayName;
            txtName.Text = scomRulesAndMonitors[AlertList.SelectedIndex].Name;
            txtId.Text = scomRulesAndMonitors[AlertList.SelectedIndex].ID.ToString();
        }
    }
}
